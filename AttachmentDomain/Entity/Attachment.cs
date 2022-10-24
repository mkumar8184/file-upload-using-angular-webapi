using System;
using System.Collections.Generic;

namespace AttachmentDomain.Entity
{
    public partial class Attachment
    {
        public int Id { get; set; }
        public string? Extension { get; set; }
        public byte[]? Documents { get; set; }
        public string? DocumentName { get; set; }
        public DateTime? UploadedOn { get; set; }
        public int? UploadedBy { get; set; }
        public bool? IsActive { get; set; }
        public string? DocumentPath { get; set; }
        public int? FileTypeId { get; set; }
        public string? Base64Url { get; set; }
        public string? DrivePath { get; set; }
        public string? VirtualFileName { get; set; }
    }
}
