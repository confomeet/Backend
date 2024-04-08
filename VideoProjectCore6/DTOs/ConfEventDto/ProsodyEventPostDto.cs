using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ProsodyEventPostDto
    {

        public string type { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;

        public string to { get; set; } = string.Empty;

        [Required]
        public string meetingId { get; set; } = string.Empty;

        public string message { get; set; } = string.Empty;
    }
}
