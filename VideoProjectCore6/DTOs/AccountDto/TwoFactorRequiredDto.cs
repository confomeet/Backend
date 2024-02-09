using System.ComponentModel.DataAnnotations;
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class TwoFactorRequiredDto
    {
        [Required]
        public int userId { get; set; }
    }
}
