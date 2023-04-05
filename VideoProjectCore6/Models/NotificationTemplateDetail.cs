namespace VideoProjectCore6.Models
{
    public partial class NotificationTemplateDetail
    {
        public int Id { get; set; }
        public int NotificationTemplateId { get; set; }
        public int NotificationChannelId { get; set; }
        public string TitleShortcut { get; set; } = null!;
        public string BodyShortcut { get; set; } = null!;
        public bool? ChangeAble { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual NotificationTemplate NotificationTemplate { get; set; } = null!;
    }
}
