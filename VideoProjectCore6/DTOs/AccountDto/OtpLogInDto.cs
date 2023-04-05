using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.DTOs.CommonDto;


#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class OtpLogInDto
    {
        [Required]
        public int userId { get; set; }

        [Required]
        public string Number { get; set; }

        public UserAgent UA { get; set; }
    }
}
