#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

namespace VideoProjectCore6.Models
{
    public partial class Contact
    {

        public Contact()
        {

            Individuals = new HashSet<Contact>();
            Sections = new HashSet<Contact>();
            Companies = new HashSet<Contact>();
            UserPhotos = new HashSet<Files>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ContactId { get; set; }
        public string DisplayName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpddatedDate { get; set; }
        public virtual User ContactNavigation { get; set; } = null!;
        public string Mobile { get; set; }
        public string Home { get; set; }
        public string Office { get; set; }
        public string Email { get; set; }


        // --------------------

        public int? Country { get; set; }

        public string Website { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public string JobDesc { get; set; }

        public string Specialization { get; set; }

        public int? DirectManageId { get; set; }

        public int? CompanyId { get; set; }

        public int? SectionId { get; set; }

        public byte? ShareWith { get; set; } = 1;

        public string City { get; set; }

        public string ImageUrl { get; set; }

        public virtual Contact ParentEventNavigation { get; set; }
        public virtual Contact ParentCompanyId { get; set; }
        public virtual Contact ParentSectionId { get; set; }


        //public virtual Contact ChildrenSectionId { get; set; }

        public virtual ICollection<Contact> InverseParentEventNavigation { get; set; }

        public virtual ICollection<Contact> Companies { get; set; }

        public virtual ICollection<Contact> InverseParentSectionId { get; set; }

        public virtual ICollection<Contact> Individuals { get; set; }

        public virtual ICollection<Contact> Sections { get; set; }

        //public virtual ICollection<Contact> Companies { get; set; }

        public virtual ICollection<IndividualsSections> IndividualsSections { get; set; }

        public virtual ICollection<IndividualsSections> IndividualSections { get; set; }

        public virtual ICollection<Files> UserPhotos { get; set; }
        //public virtual ICollection<Contact> InverseChildrenSectionId { get; set; }

    }
}
