using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.Models;

#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class EventViewDetails
    {
        public string Topic { get; set; }
        public string MeetingId { get; set; }
        public List<ConfEventCompactGet> EventLogs { get; set; }

        public string VideoLink { get; set; }
    }
}
