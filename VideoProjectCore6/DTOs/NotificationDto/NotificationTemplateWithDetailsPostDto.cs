#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateWithDetailsPostDto
    {        
        [Required]
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }

        public List<NotificationTemplateDetailPostDto> NotificationTemplateDetails { get; set; }

    }
}
