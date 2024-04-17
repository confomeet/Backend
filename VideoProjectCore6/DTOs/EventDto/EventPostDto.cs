using System.ComponentModel.DataAnnotations;

using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class EventPostDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short? Type { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string Organizer { get; set; }
        public string MeetingId { get; set; } = null;
        public bool MeetingRequired { get; set; } = true;
        public string TimeZone { get; set; }
        public string Password { get; set; }
        public sbyte? Status { get; set; }
        public bool? AllDay { get; set; }
        public int? TypeOrder { get; set; }
        public bool? RecordingReq { get; set; }
        public bool? SingleAccess { get; set; }
        public bool? AutoLobby { get; set; }
        //public bool Cisco { get; set; } = false;
        public Event GetEntity()
        {
            Event queueProcess = new Event
            {
                Topic = Topic,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                Type = Type,
                MeetingId = MeetingId,                
                TimeZone = TimeZone,
                
            };
            return queueProcess;
        }
    }
}
