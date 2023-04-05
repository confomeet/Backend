
#nullable disable
namespace VideoProjectCore6.Models
{
    public class Files
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ContactId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public User User { get; set; }
        public Contact Contact { get; set; }
    }
}
