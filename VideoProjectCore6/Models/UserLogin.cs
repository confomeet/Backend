using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class UserLogin:IdentityUserLogin<int>
    {
        public int Id { get; set; }
        public DateTime? LoginDate { get; set; }
        public virtual User User { get; set; } = null!;
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string Device { get; set; }
        public string Os { get; set; }
        public string Ip { get; set; }
    }
}
