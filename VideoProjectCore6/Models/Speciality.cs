using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class Speciality
    {
        public Speciality()
        {
            PartyWorkSpecialities = new HashSet<PartyWorkSpeciality>();
        }

        public int Id { get; set; }
        public string Shortcut { get; set; } = null!;
        public int WorkId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual User? LastUpdatedByNavigation { get; set; }
        public virtual Work Work { get; set; } = null!;
        public virtual ICollection<PartyWorkSpeciality> PartyWorkSpecialities { get; set; }
    }
}
