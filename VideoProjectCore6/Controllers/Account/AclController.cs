using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.Repositories.IAclRepository;

namespace VideoProjectCore6.Controllers.Account
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AclController : ControllerBase
    {
        private readonly IAclRepository _IAclRepository;
        // private readonly IStringLocalizer<UserController> _localizer;
        //  private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        //private readonly IWebHostEnvironment _IWebHostEnvironment;
        public AclController(IAclRepository iAclRepository)
        {
            _IAclRepository = iAclRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAcls([FromQuery] string? name, [FromHeader] string lang)
        {
            var obj = await _IAclRepository.GetACLs(name, lang);
            return Ok(obj);
        }
    }
}
