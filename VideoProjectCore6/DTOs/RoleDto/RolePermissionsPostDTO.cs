namespace VideoProjectCore6.DTOs.RoleDto
{
    public class RolePermissionsPostDTO
    {
        public int RoleID { get; set; }

        public List<Int32> ActionPermissions { get; set; }
        public List<Int32> PnsActionPermissions { get; set; }
        public List<Int32> TabPermissions { get; set; }

    }
}
