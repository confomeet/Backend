using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserResultDto
    {
        // public string StatusCode { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
    }

    public class CreateUserOldResultDto
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public OldUserPostDto User { get; set; }
    }

    public class OnLineEmployee
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LastStartWork { get; set; }
        public DateTime StartWork { get; set; }
        public int Minutes { get; set; }
        public object LogToday { get; set; }
    }
}
