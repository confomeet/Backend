using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.RecordingDto
{
    public class S3RecordingPostDto
    {
        [Required]
        public int MeetingId { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }


        [Required]
        public string Bucket { get; set; } = string.Empty;

        [Required]
        public string Key { get; set; } = string.Empty;
    }
}
