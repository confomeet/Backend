namespace VideoProjectCore6.Controllers.Queue
{
    //{

    //    [Route("api/v1/[controller]")]
    //    [ApiController]
    //    public class QueueController : ControllerBase
    //    {

    //        private readonly IQueueRepository _IQueueRepository;
    //        private readonly IUserRepository _IUserRepository;
    //        private readonly IMeetingRepository _IMeetingRepository;

    //        public QueueController(IQueueRepository iQueueRepository, IUserRepository iUserRepository, IMeetingRepository iMeetingRepository)
    //        {
    //            _IQueueRepository = iQueueRepository;
    //            _IUserRepository = iUserRepository;
    //            _IMeetingRepository = iMeetingRepository;
    //        }


    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpPost]
    //        public async Task<ActionResult> AddQueue([FromBody] QueuePostDto queueDto)
    //        {
    //            var result = await _IQueueRepository.AddQueueProcess(queueDto);
    //            if (result != 0)
    //            {
    //                return this.StatusCode(StatusCodes.Status200OK, result);
    //            }

    //            else return this.StatusCode(StatusCodes.Status404NotFound, "error accrued");
    //        }



    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("process")]
    //        public async Task<ActionResult> GetProcess(int processNo, int serviceKindNo)
    //        {
    //            var result = await _IQueueRepository.GetQueueProcess(processNo, serviceKindNo);
    //            if (result != null)
    //            {

    //                return this.StatusCode(StatusCodes.Status200OK, result);
    //            }

    //            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");

    //        }



    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpPut("{id}")]
    //        public async Task<ActionResult> Update(QueuePostDto queuePostDto, int id)
    //        {
    //            var result = await _IQueueRepository.UpdateQueueProcess(id, queuePostDto);
    //            if (result != 0)
    //            {

    //                return this.StatusCode(StatusCodes.Status200OK, result);
    //            }

    //            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
    //        }

    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("GetNextOrder")]
    //        public async Task<ActionResult> GetNextOrder()
    //        {
    //            return Ok(await _IQueueRepository.GetNextOrder(_IUserRepository.GetUserID()));
    //        }


    //        // called from inside.
    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("ChangeTicketStatusToInProgress")]
    //        public async Task<ActionResult> ChangeTicketsStatusToInProgressByProcessNo(int processNo)
    //        {
    //            bool res = true;
    //            //if (_IUserRepository.IsEmployee())
    //            //{
    //            //    res = await _IQueueRepository.ChangeTicketsStatusToInProgressByProcessNo(_IUserRepository.GetUserID(), processNo);
    //            //}
    //            return Ok(res);
    //        }

    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("GetCurrentQueueStatistics")]
    //        public async Task<ActionResult> GetCurrentQueueStatistics(bool onlyMyApp)
    //        {
    //            return Ok(await _IQueueRepository.GetCurrentQueueStatistics(onlyMyApp));
    //        }


    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("PickTicket")]
    //        public async Task<ActionResult> Get(int processId, int serviceKindNo, DateTime dateTime, bool bookTicket)
    //        {
    //            var result = await _IQueueRepository.PickTicket(processId, serviceKindNo, _IUserRepository.GetUserID(), dateTime, bookTicket);
    //            if (result != null)
    //            {
    //                return this.StatusCode(StatusCodes.Status200OK, result);
    //            }
    //            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
    //        }


    //        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //        [HttpGet("GetQueueForStatus")]
    //        public async Task<ActionResult> GetQueueForStatus(Constants.PROCESS_STATUS procStatus)
    //        {
    //            var obj = await _IQueueRepository.GetQueueForStatus(procStatus);
    //            return Ok(obj);
    //        }

    //    }
    //}
}
