namespace VideoProjectCore6.Models
{
    public partial class Action
    {
        public Action()
        {
            NotificationActions = new HashSet<NotificationAction>();
        }

        public int Id { get; set; }
        public string Shortcut { get; set; } = null!;
        public int? ActionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }

        public virtual ICollection<NotificationAction> NotificationActions { get; set; }
    }
}
