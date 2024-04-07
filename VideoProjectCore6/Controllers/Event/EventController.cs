using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.DTOs.CommonDto;

#nullable disable
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
        [HttpPost("AddGroupLocal")]
        public async Task<ActionResult> AddGroupEventLocal([FromBody] FullEventPostDto dto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IEventRepository.AddConnectedEvents(dto, _IUserRepository.GetUserID(), lang, false));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("MeetingEvent")]
        public async Task<ActionResult> AddMeetingEvent([FromBody] EventWParticipant dto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IEventRepository.AddMeetingEvent(dto, _IUserRepository.GetUserID(), true, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public async Task<ActionResult> GetLocal([FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.GetAllOfUser(_IUserRepository.GetUserID(), null, false, lang.ToLower());
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
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
        [HttpPost("{eventId}/AddParticipants")]
        public async Task<ActionResult> AddParticipants([FromRoute] int eventId, [FromBody] ParticipantsAsObj dto, [FromHeader] bool sendToAll = false, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.AddParticipantsToEventsScoped(dto, eventId, _IUserRepository.GetUserID(), lang, true, sendToAll);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
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
        
        //[HttpPost("Public")]
        //public async Task<ActionResult> GetAllPublic([FromBody] EventSearchObject obj, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        //{
        //    var result = await _IEventRepository.GetPPEventByType(0, obj,"ar");
        //    if (result != null)
        //    {
        //        return StatusCode(StatusCodes.Status200OK, result);
        //    }
        //    return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("{id}/Unlink")]
        public async Task<ActionResult> Unlink([FromRoute] int id)
        {
            var result = await _IEventRepository.UnlinkSubEvent(id, _IUserRepository.GetUserID());
            if (result.Id > 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetNumOfMeetingsDone")]
        public async Task<ActionResult> GetNumOfMeetingsDone([FromQuery] DateTime startDate, DateTime endDate)
        {
            var result = await _IEventRepository.GetNumOfMeetingsDone(startDate, endDate);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetNumOfFutureMeetings")]
        public async Task<ActionResult> GetNumOfFutureMeetings([FromQuery] DateTime startDate, DateTime endDate)
        {
            var result = await _IEventRepository.GetNumOfFutureMeetings(startDate, endDate);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{eventId}/UpdateParticipants")]
        public async Task<ActionResult> UpdateEventParticipants([FromRoute] int eventId, [FromBody] ParticipantsAsObj dto, [FromHeader] bool sendToAll, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.UpdateEventParticipants(dto, eventId, _IUserRepository.GetUserID(), lang, true, sendToAll);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        //[Key]
        [HttpGet("{eventId}")]
        public async Task<ActionResult> Event([FromRoute] string eventId, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.EventById(eventId);
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
        [HttpGet("{id}/MeetingLinks")]
        public async Task<ActionResult> EventLinks([FromRoute] int id, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.EventParticipantLinks(id, 4);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddType")]
        public async Task<ActionResult> AddEventType([FromBody] List<Dictionary<string, string>> value)
        {
            var result = await _IEventRepository.AddEventType(value);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteEventType")]
        public async Task<ActionResult> DeleteEventType([FromQuery] int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.DeleteEventType(id, lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddRecurrence")]
        public async Task<ActionResult> AddREvent([FromBody] RecurrenceEvent events, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.AddRecurrenceEvents(events.Event, events.RDates, _IUserRepository.GetUserID(), true, lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("EditEventType")]
        public async Task<ActionResult> EditEventType([FromBody] List<Dictionary<string, string>> value, [FromQuery] int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.EditEventType(value, id, lang);

            if (result != null)

            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ShiftRecurrence")]
        public async Task<ActionResult> ShiftREvent([FromQuery] int eventId, [FromBody] DateTimeRange dto, [FromHeader] string lang = "ar")
        {
            var result = await _IEventRepository.ShiftRecurrenceEvents(eventId, dto, _IUserRepository.GetUserID(), false, lang);
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
        [HttpPut("Reschedule/{id}")]
        public async Task<ActionResult> Reschedule(MeetingEventDto dto, int id, [FromHeader] string lang)
        {
            var result = await _IEventRepository.Reschedule(id, dto, _IUserRepository.GetUserID(), lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/Cancel")]
        public async Task<ActionResult> Cancel([FromRoute] int id,  [FromHeader] string lang)
        {
            var result = await _IEventRepository.Cancel(id, _IUserRepository.GetUserID(), lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }
        [HttpPost("ActiveMeetings")]
        public async Task<IActionResult> ActiveMeetings([FromBody] DateTimeRange range, [FromHeader] string lang)
        {
            return Ok(await _IEventRepository.ActiveMeetings(range, lang));
        }

        [HttpPost("FinishedMeetings")]
        public async Task<IActionResult> FinishedMeetings([FromBody] DateTimeRange range, [FromQuery] string meetingId, [FromHeader] string lang)
        {
            return Ok(await _IEventRepository.FinishedMeetings(range, meetingId, lang));
        }

        [HttpGet("MeetingDetails")]
        public async Task<IActionResult> MeetingDetails([FromQuery] int? id, [FromQuery] string meetingId, [FromHeader] string lang)
        {
            return Ok(await _IEventRepository.MeetingDetails(id, meetingId, lang));
        }

        
    }
}