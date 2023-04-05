namespace VideoProjectCore6.DTOs.EventDto
#nullable disable
{
    public class EventLogView
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string Note { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RelatedId { get; set; }
    }
}
