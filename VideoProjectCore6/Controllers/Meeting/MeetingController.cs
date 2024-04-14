using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.MeetingDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Repositories.IMeetingRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
#nullable disable
namespace VideoProjectCore6.Controllers.Meeting
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MeetingController : ControllerBase
    {

        private readonly IMeetingRepository _IMeetingRepository;
        private readonly IUserRepository _iUserRepository;
        public MeetingController(IMeetingRepository iMeetingRepository, IUserRepository iUserRepository)
        {
            _IMeetingRepository = iMeetingRepository;
            _iUserRepository = iUserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> AddMeeting([FromBody] MeetingPostDto meetingDto, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.AddMeeting(meetingDto, _iUserRepository.GetUserID(), lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error accrued");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetings(_iUserRepository.GetUserID(), lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(MeetingPostDto MeetingPostDto, int id, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.UpdateMeeting(id, MeetingPostDto, _iUserRepository.GetUserID(), lang);
            return StatusCode(StatusCodes.Status200OK, result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetingById(id, lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpGet("GetMeetingInfo")]
        public async Task<ActionResult> GetMeetingInfo(string meetingId, string password, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetingByMeetingIdAndPassword(meetingId, password, lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("LogInToMeeting")]
        public async Task<ActionResult> LogInToMeeting(string meetingNo)
        {
            var result = await _IMeetingRepository.LogInToMeeting(meetingNo);
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Invite")]
        public async Task<ActionResult> InviteToMeeting([FromBody] List<string> contacts,string meetingId)
        {
            var result = await _IMeetingRepository.InviteToMeeting(meetingId, contacts);
            return Ok(result);
        }

        [HttpGet("GetMeetingForOrderNo")]
        public async Task<ActionResult> GetMeetingForOrderNo(int OrderNo)
        {
            var result = await _IMeetingRepository.GetMeetingByOrderNo(OrderNo);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpGet("IsAttendedByAppNo")]
        public async Task<ActionResult> IsAttended(int meetingId)
        {
            var result = await _IMeetingRepository.IsAttendedByAppNo(meetingId);
            return Ok(result);
        }

        [HttpGet("hasPassword")]
        public async Task<bool> MeetingHasPassword(string meetingId)
        {
            return await _IMeetingRepository.MeetingHasPassword(meetingId);
        }

        [HttpPost("anonymJWT")]
        public async Task<IActionResult> anonymJWT(string meetingId, [FromBody] JoinData userData, [FromHeader] string lang)
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, null, userData, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Join")]
        public async Task<IActionResult> moderatorJWT(string meetingId, [FromHeader] string lang)
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, _iUserRepository.GetUserID(), null, lang);
            return Ok(obj);
        }

        [HttpPost("Join/{meetingId}")]
        public async Task<IActionResult> joinAnonymous([FromRoute] string meetingId, [FromBody] JoinData userData, [FromHeader] string lang = "ar")
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, null, userData, lang);
            return Ok(obj);
        }
        [HttpPost("Join/{userId}/{meetingId}")]
        public async Task<IActionResult> joinTo([FromRoute] string meetingId, [FromRoute] int userId , [FromHeader] string lang = "ar")
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, userId, null, lang);
            return Ok(obj);
        }

        [HttpPost("Verify/{ParticipantId}/{Guid}")]
        public async Task<IActionResult> joinTo_([FromRoute] int ParticipantId,[FromQuery] int? partyId, [FromRoute] Guid Guid, [FromHeader] string lang = "ar")
        {
            var result = await _IMeetingRepository.MeetingJWT(ParticipantId,Guid, partyId,  lang);
            return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("ReEnableMeeting")]
        public async Task<IActionResult> ReEnableMeeting(string meetingId, [FromHeader] string lang)
        {
            object obj = await _IMeetingRepository.SetMeetingStatus(meetingId, Constants.MEETING_STATUS.STARTED, false, lang);
            return Ok(obj);
        }


    }
}
