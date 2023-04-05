namespace VideoProjectCore6.DTOs.RoleDto
{
    public class RoleTabGetDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int TabOrder { get; set; }
        public string Name { get; set; }
        public bool HasAccess { get; set; }
    }
}
