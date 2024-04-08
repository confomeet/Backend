using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.CommonDto
{
    public class DateTimeRange
    {
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }
    }
}
