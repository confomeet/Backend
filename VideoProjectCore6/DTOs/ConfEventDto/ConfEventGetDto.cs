namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ConfEventGetDto
    {
        public int Id { get; set; }
        public DateTime EventTime { get; set; }

        public int EventType { get; set; }

        public int ConfId { get; set; }

        public int UserId { get; set; }

        public string EventInfo { get; set; } = string.Empty;

    }
}
