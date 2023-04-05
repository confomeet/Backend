
#nullable disable
namespace VideoProjectCore6.Models
{
    public class Group
    {

        public Group()
        {
            UserGroups = new HashSet<UserGroup>();

            AclGroups = new HashSet<AclGroups>();
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<AclGroups> AclGroups { get; set; }
    }
}
