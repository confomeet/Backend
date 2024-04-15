using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.DTOs.EventDto
#nullable disable
{
    public class EventFullView
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public bool ByMe { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string Organizer { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }     
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public short? Type  { get; set; }
        public int? ParentEventId { get; set; }
        public string MeetingId { get; set; }
        public string MeetingLink { get; set; }
        public string Password { get; set; }
        public bool PasswordReq { get; set; }
        public bool? RecordingReq { get; set; }

        public bool? SingleAccess { get; set; }

        public bool? AllDay { get; set; }

        public bool? AutoLobby { get; set; }

        public sbyte? Status { get; set; }
        public string StatusText { get; set; }
        public int SubEventCount { get; set; }
        public bool ToHide { get; set; } = false;
        
    public EventStatus MeetingStatus { get; set; }

        public List<ConfEventCompactGet> EventLogs { get; set; }

        public List<RecordingLog> VideoLogs { get; set; }

        public  List<ParticipantView> Participants { get; set; }

        public EventFullView ParentEvent { get; set; }
        public List<EventFullView> SubEvents { get; set; }

    }
}
