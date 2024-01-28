using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.RecordingDto;
using VideoProjectCore6.Repositories.IRecordingRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.RecordingService;

namespace VideoProjectCore6.Controllers.Recording
{
   

        [ApiController]
        [Route("api/v1/[controller]")]
        public class RecordingController : ControllerBase
        {
            private readonly IRecordingRepository _IRecordingRepository;
            private readonly IUserRepository _IUserRepository;

            public RecordingController(IRecordingRepository iRecordingRepository, IUserRepository iUserRepository)
            {
            _IRecordingRepository = iRecordingRepository;
                //  _IUserRepository = iUserRepository;
            }

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            [HttpPost("AddVideoRecording")]
            public async Task<ActionResult> Add([FromBody] RecordingPostDto dto)
            {
                return Ok(await _IRecordingRepository.AddRecordingLog(dto));
            }

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            //[HttpGet("GetById")]
            //public async Task<ActionResult> GetById([FromHeader] int id, [FromHeader] string lang = "ar")
            //{
            //    return Ok(await _IConfEventRepository.getConfEventById(id, lang));
            //}

            //[HttpGet]
            //public async Task<ActionResult> GetAll([FromHeader] string lang = "ar")
            //{
            //    return Ok(await _IConfEventRepository.getConfEvents(lang));
            //}

            //[HttpPost("AddProsodyEvent")]
            //public async Task<ActionResult> AddProsodyEvent([FromBody] ProsodyEventPostDto prosodyEventPostDto)
            //{
            //    return Ok(await _IConfEventRepository.addProsodyEvent(prosodyEventPostDto));
            //}

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
            //[HttpGet("roomList")]
            //public async Task<ActionResult> GetRoom([FromHeader] string lang = "ar")
            //{
            //    return Ok(await _IConfEventRepository.handleListRoom(lang));
            //}

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
            //[HttpGet("room/{meetingId}/{id}")]
            //public async Task<ActionResult> GetParticipantsByRoom([FromRoute] string meetingId, [FromRoute] string id, [FromHeader] string lang = "ar")
            //{
            //    return Ok(await _IConfEventRepository.handleGetRoom(id, meetingId, lang));
            //}

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
            //[HttpGet("roomUsersList")]
            //public async Task<ActionResult> GetParticipantsByRooms([FromBody] List<string> roomNames, [FromHeader] string lang = "ar")
            //{
            //    return Ok(await _IConfEventRepository.handleRoomsUsersList(roomNames, lang));
            //}


        }

    
}
