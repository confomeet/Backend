
#nullable disable

using VideoProjectCore6.DTOs.RoleDto;

namespace VideoProjectCore6.DTOs.AccountDto
{
    public class AuthenticateExternalSysDto
    {

        
        public string Token { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; }

        public List<RoleGetDto> RolesName { get; set; }

    }
}
