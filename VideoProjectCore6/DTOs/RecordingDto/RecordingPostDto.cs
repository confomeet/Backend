
#nullable disable
namespace VideoProjectCore6.DTOs.RecordingDto
{
    public class RecordingPostDto
    {

        public string RecordingfileName { get; set; }

        public string FileSize { get; set; }

        public string FilePath { get; set; }

        public byte? Status { get; set; } = 0;

    }
}
