#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class NotificationTemplate
    {
        public NotificationTemplate()
        {
            NotificationTemplateDetails = new HashSet<NotificationTemplateDetail>();
            NotificationActions = new HashSet<NotificationAction>();
        }

        public int Id { get; set; }
        public string NotificationNameShortcut { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual ICollection<NotificationTemplateDetail> NotificationTemplateDetails { get; set; }
        public virtual ICollection<NotificationAction> NotificationActions { get; set; }
    }
}
