using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class UserToken: IdentityUserToken<int>
    {
        public int Id { get; set; }
       // public int UserId { get; set; }
       // public string LoginProvider { get; set; } = null!;
      // public string Name { get; set; } = null!;
      // public string Value { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
