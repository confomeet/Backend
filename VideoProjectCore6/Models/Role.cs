using Microsoft.AspNetCore.Identity;
#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class Role:IdentityRole<int>
    {
        public Role()
        {
            RoleClaims = new HashSet<RoleClaim>();
            Users = new HashSet<User>();
        }
        public Role(string roleName) : base(roleName)
        {

        }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte? RecStatus { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Permission> RolePermissions { get; set; }
    }
}
