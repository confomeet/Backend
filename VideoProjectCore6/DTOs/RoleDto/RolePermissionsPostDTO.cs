namespace VideoProjectCore6.DTOs.RoleDto
{
    public class RolePermissionsPostDTO
    {
        public int RoleID { get; set; }

        public List<int> ActionPermissions { get; set; } = [];
        public List<int> PnsActionPermissions { get; set; } = [];
        public List<int> TabPermissions { get; set; } = [];

    }
}
