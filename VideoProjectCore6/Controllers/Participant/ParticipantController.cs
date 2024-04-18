using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Repositories.IParticipantRepository;
using VideoProjectCore6.Repositories.IUserRepository;
#nullable disable
namespace VideoProjectCore6.Controllers.Participant;

[ApiController]
[Route("api/v1/[controller]")]
public class ParticipantController : ControllerBase
{

    private readonly IParticipantRepository _IParticipantRepository;
    private readonly IUserRepository _IUserRepository;

    public ParticipantController(IParticipantRepository iParticipantRepository, IUserRepository iUserRepository)
    {
        _IParticipantRepository = iParticipantRepository;
        _IUserRepository = iUserRepository;
    }

    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("Search")]
    public async Task<ActionResult> SearchParticipant([FromQuery] string toSearch, [FromHeader] string lang = "en")
    {
        var result = await _IParticipantRepository.SearchParticipant(toSearch);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
    {
        return Ok(await _IParticipantRepository.Delete(id, _IUserRepository.GetUserID(), lang));
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("{id}/Remind")]
    public async Task<ActionResult> Remind(int id,[FromBody] JoinData participant, [FromHeader] string lang="ar")
    {
        var result = await _IParticipantRepository.ReNotifyParticipant(id, participant.Email, lang);
        return result.Id > 0 ? Ok(result) : StatusCode(StatusCodes.Status500InternalServerError, result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{id}/Liberate")]
    public async Task<ActionResult> Liberate([FromRoute] int id,[FromHeader] string lang = "en")
    {
        var result = await _IParticipantRepository.Liberate(id, _IUserRepository.GetUserID(),lang);
        return result.Id > 0 ? Ok(result) : BadRequest(result);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{id}/SetNote")]
    public async Task<ActionResult> SetNote([FromRoute] int id,[FromBody] NoteDto dto, [FromHeader] string lang = "en")
    {
        var result = await _IParticipantRepository.UpdateNote(id, dto.note, _IUserRepository.GetUserID(), lang);
        return result.Id > 0 ? Ok(result) : BadRequest(result);
    }
}

