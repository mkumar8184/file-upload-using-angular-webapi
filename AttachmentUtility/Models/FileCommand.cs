using Microsoft.AspNetCore.Http;

namespace AttachmentUtility.Models
{
    public class FileCommand
    {
        public int FileTypeId { get; set; }
        public IFormFile File { get; set; }
      
    }
}
