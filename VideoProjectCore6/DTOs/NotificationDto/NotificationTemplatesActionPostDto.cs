#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationTemplatesActionPostDto
    {
        [Required]
        public int ActionId { get; set; }

        [Required]
        public List<int> NotificationTemplateIds { get; set; }
       
    }
}
