namespace VideoProjectCore6.DTOs.EventDto
#nullable disable
{
    public class EventDto
    {
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string Organizer { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public string MeetingId { get; set; } = null;
        public sbyte? Status { get; set; }


    }
}
