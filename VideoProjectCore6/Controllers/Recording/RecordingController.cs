using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.RecordingDto;
using VideoProjectCore6.Repositories.IRecordingRepository;

namespace VideoProjectCore6.Controllers.Recording
{
    // FIXME: this controller must authorize all requests, but we jibri would require to add some token generation
    // logic, which is not so convinient if we use only the bash.
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RecordingController(IRecordingRepository iRecordingRepository) : ControllerBase
    {
        private readonly IRecordingRepository _IRecordingRepository = iRecordingRepository;

        [HttpPost("AddVideoRecording")]
        public async Task<ActionResult> Add([FromBody] RecordingPostDto dto)
        {
            return Ok(await _IRecordingRepository.AddRecordingLog(dto));
        }

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
    }
}
