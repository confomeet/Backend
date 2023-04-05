#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateDetailsForOneAction
    {        
        public int ActionId { get; set; }
        public List<NotificationTemplateWithDetailsGetDto> NotificationTemplateDetails { get; set; }

    } 
}
