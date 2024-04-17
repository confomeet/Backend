using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.MeetingDto
{
    public class MeetingPostDto
    {
        [Required]
        public string Topic { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string TimeZone { get; set; }

        [Required]
        public string Password { get; set; }
        public bool? RecordingReq { get; set; }
        public bool? SingleAccess { get; set; }

        public bool? AllDay { get; set; }

        public bool? AutoLobby { get; set; }
        

        [Required]
        public sbyte Status { get; set; }

        public string MeetingId { get; set; } = String.Empty;
        public string MeetingLink { get; set; } // the returned meeting link from Cisco // stored in meeting_log field


        // public int? EventId { get; set; }

        // [Required]  TODO 
        // public List<int> Participants { get; set; }

        public MeetingPostDto()
        {
            // Participants = new List<int>();
        }
        public Meeting GetEntity()
        {
            Meeting meeting = new()
            {
                Topic = Topic,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                TimeZone = TimeZone,
                Password = Password,
                Status = Status,
                RecordingReq = RecordingReq,
                SingleAccess = SingleAccess,
                AutoLobby = AutoLobby,
                MeetingLog=MeetingLink
                //  EventId = EventId
            };

            return meeting;
        }
    }
}
