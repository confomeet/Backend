#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class NotificationLog
    {
        public int Id { get; set; }
        public string NotificationTitle { get; set; } = null!;
        public string NotificationBody { get; set; } = null!;
        public string NotificationLink { get; set; } = null!;
        public string LinkCaption { get; set; }
        public string SendReportId { get; set; }
        public byte? IsSent { get; set; }
        public byte? SentCount { get; set; }
        public string ReportValueId { get; set; }
        public string Template { get; set; } = null!;
        public int? UserId { get; set; }
        public string ToAddress { get; set; } = null!;
        public string Lang { get; set; } = null!;
        public string Hostsetting { get; set; }
        public int? ApplicationId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public byte? RecStatus { get; set; }
        public int NotificationChannelId { get; set; }
    }
}
