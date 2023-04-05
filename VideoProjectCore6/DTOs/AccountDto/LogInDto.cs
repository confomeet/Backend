using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.DTOs.CommonDto;
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class LogInDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PassWord { get; set; }
        public UserAgent UA { get; set; }
    }
}
