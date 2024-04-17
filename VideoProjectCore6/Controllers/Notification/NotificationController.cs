using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.Services;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Controllers.Notification
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationLogRepository _INotificationLogRepository;
        private readonly IUserRepository _IUserRepository;

        public NotificationController(INotificationLogRepository _iNotificationLogRepository, IUserRepository iUserRepository)
        {
            _INotificationLogRepository = _iNotificationLogRepository;
            _IUserRepository = iUserRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Get([FromBody] NotificationFilterDto notificationFilterDto, [FromHeader] string lang = "ar")
        {
            var result = await _INotificationLogRepository.GetNotificationsLog(notificationFilterDto, null, lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Mine")]
        public async Task<ActionResult> MyNotification([FromBody] NotificationFilterDto notificationFilterDto, [FromHeader] string lang = "ar")
        {
            var result = await _INotificationLogRepository.GetNotificationsLog(notificationFilterDto, _IUserRepository.GetUserID(), lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
