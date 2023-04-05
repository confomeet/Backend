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
        private readonly IUserRepository _IUserRepository;
        public StatisticsController(IStatisticsRepository iStatisticsRepository, IUserRepository iUserRepository)
        {
            _IStatisticsRepository = iStatisticsRepository;
            _IUserRepository = iUserRepository;
        }
       
        [HttpPost("EvensByApp")]
        public async Task<IActionResult> EvensByApp([FromBody] DateTimeRange range, [FromHeader] string lang)
        {
            return Ok(await _IStatisticsRepository.EventsByApp(range,  lang));
        }

        [HttpPost("UsersByStatus")]
        public async Task<IActionResult> ActiveUsers([FromBody] DateTimeRange range, [FromHeader] string lang = "ar")
        {
            return Ok(await _IStatisticsRepository.UsersByStatus(range, lang));
        }

        [HttpPost("EventsByStatus")]
        public async Task<IActionResult> ActiveRooms([FromBody] DateTimeRange range, [FromHeader] short? eventType,[FromHeader] string lang)
        {
            var param = eventType != 0 ? eventType : null;

            return Ok(await _IStatisticsRepository.ActiveRooms(range, param, lang));
        }

    }
}
