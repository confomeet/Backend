using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class PartyWorkSpeciality
    {
        public int Id { get; set; }
        public int PartyWorkId { get; set; }
        public int SpecialityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual PartyWork PartyWork { get; set; } = null!;
        public virtual Speciality Speciality { get; set; } = null!;
    }
}
