using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IStatisticsRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.ConfEventService;
using VideoProjectCore6.Utility.Authorization;

namespace VideoProjectCore6.Controllers.Statistics
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepository _IStatisticsRepository;
        public StatisticsController(IStatisticsRepository iStatisticsRepository)
        {
            _IStatisticsRepository = iStatisticsRepository;
        }

        [HttpPost("EvensByApp")]
        [HasPermission(Permissions.SystemStats_Read)]
        public async Task<IActionResult> ByApp([FromBody] DateTimeRange range, [FromHeader] string lang)
        {
            return Ok(await _IStatisticsRepository.ByApp(range, lang));
        }

        [HttpPost("UsersByStatus")]
        [HasPermission(Permissions.SystemStats_Read)]
        public async Task<IActionResult> ByOnlineUsers([FromBody] DateTimeRange range, [FromHeader] string lang)
        {
            return Ok(await _IStatisticsRepository.ByOnlineUsers(range, lang));
        }

        [HttpPost("EventsByStatus")]
        [HasPermission(Permissions.SystemStats_Read)]
        public async Task<IActionResult> ByMeetingStatus([FromBody] DateTimeRange range, [FromHeader] string lang)
        {
            return Ok(await _IStatisticsRepository.ByMeetingStatus(range, lang));
        }

    }
}
