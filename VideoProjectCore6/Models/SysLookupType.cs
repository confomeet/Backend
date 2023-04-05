using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class SysLookupType
    {
        public SysLookupType()
        {
            SysLookupValues = new HashSet<SysLookupValue>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual ICollection<SysLookupValue> SysLookupValues { get; set; }
    }
}
