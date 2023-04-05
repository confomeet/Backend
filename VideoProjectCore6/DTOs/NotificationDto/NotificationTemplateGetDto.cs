#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateGetDto
    {        
        public int NotificationTemplateId { get; set; }
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }

        public NotificationTemplateGetDto()
        {
        }
    } 
}
