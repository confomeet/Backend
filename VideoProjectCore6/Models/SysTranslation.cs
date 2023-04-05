using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class SysTranslation
    {
        public int Id { get; set; }
        public string Shortcut { get; set; } = null!;
        public string Lang { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
