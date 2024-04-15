using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Utility.Authorization;


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

        [HasPermission(Permissions.Group_Create)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroup([FromBody] UserGroupPostDto userGroupPostDto, [FromHeader] string lang)
        {
            return Ok(await _IGroupRepository.AddGroup(userGroupPostDto, _IUserRepository.GetUserID()));
        }

        [HasPermission(Permissions.Group_Update)]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] UserGroupPostDto userGroupPostDto, [FromQuery] int groupId, [FromHeader] string lang)
        {
            return Ok(await _IGroupRepository.EditGroup(userGroupPostDto, groupId, _IUserRepository.GetUserID()));
        }

        [HasPermission(Permissions.Group_Read)]
        [HttpGet()]
        public async Task<IActionResult> GetAllGroups([FromQuery] string? text, [FromQuery] string? groupName, [FromHeader] int pageSize = 25, [FromHeader] int pageIndex = 1, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.GetGroups(text, groupName, lang, pageSize, pageIndex));
        }

        [HasPermission(Permissions.Group_Delete)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteGroup([FromQuery] int groupId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.DeleteGroup(groupId, lang));
        }

        [HasPermission(Permissions.Group_Update)]
        [HttpPost("UsersToGroup")]
        public async Task<IActionResult> AddUsersToGroup([FromBody] GroupPostUserDto groupPostUserDto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.AddUsersToGroup(groupPostUserDto, lang));
        }

        [HasPermission(Permissions.Group_Read)]
        [HttpDelete("UsersOfGroup")]
        public async Task<IActionResult> RemoveUsersOfGroup([FromBody] GroupPostUserDto groupPostUserDto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.RemoveUsersFromGroup(groupPostUserDto, lang));
        }

        [HasPermission(Permissions.Group_Read)]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUsersById([FromQuery] int groupId,[FromHeader] int pageIndex = 1,[FromHeader] int pageSize = 25,[FromHeader] string lang = "ar")
        {
            return Ok(await _IGroupRepository.GetUsersByGroupId(groupId, pageIndex, pageSize));
        }

    }

}
