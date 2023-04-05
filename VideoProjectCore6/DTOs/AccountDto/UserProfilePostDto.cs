
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserProfilePostDto
    {
        public string FullName { get; set; }
        public virtual string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
