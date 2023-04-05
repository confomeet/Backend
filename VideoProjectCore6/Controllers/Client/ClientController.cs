using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.ClientDto;
using VideoProjectCore6.Repositories.IClientRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
#nullable disable
namespace VideoProjectCore6.Controllers.Client
{
    //[ApiController]
    //[Route("api/v1/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _IClientRepository;
        private readonly IUserRepository _IUserRepository;
        public ClientController(IClientRepository iClientRepository, IUserRepository iUserRepository)
        {
            _IClientRepository = iClientRepository;
            _IUserRepository = iUserRepository;
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPost]
        //public async Task<ActionResult> Add([FromBody] ClientDto dto, [FromHeader] string lang = "ar")
        //{
        //    var result = await _IClientRepository.Add(dto, _IUserRepository.GetUserID(), lang);
        //    return result.Id > 0 ? Ok(result) : BadRequest(result);
        //}
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPut("{id}")]
        //public async Task<ActionResult> Put([FromRoute] ushort id, [FromBody] ClientDto dto, [FromHeader] string lang = "ar")
        //{
        //    var result = await _IClientRepository.Update(id, dto, _IUserRepository.GetUserID(), lang);
        //    return result.Id > 0 ? Ok(result) : BadRequest(result);
        //}
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpGet]
        //public async Task<ActionResult> GetAll([FromHeader] string lang = "ar")
        //{
        //    try
        //    {
        //        return Ok(await _IClientRepository.GetAll(lang));
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpGet("List")]
        //public async Task<ActionResult> GetList([FromHeader] string lang = "ar")
        //{
        //    try
        //    {
        //        return Ok(await _IClientRepository.GetView(lang));
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPut("Activate/{id}")]
        //public async Task<ActionResult> Activate([FromRoute] ushort id, [FromHeader] string lang = "ar")
        //{
        //    var result = await _IClientRepository.SetActivation(id, true, _IUserRepository.GetUserID(), lang);
        //    return result.Id > 0 ? Ok(result) : BadRequest(result);
        //}
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        //[HttpPut("Deactivate/{id}")]
        //public async Task<ActionResult> Deactivate([FromRoute] ushort id, [FromHeader] string lang = "ar")
        //{
        //    var result = await _IClientRepository.SetActivation(id,false, _IUserRepository.GetUserID(), lang);
        //    return result.Id > 0 ? Ok(result) : BadRequest(result);
        //}
    }
}
