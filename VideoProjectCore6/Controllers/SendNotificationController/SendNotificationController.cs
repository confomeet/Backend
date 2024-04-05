using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Repositories.INotificationRepository;
#nullable disable
namespace VideoProjectCore6.Controllers.SendNotificationController
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SendNotificationController: ControllerBase
    {
        private readonly INotificationLogRepository _INotificationLogRepository;
        private readonly ISendNotificationRepository _ISendNotificationRepository;

        public SendNotificationController(INotificationLogRepository _iNotificationLogRepository, ISendNotificationRepository  iSendNotificationRepository)
        {
            _INotificationLogRepository = _iNotificationLogRepository;
            _ISendNotificationRepository = iSendNotificationRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SendOTP")]
        public async Task<ActionResult> SendOTPNotification([FromBody] Receiver user,[FromQuery]int eventId, [FromHeader] string lang = "ar")
        {
            var result = await _ISendNotificationRepository.SendOTP((int)user.Id, user.Mobile, user.Email,  eventId,  lang);
            return result ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
