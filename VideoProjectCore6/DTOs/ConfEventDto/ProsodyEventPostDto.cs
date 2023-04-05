namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ProsodyEventPostDto
    {

        public string type { get; set; }

        public string from { get; set; }

        public string to { get; set; }

        public int meetingId { get; set; }

        public string message { get; set; }
    }
}
