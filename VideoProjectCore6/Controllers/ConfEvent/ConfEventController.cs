using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Services;

namespace VideoProjectCore6.Controllers.ConfEvent
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ConfEventController(IConfEventRepository iConfEventRepository) : ControllerBase
    {
        private readonly IConfEventRepository _IConfEventRepository = iConfEventRepository;

        [HttpPost("AddProsodyEvent")]
        public async Task<ActionResult> AddProsodyEvent([FromBody] ProsodyEventPostDto prosodyEventPostDto)
        {
            return Ok(await _IConfEventRepository.AddProsodyEvent(prosodyEventPostDto));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("ActiveRoomsList")]
        public async Task<ActionResult> GetRoom([FromHeader] string lang = "en")
        {
            return Ok(await _IConfEventRepository.HandleListRoom(lang));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("room/{meetingId}/{id}")]
        public async Task<ActionResult> GetParticipantsByRoom([FromBody] DateTimeRange range, [FromRoute] string meetingId, [FromRoute] string id, [FromHeader] string lang = "en")
        {
            return Ok(await _IConfEventRepository.HandleGetRoom(range, id, meetingId));
        }
    }
}
