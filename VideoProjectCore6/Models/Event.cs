#nullable disable
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoProjectCore6.Models
{
    public partial class Event
    {
        public Event()
        {
            Participants = new HashSet<Participant>();
            InverseParentEventNavigation = new HashSet<Event>();
        }
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public short? Type { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string Organizer { get; set; }
        public DateTime EndDate { get; set; }
        public string MeetingId { get; set; }
        public DateTime CreatedDate { get; set; }      
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }      
        public int? LastUpdatedBy { get; set; }
        public sbyte? RecStatus { get; set; } //  SBYTE TO HOLD -1 STATUS
        public int? ParentEvent { get; set; }
        public string TimeZone { get; set; }
        public bool? AllDay { get; set; }
        public ushort? AppId { get; set; } 

        public virtual Meeting Meeting { get; set; }
        public virtual Event ParentEventNavigation { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Participant> Participants { get; set; }
        [NotMapped]
        public virtual ICollection<Event> InverseParentEventNavigation { get; set; }
    }
}
