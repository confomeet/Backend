

#nullable disable
namespace VideoProjectCore6.Models
{
    public class ACL
    {
        public ACL()
        {
            AclGroups = new HashSet<AclGroups>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int CreatedById { get; set; }

        public int UpdatedById { get; set; }

        public virtual ICollection<AclGroups> AclGroups { get; set; }
    }
}