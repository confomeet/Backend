#nullable disable
using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.Models;
using VideoProjectCore6.DTOs.FileDto;


namespace VideoProjectCore6.DTOs.ContactDto
{
    public class ContactDto
    {   public int? UserId { get; set; }
        public int? ContactId { get; set; }
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "You must provide a mobile number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Not a valid mobile number")]
        public string Mobile { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Not a valid home number")]
        public string Home { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Not a valid office number")]
        public string Office { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int? Country { get; set; }

        public string Website { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "You must provide a type")]
        public string Type { get; set; }

        public string JobDesc { get; set; }

        public string Specialization { get; set; }

        public int? DirectManageId { get; set; }

        public int? CompanyId { get; set; }

        public int? SectionId { get; set; }

        public List<int?> SectionIds { get; set; }

        public string City { get; set; }
        public bool? ShareWith { get; set; } = true;

        public IEnumerable<FileGetDto> File { get; set; }
        //public virtual ICollection<Contact> Sections { get; set; }
        //public virtual ICollection<Contact> Individuals { get; set; }
        //public virtual ICollection<Contact> Individuals { get; set; }

    }
}
