#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationActionGetDto
    {
        [Required]
        public List<int> ActionListId { get; set; }
       
    }
}
