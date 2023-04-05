using Microsoft.AspNetCore.Identity;


namespace VideoProjectCore6.Models
{
    public partial class RoleClaim: IdentityRoleClaim<int>
    {
        public virtual Role Role { get; set; } = null!;
    }
}
