namespace VideoProjectCore6.DTOs.RoleDto
{
    public class RoleWithPermissionsPostDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> RoleNameShortCut { get; set; } = [];
        public Dictionary<string, List<string>> Permissions { get; set; } = [];

    }
}
