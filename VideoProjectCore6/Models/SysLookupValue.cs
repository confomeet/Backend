using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class SysLookupValue
    {
        public int Id { get; set; }
        public int LookupTypeId { get; set; }
        public string Shortcut { get; set; } = null!;
        public bool? BoolParameter { get; set; }
        public byte? Order { get; set; }
        public string? Specification { get; set; }

        public virtual SysLookupType LookupType { get; set; } = null!;
    }
}
