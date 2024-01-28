
#nullable disable
namespace VideoProjectCore6.Models
{
    public enum RecordingStatus : int
    {
        Recording = 0,
        Recorded = 1,
        Uploaded = 2,
        UploadingFailed = 100
    }

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

        public string VideoPublicLink { get; set; }

        public RecordingStatus Status { get; set; }
    }
}
