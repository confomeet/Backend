using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.Hubs;
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
            return Ok(await _IConfEventRepository.AddProsodyEvent(prosodyEventPostDto, EventHub.Current!));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("roomList")]
        public async Task<ActionResult> GetRoom([FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.HandleListRoom(lang));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("room/{meetingId}/{id}")]
        public async Task<ActionResult> GetParticipantsByRoom([FromBody] DateTimeRange range, [FromRoute] string meetingId, [FromRoute] string id, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.HandleGetRoom(range, id, meetingId));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("roomUsersList")]
        public async Task<ActionResult> GetParticipantsByRooms([FromBody] List<string> roomNames, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.HandleRoomsUsersList(roomNames, lang));
        }
    }
}
