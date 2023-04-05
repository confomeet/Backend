namespace VideoProjectCore6.DTOs.TabDto
#nullable disable
{
    public class UserTabGetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string IconBase64 { get; set; }
        public string IconString { get; set; }
        public List<UserTabGetDTO> Elements { get; set; }
        public int? ParentId { get; set; }
        public int TabOrder { get; set; }
        public bool HasAccess { get; set; }

    }
}
