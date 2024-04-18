using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Utility.Authorization;

namespace VideoProjectCore6.Controllers.Event
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _IEventRepository;
        private readonly IUserRepository _IUserRepository;

        public EventController(IEventRepository iEventRepository, IUserRepository iUserRepository)
        {
            _IEventRepository = iEventRepository;
            _IUserRepository = iUserRepository;
        }

        [HasPermission(Permissions.Meeting_Create)]
        [HttpPost("MeetingEvent")]
        public async Task<ActionResult> AddMeetingEvent([FromBody] EventWParticipant dto, [FromHeader] string lang = "en")
        {
            return Ok(await _IEventRepository.AddMeetingEvent(dto, _IUserRepository.GetUserID(), true, lang));
        }

        [HasPermission(Permissions.Meeting_Search_IfParticipant)]
        [HttpPost("Search")]
        public async Task<ActionResult> SearchLocal([FromBody] EventSearchObject obj, [FromHeader] string lang = "en")
        {
            var result = await _IEventRepository.GetAllOfUser(_IUserRepository.GetUserID(), obj, lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HasPermission(Permissions.Meeting_Update_All)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(MeetingEventDto dto, int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.UpdateEvent(id, _IUserRepository.GetUserID(), dto, null, lang);
            return Ok(result);
        }

        [HasPermission(Permissions.Meeting_Search_All)]
        [HttpPost("All")]
        public async Task<ActionResult> GetAll([FromBody] EventSearchObject obj, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _IEventRepository.GetAll(_IUserRepository.GetUserID(), obj, pageIndex, pageSize);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HasPermission(Permissions.Meeting_FetchDetails_IfParticipant)]
        [HttpGet("{id}/Details")]
        public async Task<ActionResult> EventDetails([FromRoute] int id, [FromHeader] string timeZone, [FromHeader] string lang = "en")
        {
            var result = await _IEventRepository.EventDetails(id, _IUserRepository.GetUserID(), timeZone);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status404NotFound, "Not found");
        }

        [HasPermission(Permissions.Meeting_Create)]
        [HttpPost("AddRecurrence")]
        public async Task<ActionResult> AddREvent([FromBody] RecurrenceEvent events, [FromHeader] string lang = "en")
        {
            var result = await _IEventRepository.AddRecurrenceEvents(events.Event, events.RDates, _IUserRepository.GetUserID(), true, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HasPermission(Permissions.Meeting_Update_All)]
        [HttpPut("Recurrence/{id}")]
        public async Task<ActionResult> Update(EventWUpdatOption dto, int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.UpdateEvent(id, _IUserRepository.GetUserID(), dto.EventDto, dto.Options, lang);
            return Ok(result);
        }

        [HasPermission(Permissions.Meeting_Cancel_All)]
        [HttpPut("{id}/Cancel")]
        public async Task<ActionResult> Cancel([FromRoute] int id,  [FromHeader] string lang)
        {
            var result = await _IEventRepository.Cancel(id, _IUserRepository.GetUserID(), lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }
    }
}