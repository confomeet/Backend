
#nullable disable
namespace VideoProjectCore6.Models
{
    public class SmtpConfig
    {

        public int Id { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Encryption { get; set; }

        public string Secure { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedById { get; set; }

        public int? UpdatedById { get; set; }

        public User User { get; set; }
    }
}
