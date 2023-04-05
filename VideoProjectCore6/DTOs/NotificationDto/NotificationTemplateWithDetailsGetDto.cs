#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateWithDetailsGetDto
    {        
        public int NotificationTemplateId { get; set; }
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }
        public List<NotificationTemplateDetailGetDto> NotificationTemplateDetails { get; set; }

        public NotificationTemplateWithDetailsGetDto()
        {
            NotificationTemplateShortCutLangValue = new Dictionary<string, string>();
            NotificationTemplateDetails = new List<NotificationTemplateDetailGetDto>();
        }
    } 
}
