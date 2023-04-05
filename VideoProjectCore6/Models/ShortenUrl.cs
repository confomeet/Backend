using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class ShortenUrl
    {
        public Guid GuidUrl { get; set; }
        public string Url { get; set; } = null!;
    }
}
