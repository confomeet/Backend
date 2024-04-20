using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.UserDTO;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.DTOs.FileDto;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using System.Text;
using VideoProjectCore6.Utility.Authorization;
#nullable disable
namespace VideoProjectCore6.Controllers.Account
{
    [ApiController]
    [Route(ControllerRoute)]
    public class UserController : ControllerBase
    {
        public const string ControllerRoute = "/api/v1/User";
        private readonly IUserRepository _IUserRepository;
        // private readonly IStringLocalizer<UserController> _localizer;
        //  private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ILogger _ILogger;
        public UserController(IWebHostEnvironment iWebHostInviroment,
                              /// IStringLocalizer<UserController> localizer,
                              IUserRepository iUserRepository, /*,IOptions<SignInWithUGateSettings> signInWithUGateSettings*/
                              ILogger<UserController> logger)
        {
            _IUserRepository = iUserRepository;
            //  _SignInWithUGateSettings = signInWithUGateSettings.Value;
            _IWebHostEnvironment = iWebHostInviroment;
            //_localizer = localizer;
            _ILogger = logger;
        }

        [HasPermission(Permissions.User_Delete)]
        [HttpPost("{id}/Delete")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id, [FromHeader] string lang)
        {
            APIResult result = await _IUserRepository.DeleteUser(id, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HasPermission(Permissions.Profile_EditPassword)]
        [HttpPost("EditPassword")]
        public async Task<IActionResult> EditPassword(EditUserPasswordDTO editUserPasswordDTO, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditPassword(editUserPasswordDTO, lang);
            return Ok(obj);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, [FromHeader] string lang)
        {
            //TODO
            //RegisterValidator validator = new(_localizer);

            var result = await _IUserRepository.RegisterAsync(registerDTO, lang, true);
            return result.Id > 0 ? Ok(result) : BadRequest(result);

        }

        [HasPermission(Permissions.User_Create)]
        [HttpPost("AdminRegisterNewUser")]
        public async Task<IActionResult> AdminRegisterNewUser(RegisterDTO registerDTO, [FromHeader] string lang)
        {
            //TODO
            //RegisterValidator validator = new(_localizer);

            var result = await _IUserRepository.RegisterAsync(registerDTO, lang, true, true);
            return result.Id > 0 ? Ok(result) : BadRequest(result);

        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email, [FromHeader] string lang)
        {
            return Ok(await _IUserRepository.ConfirmEmail(token, email, lang));
        }

        [HasPermission(Permissions.User_Update)]
        [HttpGet("ResendActivation")]
        public async Task<IActionResult> ResendActivation(string email, [FromHeader] string lang)
        {
            return Ok(await _IUserRepository.ResendActivation(email, lang));
        }

        [AllowAnonymous]
        [HttpPost]
        #pragma warning disable ASP0023  // See https://github.com/dotnet/aspnetcore/issues/49777
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordObject, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.ResetPasswordByToken(resetPasswordObject, lang);
            return Ok(obj);
        }

        [AllowAnonymous]
        [HttpPost]
        #pragma warning disable ASP0023  // See https://github.com/dotnet/aspnetcore/issues/49777
        [Route("[action]")]
        public async Task<IActionResult> SendResetPasswordEmail(string email, [FromHeader] string lang)
        {
            MultiLangMessage multiLangMessage = new MultiLangMessage
            {
                En = "Please follow the link to reset your password",
                Ru = "Для сброса пароля перейдите по ссылке"
            };

            var obj = await _IUserRepository.SendResetPasswordEmail(multiLangMessage, email, lang);
            return Ok(obj);
        }


        [HasPermission(Permissions.User_Read)]
        [HttpGet]
        public async Task<IActionResult> AllUser([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "en")
        {
            try
            {
                var result = await _IUserRepository.GetUsers(lang, pageIndex, pageSize);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error getting users");
            }
        }

        [HasPermission(Permissions.User_Read)]
        [HttpPost("SearchFilterUser")]
        public async Task<IActionResult> SearchFilterUser([FromBody] UserFilterDto userFilterDto, [FromHeader] string lang = "en")
        {
            try
            {
                var paramter = userFilterDto != null ? userFilterDto : null;

                var result = await _IUserRepository.SearchFilterUsers(paramter, _IUserRepository.GetUserID(), lang);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error getting users");
            }
        }


        // -------------------------------------
        // Temporary endpoint
        [HasPermission(Permissions.User_Read)]
        [HttpGet("FilterUsers")]
        public async Task<IActionResult> SearchFilterUser([FromQuery] string text, [FromQuery] string name, [FromQuery] string email,
            [FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "en")
        {
            try
            {
                var result = await _IUserRepository.SearchFilterUsers(text, name, email, pageIndex, pageSize, lang);
                return Ok(result);
            }

            catch
            {
                return BadRequest("Error getting users");
            }

        }
        // -----------------------------------

        [HasPermission(Permissions.User_Enable)]
        [HttpPost("{id}/UnLock")]
        public async Task<IActionResult> UnLockAccount([FromRoute] int id, [FromHeader] string lang = "en")
        {
            var result = await _IUserRepository.UnLockAccount(id, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HasPermission(Permissions.User_Disable)]
        [HttpPost("{id}/Lock")]
        public async Task<IActionResult> LockAccount([FromRoute] int id, [FromHeader] string lang = "en")
        {
            var result = await _IUserRepository.LockAccount(id, null, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HasPermission(Permissions.User_Enable)]
        [HttpPost("{id}/Activate")]
        public async Task<IActionResult> ActivateAccount([FromRoute] int id, [FromHeader] string lang = "en")
        {
            var result = await _IUserRepository.ActivateAccount(id, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPost("CreateUserTest")]
        //public IActionResult CreateUserTest(IFormFile ImageName)
        //{

        //    var resul = SaveImage(ImageName);
        //    string Image = "wwwroot/transactions/UsersPhoto/" + resul;
        //    //  UserResultDto obj = await _IUserRepository.CreateUser(UserPostDto);
        //    if (resul == null)
        //    {
        //        return NotFound(resul);
        //    }
        //    return Ok(resul);
        //}

        //[NonAction]
        //public string SaveImage(IFormFile imageFile)
        //{
        //    string imageName;
        //    if (imageFile == null) return null;
        //    imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(" ", "_");
        //    imageName = imageName + DateTime.Now.ToString("yyyymmdd") + Path.GetExtension(imageFile.FileName);

        //    var imagePath = Path.Combine(_IWebHostEnvironment.ContentRootPath, "wwwroot/transactions/UsersPhoto", imageName);
        //    using (var fileStream = new FileStream(imagePath, FileMode.Create))
        //    {
        //        imageFile.CopyToAsync(fileStream);
        //    }
        //    return imageName;
        //}

        [HasPermission(Permissions.Profile_Update)]
        [HttpPost("EditProfile/{id}")]
        public async Task<IActionResult> EditUser([FromRoute] int id, [FromBody] UserPostDto userPostDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditProfile(id, userPostDto, false, false, lang);
            if (obj.Code == APIResult.RESPONSE_CODE.PageNotFound)
            {
                return NotFound(obj);
            }
            if (obj.Id < 0)
                return BadRequest(obj);
            return Ok(obj);
        }

        [HasPermission(Permissions.User_Update)]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditUserWithRole([FromRoute] int id, [FromBody] UpdateUserDto dto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditUser(id, dto, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [HasPermission(Permissions.User_Read)]
        [HttpGet("FindUserById/{id}")]
        public async Task<IActionResult> FindUserById(int id, [FromHeader] string lang)
        {
            //UserDto obj = await _IUserRepository.FindUserById(id, lang);
            //if (obj == null)
            //{
            //    return NotFound(obj);
            //}
            return Ok(await _IUserRepository.FindUserById(id, lang));
        }

        [HttpGet("RelatedUsers")]
        public async Task<IActionResult> GetRelatedUsers()
        {
            return await Task.FromResult(Ok(new List<ValueId>()));
        }

        [AllowAnonymous]
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(LogInDto logInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.LogIn(logInDto, lang);
            if (obj.Id < 0)
            {
                return Unauthorized(obj);
            }
            return Ok(obj);
        }

        [AllowAnonymous]
        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(OtpLogInDto otpLogInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.VerifyOTP(otpLogInDto, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [AllowAnonymous]
        [HttpGet("LoginWithToken")]
        public async Task<IActionResult> LoginWithToken([FromQuery] string redirectUrl, [FromQuery] string token) {
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized();
            }
            var signedIn = await _IUserRepository.LogInWithToken(token);
            if (signedIn.Id <= 0) {
                return Unauthorized();
            }
            FillSessionWithUserInfo(signedIn.Result);
            return Redirect(redirectUrl);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromHeader] string lang)
        {
            var obj = await _IUserRepository.RefreshToken(lang);
            return Ok(obj);
        }

        [HasPermission(Permissions.Profile_Read)]
        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile([FromHeader] string lang = "en")
        {
            var currentPath = Request.Path.Value;
            var pathToTokenLogin = currentPath.Replace("MyProfile", "LoginWithToken");
            var obj = await _IUserRepository.ViewMyProfile(_IUserRepository.GetUserID(), pathToTokenLogin, lang);
            return Ok(obj);
        }

        [HasPermission(Permissions.Profile_Update)]
        [HttpPost("EditMyProfilePhoto")]
        public async Task<IActionResult> EditProfilePhoto([FromForm] FilePostDto filePostDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditProfilePhoto(_IUserRepository.GetUserID(), filePostDto, false, false, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        private static readonly JsonSerializerOptions AuthInfoSerializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private void FillSessionWithUserInfo(LogInResultDto logInResult) {
            var authUser = JsonSerializer.Serialize(logInResult, typeof(LogInResultDto), AuthInfoSerializeOptions);
            var b64AuthUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(authUser));
            Response.Cookies.Append("authUser", b64AuthUser, new CookieOptions{
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Secure = true,
            });
        }

    }
}
