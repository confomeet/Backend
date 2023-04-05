//using FluentValidation;
using VideoProjectCore6.Models;

#nullable disable
namespace VideoProjectCore6.DTOs.UserDTO
{
    public class RegisterDTO
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        //  public List<RoleDTO> Roles { get; set; }

        //[Required]
        public string ReCaptchaToken { get; set; }

        public List<int?> Roles { get; set; } 


        public User GetEntity()
        {
            User user = new()
            {
                FullName = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber
                
            };

            return user;
        }
        public static RegisterDTO GetDTO(User user)
        {
            RegisterDTO registerDTO = new()
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return registerDTO;
        }
    }
    //public class RegisterValidator : AbstractValidator<RegisterDTO>
    //{
    //    const string passwordErrorMessage = "must contain:" +
    //                       " a minimum of 1 lower case letter [a - z] and," +
    //                       " a minimum of 1 upper case letter [A - Z] and," +
    //                       " a minimum of 1 numeric character [0 - 9] and," +
    //                       " a minimum of 1 special character, " +
    //                       "And it must be at least 8 characters in length.";
    //    public RegisterValidator(IStringLocalizer localizer)
    //    {
    //        RuleFor(x => x.UserName).NotEmpty().WithName(x => localizer["UserName"]).Matches(@"^\w*$").WithMessage(x => localizer["User name is invalid, it can only contain letters or digits."]);
    //        RuleFor(x => x.Email).NotEmpty().WithName(x => localizer["Email"]).EmailAddress().WithName(x => localizer["UserName"]);
    //        RuleFor(x => x.Password).NotEmpty().WithName(x => localizer["Password"]).MinimumLength(8).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w]).{8,}$").WithMessage(x => localizer["Password"] + ": " + localizer[passwordErrorMessage]);
    //        RuleFor(x => x.ConfirmPassword).Equal(u => u.Password).NotEmpty().WithName(x => localizer["ConfirmPassword"]);
    //    }
    //}
}
