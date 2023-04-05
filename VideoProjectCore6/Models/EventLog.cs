namespace VideoProjectCore6.Models
{

    public partial class EventLog
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public short ActionId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int RelatedId { get; set; }
        public string ObjectType { get; set; } = null!;
    }

}
