using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class FileConfiguration
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public int MaxSize { get; set; }
        public int MinSize { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public byte? RecStatus { get; set; }
    }
}
