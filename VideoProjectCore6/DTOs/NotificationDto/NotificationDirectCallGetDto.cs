
#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationDirectCallGetDto
    {
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public string RecieverId { get; set; }
        public byte? Status { get; set; }

        public int? SenderId { get; set; }

        public string SenderName { get; set; }

    }
}
