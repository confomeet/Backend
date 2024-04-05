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
    public class ConfEventController : ControllerBase
    {
        private readonly IConfEventRepository _IConfEventRepository;
    
        public ConfEventController(IConfEventRepository iConfEventRepository)
        {
            _IConfEventRepository = iConfEventRepository;

            
     
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ConfEventPostDto dto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.addConfEvent(dto, lang));
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetById")]
        public async Task<ActionResult> GetById([FromHeader] int id, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.getConfEventById(id, lang));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.getConfEvents(lang));
        }

        [HttpPost("AddProsodyEvent")]
        public async Task<ActionResult> AddProsodyEvent([FromBody] ProsodyEventPostDto prosodyEventPostDto)
        {
            return Ok(await _IConfEventRepository.addProsodyEvent(prosodyEventPostDto, EventHub.Current));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("roomList")]
        public async Task<ActionResult> GetRoom([FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.handleListRoom(lang));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("room/{meetingId}/{id}")]
        public async Task<ActionResult> GetParticipantsByRoom([FromBody] DateTimeRange range, [FromRoute] string meetingId, [FromRoute] string id, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.handleGetRoom(range, id, meetingId, lang));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("roomUsersList")]
        public async Task<ActionResult> GetParticipantsByRooms([FromBody] List<string> roomNames, [FromHeader] string lang = "ar")
        {
            return Ok(await _IConfEventRepository.handleRoomsUsersList(roomNames, lang));
        }


    }
}
