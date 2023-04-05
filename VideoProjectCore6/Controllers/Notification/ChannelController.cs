using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.Repositories.IChannelRepository;

namespace VideoProjectCore6.Controllers.Notification
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChannelController : ControllerBase
        
    { 
        private readonly IChannelRepository _iChannelRepository;
        public ChannelController(IChannelRepository iChannelRepository)
        {
            _iChannelRepository = iChannelRepository;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult GetChannelNames([FromHeader] string lang)
        {
            var result = _iChannelRepository.GetChannelsName(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
