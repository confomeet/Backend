
#nullable disable
using VideoProjectCore6.Models;

namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationLogGetDto
    {
        public int Id { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public int NotificationChannelId { get; set; }
        public string SendReportId { get; set; }
        public byte? IsSent { get; set; }
        public byte? SentCount { get; set; }
        public string ReportValueId { get; set; }
        public int? UserId { get; set; }
        public string ToAddress { get; set; }
        public string Lang { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ApplicationId { get; set; }
        public int? PNSApplicationId { get; set; }
        public bool PNSApplication { get; set; }
        public string NotificationLink { get; set; }
        public string LinkCaption { get; set; }
        public string Template { get; set; }
        
        public string RecipientName { get; set; }

        public string RecipientEmail { get; set; }

        public static NotificationLogGetDto GetDTO(NotificationLog notificationLog)
        {
            NotificationLogGetDto dto = new NotificationLogGetDto
            {
                Id = notificationLog.Id,
                IsSent = notificationLog.IsSent,
                Lang = notificationLog.Lang,
                NotificationBody = notificationLog.NotificationBody,
                NotificationChannelId = notificationLog.NotificationChannelId,
                NotificationTitle = notificationLog.NotificationTitle,
                ReportValueId = notificationLog.ReportValueId,
                SendReportId = notificationLog.SendReportId,
                SentCount = notificationLog.SentCount,
                ToAddress = notificationLog.ToAddress,
                ApplicationId = notificationLog.ApplicationId,
                UserId = notificationLog.UserId,
                CreatedAt = notificationLog.CreatedDate,
                UpdatedAt = notificationLog.LastUpdatedDate,
                LinkCaption = notificationLog.LinkCaption,
                Template = notificationLog.Template,
                NotificationLink = notificationLog.NotificationLink,

    };

            return dto;
        }

    }
}
