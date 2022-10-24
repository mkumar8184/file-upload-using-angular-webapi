
using AttachmentDomain.Entity;
using AttachmentUtility.Models;

namespace AttachmentUtility
{
    public interface IAttachmentService
    {
        Task<FileUploadResult> UploadFile(FileCommand command);
      
        void DeleleFileById(int id);
       
        MemoryStream DownloadFile(int fileId, out string fileName);
       
        string GetContentType(string path);
    }
}
