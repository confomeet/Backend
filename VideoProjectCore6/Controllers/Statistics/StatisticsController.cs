using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IStatisticsRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.ConfEventService;

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
        [Authorize]
        public async Task<IActionResult> ByApp([FromBody] DateTimeRange range)
        {
            return Ok(await _IStatisticsRepository.ByApp(range));
        }

        [HttpPost("UsersByStatus")]
        [Authorize]
        public async Task<IActionResult> ByOnlineUsers([FromBody] DateTimeRange range)
        {
            return Ok(await _IStatisticsRepository.ByOnlineUsers(range));
        }

        [HttpPost("EventsByStatus")]
        [Authorize]
        public async Task<IActionResult> ByMeetingStatus([FromBody] DateTimeRange range)
        {
            return Ok(await _IStatisticsRepository.ByMeetingStatus(range));
        }

    }
}
