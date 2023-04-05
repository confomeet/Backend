#nullable disable
using System.ComponentModel.DataAnnotations;


namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplateDetailPostDto
    {     
        [Required]
        public int NotificationChannelId { get; set; }

        [Required]
        public Dictionary<string, string> TitleShortCutLangValue { get; set; }

        [Required]
        public Dictionary<string, string> BodyShortCutLangValue { get; set; }

        [Required]
        public bool ChangeAble { get; set; }
    }
}
