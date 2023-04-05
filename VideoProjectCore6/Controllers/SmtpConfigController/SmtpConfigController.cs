


#nullable disable
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VideoProjectCore6.DTOs.SmtpConfigDto;
using VideoProjectCore6.Repositories.ISmtpConfigRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;


namespace VideoProjectCore6.Controllers.SmtpConfigController
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SmtpConfigController : ControllerBase
    {
        private readonly ISmtpConfigRepository _ISmtpConfigRepository;
        private readonly IUserRepository _IUserRepository;

        public SmtpConfigController(IWebHostEnvironment iWebHostInviroment,
                              /// IStringLocalizer<UserController> localizer,
                              ISmtpConfigRepository iSmtpConfigRepository,
                              IUserRepository iUserRepository)
        {
            _ISmtpConfigRepository = iSmtpConfigRepository;
            _IUserRepository = iUserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("CreateUpdate")]
        public async Task<IActionResult> SmtpConfig([FromBody] SmtpConfigPostDto smtpConfigPostDto, [FromHeader] string lang)
        {
            var obj = await _ISmtpConfigRepository.CreateUpdate(_IUserRepository.GetUserID(), smtpConfigPostDto, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet()]
        public async Task<IActionResult> DisplaySmtpConfig([FromHeader] string lang)
        {
            var obj = await _ISmtpConfigRepository.DisplaySmtpConfig(lang);
            return Ok(obj);
        }

    }


}
