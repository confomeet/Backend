using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Tokens;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.DTOs.ContactDto;
using VideoProjectCore6.Hubs;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IContactRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Services.ConfEventService;

namespace VideoProjectCore6.Controllers.ConfEvent
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ConfEventController : ControllerBase
    {
        private readonly IConfEventRepository _IConfEventRepository;
        private readonly IUserRepository _IUserRepository;
     
    
        public ConfEventController(IConfEventRepository iConfEventRepository)
        {
            _IConfEventRepository = iConfEventRepository;
            //  _IUserRepository = iUserRepository;

            
     
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
