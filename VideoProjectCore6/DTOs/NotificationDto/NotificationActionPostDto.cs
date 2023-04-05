#nullable disable
using System.ComponentModel.DataAnnotations;


namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationActionPostDto
    {
        [Required]
        public List<int> ActionListId { get; set; }
       
    }
}
