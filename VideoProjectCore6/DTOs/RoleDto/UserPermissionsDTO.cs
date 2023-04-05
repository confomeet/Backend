using VideoProjectCore6.Models;

namespace VideoProjectCore6.DTOs.RoleDto
{
    public class UserPermissionsDTO
    {
        public int UserID { get; set; }

        public List<RoleClaim> Permissions { get; set; }

        public UserPermissionsDTO()
        {
            Permissions = new List<RoleClaim>();
        }

    }
}
