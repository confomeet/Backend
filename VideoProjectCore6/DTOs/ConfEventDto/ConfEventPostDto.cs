using VideoProjectCore6.Services;

namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ConfEventPostDto
    {

        public int Id { get; set; }
        public DateTime EventTime { get; set; }

        public Constants.EVENT_TYPE EventType { get; set; }

        public string ConfId { get; set; } = string.Empty;

        public string UserId { get; set; } = "string";

        public string EventInfo { get; set; } = string.Empty;

        public string MeetingId { get; set; } = "10000";

    }
}
