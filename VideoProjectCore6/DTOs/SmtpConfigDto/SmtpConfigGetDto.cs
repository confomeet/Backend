
#nullable disable
namespace VideoProjectCore6.DTOs.SmtpConfigDto
{
    public class SmtpConfigGetDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        //public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}
