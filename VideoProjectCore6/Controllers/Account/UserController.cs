﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.UserDTO;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.Services;
using VideoProjectCore6.DTOs.FileDto;
#nullable disable
namespace VideoProjectCore6.Controllers.Account
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _IUserRepository;
        // private readonly IStringLocalizer<UserController> _localizer;
        //  private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public UserController(IWebHostEnvironment iWebHostInviroment,
                              /// IStringLocalizer<UserController> localizer,
                              IUserRepository iUserRepository /*,IOptions<SignInWithUGateSettings> signInWithUGateSettings*/)
        {
            _IUserRepository = iUserRepository;
            //  _SignInWithUGateSettings = signInWithUGateSettings.Value;
            _IWebHostEnvironment = iWebHostInviroment;
            //_localizer = localizer;
        }



        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(LogInDto logInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.LocalSignIn(logInDto, lang);
            return obj.StatusCode.Id > 0 ? Ok(obj) : BadRequest(obj);
        }

        //[HttpPost("SignOut")]
        //public IActionResult SignOut(bool WithUnifiedgate)
        //{
        //    _IUserRepository.SignOut();
        //    if (WithUnifiedgate)
        //        return Redirect(_SignInWithUGateSettings.signOut);

        //    else return Ok("User SignOut !!!");
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpGet("GetUserRoles")]
        //public async Task<IActionResult> GetUserRoles([FromHeader] string lang)
        //{
        //    var obj = await _IUserRepository.GetUserRoles(_IUserRepository.GetUserID(), lang);
        //    return Ok(obj);
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpGet("GetUsersRoles")]
        //public async Task<IActionResult> GetUsersRoles(string blindSearch, [FromHeader] string lang)
        //{
        //    var obj = await _IUserRepository.GetUsersRoles(blindSearch, lang);
        //    return Ok(obj);
        //}


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPost("EditUserRolesAsync")]
        //public async Task<ActionResult> EditUserRolesAsync(int userId, List<int> userRoles, [FromHeader] string lang)
        //{
        //    var result = await _IUserRepository.EditUserRolesAsync(userId, userRoles, lang);
        //    if (result.Succeeded)
        //    {
        //        return StatusCode(StatusCodes.Status200OK, result);
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        //    }
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordObject, [FromHeader] string lang)
        //{
        //    var obj = await _IUserRepository.ResetPasswordByToken(resetPasswordObject, lang);
        //    return Ok(obj);
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPost("GetUserId")]
        //public ActionResult GetUserId()
        //{
        //    return Ok(_IUserRepository.GetUserID().ToString());
        //}


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPost("IsAdmin")]
        //public ActionResult IsAdmin()
        //{
        //    return Ok(_IUserRepository.IsAdmin());
        //}

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPost("IsEmployee")]
        //public ActionResult IsEmployee()
        //{
        //    return Ok(_IUserRepository.IsEmployee(_IUserRepository.GetUserID()));
        //}

        //// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [TypeFilter(typeof(KeyAttribute))]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] UserPostDto UserPostDto, [FromHeader] string lang)
        {
            string ImageUrl = null; //"wwwroot/transactions/UsersPhoto/" + SaveImage(UserPostDto.ImageFile);
            APIResult result = await _IUserRepository.CreateUser(UserPostDto, ImageUrl, false, "ar", true);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddeUser([FromBody] UserPostDto UserPostDto, [FromHeader] string lang)
        {
            string ImageUrl = null; //"wwwroot/transactions/UsersPhoto/" + SaveImage(UserPostDto.ImageFile);
            APIResult result = await _IUserRepository.CreateUser(UserPostDto, ImageUrl, false, "ar", true);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("Check")]
        public async Task<IActionResult> CheckUserIfExist([FromQuery] int userId, [FromQuery] int userType)
        {
            var result = await _IUserRepository.GetLocalUserId(userId, userType);
            return result.Id > 0 ? Ok(true) : Ok(false);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("EditPassword")]
        public async Task<IActionResult> EditPassword(EditUserPasswordDTO editUserPasswordDTO, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditPassword(editUserPasswordDTO, lang);
            return Ok(obj);
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, [FromHeader] string lang)
        {
            //TODO
            //RegisterValidator validator = new(_localizer);

            var result = await _IUserRepository.RegisterAsync(registerDTO, lang, true);
            return result.Id > 0 ? Ok(result) : BadRequest(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AdminRegisterNewUser")]
        public async Task<IActionResult> AdminRegisterNewUser(RegisterDTO registerDTO, [FromHeader] string lang)
        {
            //TODO
            //RegisterValidator validator = new(_localizer);

            var result = await _IUserRepository.RegisterAsync(registerDTO, lang, true, true);
            return result.Id > 0 ? Ok(result) : BadRequest(result);

        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email, [FromHeader] string lang)
        {
            return Ok(await _IUserRepository.ConfirmEmail(token, email, lang));
        }


        [HttpGet("ResendActivation")]
        public async Task<IActionResult> ResendActivation(string email, [FromHeader] string lang)
        {
            return Ok(await _IUserRepository.ResendActivation(email, lang));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordObject, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.ResetPasswordByToken(resetPasswordObject, lang);
            return Ok(obj);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendResetPasswordEmail(string email, [FromHeader] string lang)
        {
            MultiLangMessage multiLangMessage = new MultiLangMessage
            {
                En = "Please follow the link to reset your password",
                Ar = "الرجاء الضغط على الرابط لإعادة تعيين كلمة المرور"
            };

            var obj = await _IUserRepository.SendResetPasswordEmail(multiLangMessage, email, lang);
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<IActionResult> AllUser([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "ar")
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchUser([FromQuery] string email, [FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "ar")
        {
            try
            {
                var result = await _IUserRepository.Search(email, pageIndex, pageSize, lang);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error getting users");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SearchFilterUser")]
        public async Task<IActionResult> SearchFilterUser([FromBody] UserFilterDto userFilterDto, [FromHeader] string lang = "ar")
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("FilterUsers")]
        public async Task<IActionResult> SearchFilterUser([FromQuery] string text, [FromQuery] string name, [FromQuery] string email,
            [FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "ar")
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("{id}/UnLock")]
        public async Task<IActionResult> UnLockAccount([FromRoute] int id, [FromHeader] string lang = "ar")
        {
            var result = await _IUserRepository.UnLockAccount(id, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("{id}/Lock")]
        public async Task<IActionResult> LockAccount([FromRoute] int id, [FromHeader] string lang = "ar")
        {
            var result = await _IUserRepository.LockAccount(id, null, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("{id}/Activate")]
        public async Task<IActionResult> ActivateAccount([FromRoute] int id, [FromHeader] string lang = "ar")
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.EmployeePolicy*/)]
        [HttpPost("EditProfile/{id}")]
        public async Task<IActionResult> EditUser([FromRoute] int id, [FromBody] UserPostDto userPostDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditProfile(id, userPostDto, false, false, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.EmployeePolicy*/)]
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditUserWithRole([FromRoute] int id, [FromBody] UserView dto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditUser(id, _IUserRepository.GetUserID(), dto, false, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RelatedUsers")]
        public async Task<IActionResult> GetRelatedUsers([FromHeader] string lang = "ar")
        {
            return Ok(await _IUserRepository.GetRelatedUsers(_IUserRepository.GetUserID(), lang));
        }

        [HttpPost("VerifyUserCredentials")]
        public async Task<IActionResult> VerifyUserCredentials(LogInDto logInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.VerifyUserCredentials(logInDto, lang);
            if (obj.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(OtpLogInDto otpLogInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.VerifyOTP(otpLogInDto, lang);
            if (obj.StatusCode.Id < 0)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var obj = await _IUserRepository.RefreshToken();
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile([FromHeader] string lang = "ar")
        {
            var obj = await _IUserRepository.ViewMyProfile(_IUserRepository.GetUserID(), lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.EmployeePolicy*/)]
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
    }
}