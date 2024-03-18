
#nullable disable
using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserGroupDtoView
    {
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual int Id { get; set; }
        public string FullName { get; set; }
        public bool Locked { get; set; }
        public bool Confirmed { get; set; }
        public List<int> Roles { get; set; }
        public List<ValueId> UserGroups { get; set; }
        public bool? Enable2FA { get; set; }
    }
}
