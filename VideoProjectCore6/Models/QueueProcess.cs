using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class QueueProcess
    {
        public int Id { get; set; }
        public int ProcessNo { get; set; }
        public int ServiceKindNo { get; set; }
        public DateTime? ExpectedDateTime { get; set; }
        public bool? NotifyLowLevel { get; set; }
        public int TicketId { get; set; }
        public string? Note { get; set; }
        public string? Provider { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }
        public bool? NotifyHighLevel { get; set; }
        public bool? NotifyMediumLevel { get; set; }
        public byte? Status { get; set; }
    }
}
