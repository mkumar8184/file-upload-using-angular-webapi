namespace AttachmentUtility.Models
{
    public class FileUploadResult
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Extension { get; set; }
        public int? FileTypeId { get; set; }
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
        public string FileBase64String { get; set; }
      public string DownloadLink { get; set; }
    }
}
