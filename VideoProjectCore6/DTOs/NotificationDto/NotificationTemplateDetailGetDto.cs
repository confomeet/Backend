#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateDetailGetDto
    {     
        public int NotificationChannelId { get; set; }

        public Dictionary<string, string> ChannelShortCutLangValue { get; set; }

        public Dictionary<string, string> TitleShortCutLangValue { get; set; }

        public Dictionary<string, string> BodyShortCutLangValue { get; set; }

        public bool? ChangeAble { get; set; }
    }
}
