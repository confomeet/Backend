using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class Work
    {
        public Work()
        {
            PartyWorks = new HashSet<PartyWork>();
            Specialities = new HashSet<Speciality>();
        }

        public int Id { get; set; }
        public string? Shorcut { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual User? LastUpdatedByNavigation { get; set; }
        public virtual ICollection<PartyWork> PartyWorks { get; set; }
        public virtual ICollection<Speciality> Specialities { get; set; }
    }
}
