
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserPostDto
    {
        public virtual string PhoneNumber { get; set; }
        public virtual string PasswordHash { get; set; }

        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Gender { get; set; }
        public string TelNo { get; set; }
        public string Address { get; set; }
        //public IFormFile ImageFile { get; set; }
        public List<int> UserRoles { get; set; }
        public int SecurityQuestionId { get; set; }
        public int NatId { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public int Status { get; set; }
        public string EmailLang { get; set; }
        public string SmsLang { get; set; }

        public int AreaId { get; set; }
        public int NotificationType { get; set; }
        public int ProfileStatus { get; set; }
        public uint? UserId { get; set; }
        public int? EntityId { get; set; }

        public byte? RecStatus { get; set; }
}

    public class SignatureBase64PostDto
    {
        public string SignatureBase64 { get; set; }
    }

    public class SignaturePostDto
    {
        public IFormFile SignatureFile { get; set; }
    }

    public class OldUserPostDto
    {
        public virtual string PhoneNumber { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Gender { get; set; }
        public string TelNo { get; set; }
        public string Address { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<int> UserRoles { get; set; }
        public int SecurityQuestionId { get; set; }
        public int NatId { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public int Status { get; set; }
        public string EmailLang { get; set; }
        public string SmsLang { get; set; }
        public int AreaId { get; set; }
        public int NotificationType { get; set; }
        public int ProfileStatus { get; set; }
    }


    public class EditUserPasswordDTO
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
