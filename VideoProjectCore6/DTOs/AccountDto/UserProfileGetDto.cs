using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.FileDto;


#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserProfileGetDto
    {
        public string FullName { get; set; }
        public virtual string Email { get; set; }
        public string PhoneNumber { get; set; }

        public List<int> Roles { get; set; }
        public IEnumerable<FileGetDto> File { get; set; }

        public List<ValueId> UserGroups { get; set; }
    }
}
