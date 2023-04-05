using VideoProjectCore6.DTOs.TabDto;

namespace VideoProjectCore6.DTOs.RoleDto
{
    public class RolePermissionsGetDTO
    {
        public int RoleId { get; set; }
        public dynamic ActionPermissions { get; set; }
        public dynamic PnsActionPermissions { get; set; }
        public List<UserTabGetDTO> TabPermissions { get; set; }

    }
}
