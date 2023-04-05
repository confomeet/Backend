using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class UserLogger
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoggingDate { get; set; }
        public bool? StartWorkForEmployee { get; set; }
    }
}
