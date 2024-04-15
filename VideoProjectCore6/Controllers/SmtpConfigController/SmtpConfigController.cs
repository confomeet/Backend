


#nullable disable
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VideoProjectCore6.DTOs.SmtpConfigDto;
using VideoProjectCore6.Repositories.ISmtpConfigRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Utility.Authorization;


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

        [HasPermission(Permissions.SmtpConfig_CreateUpdate)]
        [HttpPost("CreateUpdate")]
        public async Task<IActionResult> SmtpConfig([FromBody] SmtpConfigPostDto smtpConfigPostDto, [FromHeader] string lang)
        {
            var obj = await _ISmtpConfigRepository.CreateUpdate(_IUserRepository.GetUserID(), smtpConfigPostDto, lang);
            return Ok(obj);
        }

        [HasPermission(Permissions.SmtpConfig_Read)]
        [HttpGet()]
        public async Task<IActionResult> DisplaySmtpConfig([FromHeader] string lang)
        {
            var obj = await _ISmtpConfigRepository.DisplaySmtpConfig(lang);
            return Ok(obj);
        }

    }


}
