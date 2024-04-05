using Microsoft.AspNetCore.Identity;


namespace VideoProjectCore6.Models
{
    public partial class UserClaim:IdentityUserClaim<int>
    {
      //  public int UserId { get; set; }
       // public string ClaimType { get; set; } = null!;
       // public string ClaimValue { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
