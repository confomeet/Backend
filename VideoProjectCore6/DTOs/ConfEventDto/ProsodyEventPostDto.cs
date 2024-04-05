namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ProsodyEventPostDto
    {

        public string type { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;

        public string to { get; set; } = string.Empty;

        public int meetingId { get; set; }

        public string message { get; set; } = string.Empty;
    }
}
