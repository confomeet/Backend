using System;
using System.Collections.Generic;
#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class Meeting
    {
        public Meeting()
        {
          MeetingLoggings = new HashSet<MeetingLogging>();
          Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string Topic { get; set; } = null!;
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; } = null!;
        public string Password { get; set; } 
        public string MeetingId { get; set; } = null!;
        public sbyte Status { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }
        public string MeetingLog { get; set; }
        public bool? RecordingReq { get; set; }
        public bool? SingleAccess { get; set; }
        public bool? AutoLobby { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<MeetingLogging> MeetingLoggings { get; set; }
    }
}
