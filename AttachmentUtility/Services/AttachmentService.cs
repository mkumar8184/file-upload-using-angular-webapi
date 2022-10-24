
using AttachmentDomain.Entity;
using AttachmentUtility.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Attachment = AttachmentDomain.Entity.Attachment;

namespace AttachmentUtility
{
    public class AttachmentService : IAttachmentService
    {
     
        private IHostingEnvironment _hostingEnvironment;

        private IConfiguration _configuration;
        private readonly CloudStorageAccount _storageAccount;
        private readonly ILogger<AttachmentService> _logger;
        private readonly AttachmentExampleContext _context;
        
        public AttachmentService(IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
           ILogger<AttachmentService> logger,
           AttachmentExampleContext context
          

             )
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _storageAccount = CloudStorageAccount.Parse(_configuration["AppConfig:AzureFileConnection"]);
            _logger = logger;
            _context = context;
          


        }
        public async Task<FileUploadResult> UploadFile(FileCommand command)
        {
            try
            {
                if (command.File == null )
                {
                    throw new Exception ("No File Uploaded");
                }
                if (Convert.ToBoolean(_configuration["AppConfig:UploadToAzure"]) == true)
                {
                    return await AzureUpload(command);
                }
                else
                {
                    return await UploadInLocalDirectory(command);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in uploading file with {ex}");

            }
     

        }
        public async Task<FileUploadResult> UploadInLocalDirectory(FileCommand command)
        {
            var _fileResult = new FileUploadResult();
            try
            {
             
                var file = command.File;
                string folderName = "Upload";// can be move in appsetting.json
                var pathToSave = Path.Combine(_hostingEnvironment.WebRootPath, folderName);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                if (file.Length > 0)
                {
                    var byteFile = GetBytes(file);
                    if (byteFile == null || byteFile.Length == 0)
                    {
                        throw new Exception( "Error in converting file ");

                    }
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                    string fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);

                    }
                    var downloadPath =
                        Path.Combine(_configuration["AppConfig:DownloadPath"], folderName + "/" + fileName);
                    var attachmentCommand = new Attachment
                    {
                        DocumentName = file.FileName,
                        DocumentPath = downloadPath,
                        Documents = Convert.ToBoolean(_configuration["AppConfig:StoreFileInDB"]) ? byteFile :  new byte[0],
                        Extension = Path.GetExtension(fileName),
                        IsActive = true,
                        UploadedOn = DateTime.Now,
                        FileTypeId = command.FileTypeId,
                        Base64Url = GetByteToBase64(byteFile, Path.GetExtension(fileName))
                    };

                    await _context.Attachments.AddAsync(attachmentCommand);
                    await _context.SaveChangesAsync();

                    _fileResult.Id = attachmentCommand.Id;
                    _fileResult.FileName = attachmentCommand.DocumentName;
                    _fileResult.Extension = attachmentCommand.Extension;
                    _fileResult.FilePath = attachmentCommand.DocumentPath;
                    _fileResult.FileBase64String = attachmentCommand.Base64Url;
                    _fileResult.FileContent = byteFile;
                    _fileResult.DownloadLink = attachmentCommand.DocumentPath;
                    return _fileResult;


                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error in uploading file with {ex}");
            }
            return _fileResult;
        }
        private string PrepareAzurePath(string docPath)
        {
            if (!string.IsNullOrWhiteSpace(docPath))
            {
                return docPath + "?sv=" + _configuration["AppConfig:SVKey"];
            }
            return string.Empty;
        }
        public string GetByteToBase64(byte[] byteContent, string extension)
        {
            if (extension.Contains(".mp4"))
            {
                return "data:video/mp4;base64," + System.Convert.ToBase64String(byteContent);
            }
            return "data:image/jpeg;base64," + System.Convert.ToBase64String(byteContent);
        }
       
       
        public byte[] GetBytes(IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public void DeleleFileById(int id)
        {
            var attachement = _context.Attachments.FirstOrDefault(x=>x.Id==id);
            if (attachement != null)
            {
                _context.Attachments.Remove(attachement);
                _context.SaveChangesAsync();
                
            }
            //here you can write code to remove from azure/local file storage
            //Write e from from storage
        }
        public async Task<FileUploadResult> AzureUpload(FileCommand command)
        {
            string fileName;
            byte[] fileInBytes;
            var file = command.File;
            var _fileResult = new FileUploadResult();
            fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            fileInBytes = GetBytes(file);
            if (fileInBytes == null || fileInBytes.Length == 0)
            {
                throw new Exception("Error in converting file ");

            }

            CloudFileClient fileClient = _storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference(_configuration["AppConfig:AzureFileShare"]);

            if (share.ExistsAsync().Result)
            {
                try
                {
                    CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                    CloudFileDirectory directoryRef;

                    directoryRef = rootDir.GetDirectoryReference(_configuration["AppConfig:AzureDirectory"]);
                    await directoryRef.CreateIfNotExistsAsync();
                    CloudFile cloudFile = directoryRef.GetFileReference(fileName);
                     await  cloudFile.UploadFromByteArrayAsync(fileInBytes, 0, fileInBytes.Count());


                    var downloadPath = cloudFile.Uri; 
                    var attachmentCommand = new Attachment
                    {
                        DocumentName = file.FileName,
                        DocumentPath = PrepareAzurePath(downloadPath.ToString()),
                        Documents = Convert.ToBoolean(_configuration["AppConfig:StoreFileInDB"])? fileInBytes : null,
                        Extension = Path.GetExtension(fileName),
                        IsActive = true,
                        UploadedOn = DateTime.Now,
                        FileTypeId = command.FileTypeId,
                        Base64Url = GetByteToBase64(fileInBytes, Path.GetExtension(fileName)),
                        VirtualFileName = fileName
                        
                    };

                     await _context.Attachments.AddAsync(attachmentCommand);
                     await _context.SaveChangesAsync();
                   _fileResult.Id = attachmentCommand.Id;
                    _fileResult.FileName = attachmentCommand.DocumentName;
                    _fileResult.Extension = attachmentCommand.Extension;
                    _fileResult.FilePath = attachmentCommand.DocumentPath;                  
                    _fileResult.FileBase64String = attachmentCommand.Base64Url;
                    _fileResult.FileContent = fileInBytes;
                    _fileResult.DownloadLink = attachmentCommand.DocumentPath;
                   
                    return _fileResult;

                    
                }
                catch (Exception ex)
                {
                    _logger.LogError("Uploading file to azure exception: " + ex.Message);
                    throw new Exception("exception Uploading file to azure");
                }
            }
            else
            {
               // Log.Error("Document share does not exist!");
                throw new Exception("Can't file storage account");
            }
          
        }
        public Attachment GetFileDetails(int fileId)
        {
            var result =  _context.Attachments.FirstOrDefault(x => x.Id == fileId);
           
            if (result == null)
            {
                return null;
            }
            return result;
        }
        //download stream from file name only from azure storage
        public MemoryStream DownloadFileStream(string fileName)
        {
            MemoryStream ms = new MemoryStream();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration["AppConfig:AzureFileConnection"]);
            CloudFileClient cloudFileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare cloudFileShare = cloudFileClient.GetShareReference(_configuration["AppConfig:AzureFileShare"]);
            if (cloudFileShare.ExistsAsync().Result)
            {
                CloudFileDirectory rootDir = cloudFileShare.GetRootDirectoryReference();
                CloudFileDirectory workarea = rootDir.GetDirectoryReference(_configuration["AppConfig:AzureDirectory"]);
                if (workarea.ExistsAsync().Result)
                {
                    CloudFile cloudFile = workarea.GetFileReference(fileName);

                    if (cloudFile.ExistsAsync().Result)
                    {
                         cloudFile.DownloadToStreamAsync(ms);                       
                       
                        cloudFile.OpenReadAsync().Result.CopyTo(ms);
                       
                    }
                }
            }
            return ms;
          
        }
        public MemoryStream DownloadFile(int fileId, out string fileName)
        {
            var result = GetFileDetails(fileId);
            fileName = result.DocumentName;
            var _webclient = new WebClient();// you can inject it
            var data = _webclient.DownloadData(result.DocumentPath);
            var memory = new MemoryStream(data);
            return memory;
        }      
        public string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

    }
}
