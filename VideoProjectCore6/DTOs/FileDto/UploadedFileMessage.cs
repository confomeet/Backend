#nullable disable

namespace VideoProjectCore6.DTOs.FileDto
{
    public class UploadedFileMessage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool SuccessUpload { get; set; }
        public string Message { get; set; }
        public string FileUrl { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }
}
