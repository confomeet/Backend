using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.Services;

namespace VideoProjectCore6.Models
{
    public partial class ConfEvent
    {
        public int Id { get; set; }

        [Required]
        public DateTime EventTime { get; set; }

        [Required]
        public Constants.EVENT_TYPE EventType { get; set; }

        public string UserId { get; set; } = "string";

        public string EventInfo { get; set; } = string.Empty;

        [Required]
        public string MeetingId { get; set; } = null!;

    }
}
