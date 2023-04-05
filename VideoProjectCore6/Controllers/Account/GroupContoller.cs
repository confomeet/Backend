using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;


namespace VideoProjectCore6.Controllers.Account
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _IGroupRepository;
        private readonly IUserRepository _IUserRepository;
        // private readonly IStringLocalizer<UserController> _localizer;
        //  private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        //private readonly IWebHostEnvironment _IWebHostEnvironment;
        public GroupController(IGroupRepository iGroupRepository, IUserRepository iUserRepository)
        {
            _IGroupRepository = iGroupRepository;
            _IUserRepository = iUserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroup([FromBody] UserGroupPostDto userGroupPostDto, [FromHeader] string lang)
        {
            return Ok(await _IGroupRepository.AddGroup(userGroupPostDto, _IUserRepository.GetUserID()));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] UserGroupPostDto userGroupPostDto, [FromQuery] int groupId, [FromHeader] string lang)
        {
            return Ok(await _IGroupRepository.EditGroup(userGroupPostDto, groupId, _IUserRepository.GetUserID()));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public async Task<IActionResult> GetAllGroups([FromQuery] string? text, [FromQuery] string? groupName, [FromHeader] int pageSize = 25, [FromHeader] int pageIndex = 1, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.GetGroups(text, groupName, lang, pageSize, pageIndex));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteGroup([FromQuery] int groupId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.DeleteGroup(groupId, lang));
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("UsersToGroup")]
        public async Task<IActionResult> AddUsersToGroup([FromBody] GroupPostUserDto groupPostUserDto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.AddUsersToGroup(groupPostUserDto, lang));
        }

        [HttpDelete("UsersOfGroup")]
        public async Task<IActionResult> RemoveUsersOfGroup([FromBody] GroupPostUserDto groupPostUserDto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.RemoveUsersFromGroup(groupPostUserDto, lang));
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUsersById([FromQuery] int groupId,[FromHeader] int pageIndex = 1,[FromHeader] int pageSize = 25,[FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.GetUsersByGroupId(groupId, pageIndex, pageSize));
        }

    }

}
