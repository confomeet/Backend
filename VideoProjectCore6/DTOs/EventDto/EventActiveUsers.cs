
#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class EventActiveUsers
    {

        public string MeetingId { get; set; }
        public string Topic { get; set; }

        public int? AllParticipants { get; set; }

        public int? OnlineParticipants { get; set; }
    }
}
