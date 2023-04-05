using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class Tab
    {
        public Tab()
        {
            InverseParent = new HashSet<Tab>();
        }

        public int Id { get; set; }
        public string TabNameShortcut { get; set; } = null!;
        public string? Link { get; set; }
        public int? ParentId { get; set; }
        public byte TabOrder { get; set; }
        public byte[]? Icon { get; set; }
        public string? IconString { get; set; }

        public virtual Tab? Parent { get; set; }
        public virtual ICollection<Tab> InverseParent { get; set; }
    }
}
