using System;
using System.Collections.Generic;

namespace VideoProjectCore6.Models
{
    public partial class MeetingLogging
    {
        public int Id { get; set; }
        public int? MeetingId { get; set; }
        public int? UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public bool IsModerator { get; set; }
        public DateTime FirstLogin { get; set; }
        public string? PreviousLoginList { get; set; }

        public virtual Meeting? Meeting { get; set; }
    }
}
