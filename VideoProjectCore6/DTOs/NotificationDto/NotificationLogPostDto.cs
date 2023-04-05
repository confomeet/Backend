using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationLogPostDto
    {
        public int Id { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public string NotificationLink { get; set; }
        public string LinkCaption { get; set; }
        public string Template { get; set; }
        public int NotificationChannelId { get; set; }
        public string SendReportId { get; set; }
        public byte? IsSent { get; set; }
        public byte SentCount { get; set; }
        public string ReportValueId { get; set; }
        public int? UserId { get; set; }
        public string ToAddress { get; set; }
        public string Lang { get; set; }
        public string HostSetting { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string MeetingId { get; set; }
        public int? EventId { get; set; }

        public byte? RecStatus { get; set; }

        public int? CreatedBy { get; set; }

        public NotificationLogPostDto()
        {
            CreatedDate = DateTime.Now;
        }
        public NotificationLog GetEntity()
        {
            NotificationLog notifyLog = new NotificationLog
            {
                Id = Id,
                NotificationTitle = NotificationTitle,
                NotificationBody = NotificationBody,
                NotificationLink = NotificationLink,
                LinkCaption = LinkCaption,
                Template = Template,
                IsSent = IsSent,
                Lang = Lang,
                NotificationChannelId = NotificationChannelId,
                ReportValueId = ReportValueId,
                SendReportId = SendReportId,
                SentCount = SentCount,
                ToAddress = ToAddress,
                UserId = UserId,
                Hostsetting = HostSetting,
                CreatedDate = CreatedDate,
                LastUpdatedDate = UpdatedDate,
                ApplicationId = EventId,//Int32.Parse(MeetingId)  // TODO
                RecStatus = RecStatus,
                CreatedBy = CreatedBy,
            };

            return notifyLog;
        }

        static public NotificationLogPostDto GetDto(NotificationLog notifyLog)
        {
            NotificationLogPostDto dto = new NotificationLogPostDto
            {
                Id = notifyLog.Id,
                HostSetting = notifyLog.Hostsetting,
                IsSent = notifyLog.IsSent,
                Lang = notifyLog.Lang,
                NotificationBody = notifyLog.NotificationBody,
                NotificationLink = notifyLog.NotificationLink,
                LinkCaption = notifyLog.LinkCaption,
                Template = notifyLog.Template,
                NotificationChannelId = notifyLog.NotificationChannelId,
                NotificationTitle = notifyLog.NotificationTitle,
                ReportValueId = notifyLog.ReportValueId,
                SendReportId = notifyLog.SendReportId,
                SentCount = (byte)notifyLog.SentCount,
                ToAddress = notifyLog.ToAddress,
                UserId = notifyLog.UserId,
                CreatedDate = notifyLog.CreatedDate,
                UpdatedDate = notifyLog.LastUpdatedDate,
                EventId= notifyLog.ApplicationId ,// TODO,

            };
            return dto;

        }


        public NotificationLogPostDto ShallowCopy()
        {
            NotificationLogPostDto other = (NotificationLogPostDto)this.MemberwiseClone();
            return other;
        }
        public MultiLangPostNotification ToMultiLangue(string bodyL,string template,string title,string caption)
        {
            return new MultiLangPostNotification
            {
                NotificationTitle= title,
                LinkCaption = caption,
                Lang = string.Empty,
                EventId = EventId,
                NotificationBody = NotificationBody,
                NotificationBodyL = bodyL,
                NotificationLink = NotificationLink,
                Template = !string.IsNullOrWhiteSpace(template)?template:template,
                NotificationChannelId = NotificationChannelId,
                IsSent = IsSent,
                ToAddress = ToAddress,
                UserId = UserId 
            };
        }
    }
}
