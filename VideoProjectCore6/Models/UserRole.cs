using Microsoft.AspNetCore.Identity;
#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
