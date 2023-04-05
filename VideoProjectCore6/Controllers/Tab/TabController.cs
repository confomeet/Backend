using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.TabDto;
using VideoProjectCore6.Repositories.ITabRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
#nullable disable
namespace VideoProjectCore6.Controllers.Tab
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TabController : ControllerBase
    {
        private readonly ITabRepository _ITabRepository;
        private readonly IUserRepository _IUserRepository;
        public TabController(ITabRepository iTabRepository, IUserRepository iuserRepository)
        {
            _ITabRepository = iTabRepository;
            _IUserRepository = iuserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddTab")]
        public async Task<ActionResult> AddTab([FromBody] TabPermPostDto tabDto, [FromHeader] string lang)
        {
            var result = await _ITabRepository.AddTab(tabDto);
            return result.Id > 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("updateTabById")]
        public async Task<ActionResult> Update([FromBody] TabPermPostDto tabDto, int id, [FromHeader] string lang)
        {
            var result = await _ITabRepository.UpdateTab(tabDto, id);
            return result.Id > 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _ITabRepository.DeleteTab(id, lang);
            return result.Id > 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _ITabRepository.GetTabs(lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetMyTabs")]
        public async Task<ActionResult> GetMyTabs([FromHeader] string lang)
        {
            var result = await _ITabRepository.GetMyTabs(_IUserRepository.GetUserID(), lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        // TODO should be only for admin.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("UpdateIconTab")]
        public async Task<ActionResult> UpdateIconTab([FromForm] FromFileDto file, int rowId)
        {
            var result = await _ITabRepository.UpdateIconTab(file.IconImage, rowId);
            return result != 0 ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        // TODO should be only for admin.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("UpdateIconStringTab")]
        public async Task<ActionResult> UpdateIconStringTab(string iconString, int rowId)
        {
            var result = await _ITabRepository.UpdateIconStringTab(iconString, rowId);
            return result != 0 ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
