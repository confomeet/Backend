﻿
#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class ConfEvent
    {
        public int Id { get; set; }
        public DateTime EventTime { get; set; }

        public int? EventType { get; set; }

        public string ConfId { get; set; }

        public string UserId { get; set; } = "string";

        public string EventInfo { get; set; }
        public int MeetingId { get; set; } = 10000;

    }
}
