using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.RecordingDto;
using VideoProjectCore6.Repositories.IRecordingRepository;

namespace VideoProjectCore6.Controllers.Recording
{
   

        [ApiController]
        [Route("api/v1/[controller]")]
        public class RecordingController : ControllerBase
        {
            private readonly IRecordingRepository _IRecordingRepository;

            public RecordingController(IRecordingRepository iRecordingRepository)
            {
            _IRecordingRepository = iRecordingRepository;
            }

            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            [HttpPost("AddVideoRecording")]
            public async Task<ActionResult> Add([FromBody] RecordingPostDto dto)
            {
                return Ok(await _IRecordingRepository.AddRecordingLog(dto));
            }

            // FIXME: request must be authorized
            [HttpPost("AddS3VideoRecording")]
            public async Task<ActionResult> AddS3VideoRecording([FromBody] S3RecordingPostDto recording) {
                var result = await _IRecordingRepository.AddS3Recording(recording);
                if (result.Id < 0)
                    return Problem(result.Message[0], null, StatusCodes.Status500InternalServerError, "Failed to save S3 recording");
                return Ok(result);
            }

            [HttpGet("DownloadS3Record/{recordingId}")]
            public async Task<ActionResult> DownloadS3Record(Guid recordingId) {
                var downloadUrl = await _IRecordingRepository.GetS3RedirectUrl(recordingId);
                if (downloadUrl == null)
                    return NotFound();
                return Redirect(downloadUrl);
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
