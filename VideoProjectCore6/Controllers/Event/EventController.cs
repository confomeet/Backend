using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("MeetingEvent")]
        public async Task<ActionResult> AddMeetingEvent([FromBody] EventWParticipant dto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IEventRepository.AddMeetingEvent(dto, _IUserRepository.GetUserID(), true, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Search")]
        public async Task<ActionResult> SearchLocal([FromBody] EventSearchObject obj, [FromQuery] bool relatedUserEvents, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.GetAllOfUser(_IUserRepository.GetUserID(), obj, relatedUserEvents, lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(MeetingEventDto dto, int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.UpdateEvent(id, _IUserRepository.GetUserID(), dto, null, lang);
            return Ok(result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.DeleteEvent(id);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Translation.getMessage(lang, "zeroResult"));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/Details")]
        public async Task<ActionResult> EventDetails([FromRoute] int id, [FromHeader] string timeZone, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.EventDetails(id, _IUserRepository.GetUserID(), timeZone);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status404NotFound, "Not found");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddRecurrence")]
        public async Task<ActionResult> AddREvent([FromBody] RecurrenceEvent events, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.AddRecurrenceEvents(events.Event, events.RDates, _IUserRepository.GetUserID(), true, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("Recurrence/{id}")]
        public async Task<ActionResult> Update(EventWUpdatOption dto, int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.UpdateEvent(id, _IUserRepository.GetUserID(), dto.EventDto, dto.Options, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/Cancel")]
        public async Task<ActionResult> Cancel([FromRoute] int id,  [FromHeader] string lang)
        {
            var result = await _IEventRepository.Cancel(id, _IUserRepository.GetUserID(), lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }
    }
}