#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class NotificationAction
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int NotificationTemplateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual Action Action { get; set; } = null!;
        public virtual NotificationTemplate NotificationTemplate { get; set; } = null!;
    }
}
