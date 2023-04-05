
#nullable disable
namespace VideoProjectCore6.DTOs.ConfEventDto
{
    public class ConfEventCompactGet
    {
        public DateTime EventTime { get; set; }

        public string Status { get; set; }

        public string UserName { get; set; }

        public string KickedBy { get; set; }

    }
}
