

#nullable disable
namespace VideoProjectCore6.Models
{
    public class AclGroups
    {
        public int Id { get; set; }
        public int AclId { get; set; }
        public int UserGroupId { get; set; }
        public ACL ACL { get; set; }
        public Group Group { get; set; }
    }
}
