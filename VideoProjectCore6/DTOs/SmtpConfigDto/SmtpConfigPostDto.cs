
#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SmtpConfigDto
{
    public class SmtpConfigPostDto
    {

        [Required]
        public string DisplayName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }
    }
}
