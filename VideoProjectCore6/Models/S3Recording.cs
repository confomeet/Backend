using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.Models
{
    public class S3Recording
    {
        [Key]
        public Guid Uuid { get; set; }

        [Required]
        public int RecordingLog { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }


        [Required]
        public string Bucket { get; set; } = string.Empty;

        [Required]
        public string Key { get; set; } = string.Empty;
    };
}