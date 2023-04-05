using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.Models;


#nullable disable
namespace VideoProjectCore6.DTOs.ContactDto
{
    public class ContactGetDto
    {



        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ContactId { get; set; }
        public string DisplayName { get; set; }
        public string Mobile { get; set; }

        public string Home { get; set; }
        public string Office { get; set; }

        public string Email { get; set; }

        public int? Country { get; set; }

        public string Website { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public string City { get; set; }

        public string JobDesc { get; set; }

        public string Specialization { get; set; }

        public int? DirectManageId { get; set; }

        public int? CompanyId { get; set; }

        public int? SectionId { get; set; }

        public bool? ShareWith { get; set; } = true;

        public virtual ICollection<Contact> Sections { get; set; }

        public virtual ICollection<Contact> Childrens { get; set; }

        public List<int?> SectionIds { get; set; }
        public bool isActive { get; set; }

        public IEnumerable<FileGetDto> File { get; set; }
        //public virtual ICollection<Contact> Individuals { get; set; }
        //public virtual ICollection<Contact> Individuals { get; set; }

    }
}
