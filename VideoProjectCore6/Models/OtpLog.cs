using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class OtpLog
    {
        public int Id { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int UserId { get; set; }
        public string? OtpCode { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
