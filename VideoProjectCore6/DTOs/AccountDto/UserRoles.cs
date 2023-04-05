using VideoProjectCore6.DTOs.RoleDto;
namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class UserRoles
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<RoleGetDto> RolesName { get; set; }
    }
}
