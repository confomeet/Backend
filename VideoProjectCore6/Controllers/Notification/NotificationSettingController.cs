using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;

namespace VideoProjectCore6.Controllers.Notification
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationSettingController : ControllerBase
    {
        private readonly INotificationSettingRepository _iNotificationSettingRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IUserRepository _IUserRepository;

        public NotificationSettingController(INotificationSettingRepository iNotificationRepository, IGeneralRepository iGeneralRepository, IUserRepository iUserRepository)
        {
            _iNotificationSettingRepository = iNotificationRepository;
            _iGeneralRepository = iGeneralRepository;
            _IUserRepository = iUserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.AdminPolicy*/)]
        [HttpPost("AddNotificationTemplate")]
        public async Task<ActionResult> AddNotificationTemplate([FromBody] NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationTemplateWithDetails(notificationTemplatePostDto, _IUserRepository.GetUserID(), lang);

            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Translation.getMessage(lang, "zeroResult"));

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationActions")]
        public async Task<ActionResult> AddNotificationActions([FromBody] NotificationActionPostDto notificationActionPostDto, int notificationId, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationAction(notificationActionPostDto, notificationId);

            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Translation.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationsToOneAction")]
        public async Task<ActionResult> AddNotificationsToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationTemplatesToOneAction(notificationTemplatesAction);

            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Translation.getMessage(lang, "failedAdd"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{templateId}")]
        public async Task<ActionResult> DeleteTemplateId(int templateId, [FromHeader] string lang)
        {

            var result = await _iNotificationSettingRepository.DeleteNotificationTemplate(templateId);
            if (result == true)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Translation.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.AdminPolicy*/)]
        [HttpGet("{notificationId}")]
        public async Task<ActionResult> GetNotificationTemplateDetails(int notificationId)
        {
            var result = await _iNotificationSettingRepository.GetAllNotificationDetails(notificationId);

            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.AdminPolicy*/)]
        [HttpGet()]
        public async Task<ActionResult> GetAllNotificationTemplates()
        {
            var result = await _iNotificationSettingRepository.GetAllNotificationTemplates();
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.AdminPolicy*/)]
        [HttpPut("{notificationTemplateId}")]
        public async Task<ActionResult> Put(NotificationTemplateWithDetailsPostDto notificationTemplateWithDetailsPostDto, int notificationTemplateId)
        {
            var result = await _iNotificationSettingRepository.EditNotificationTemplateDetials(notificationTemplateWithDetailsPostDto, notificationTemplateId);

            if (result != 0)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Constants.AdminPolicy*/)]
        [HttpGet("parameterList")]
        public ActionResult GetparameterList()
        {
            var result = _iNotificationSettingRepository.GetParameterList();

            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetNotificationsDetailsForAction")]
        public async Task<ActionResult> GetNotificationsForAction(int actionId)
        {
            var result = await _iNotificationSettingRepository.GetNotificationsForAction(actionId);

            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
