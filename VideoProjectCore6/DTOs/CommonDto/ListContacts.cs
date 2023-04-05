using VideoProjectCore6.DTOs.ContactDto;

namespace VideoProjectCore6.DTOs.CommonDto
#nullable disable
{
    public class ListContacts
    {
        public List<ContactGetDto> Items { get; set; }
        public int? CompanyId { get; set; }
        public bool isAdmin { get; set; }
    }
}
