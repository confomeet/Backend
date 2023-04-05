
#nullable disable
namespace VideoProjectCore6.Models
{
    public class RecordingLog
    {
        public int Id { get; set; }

        public string FileSize { get; set; }

        //public int? MeetingId { get; set; }

        //public DateTime RecordingDate { get; set; }

        public string RecordingfileName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string FilePath { get; set; }

        public byte? IsSucceeded { get; set; }
    }
}
