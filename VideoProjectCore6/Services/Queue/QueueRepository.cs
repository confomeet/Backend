namespace VideoProjectCore6.Services.Queue
{
  //  public class QueueRepository : IQueueRepository
//    {
//        private readonly EngineCoreDBContext _EngineCoreDBContext;
//        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
//        private readonly IWorkingTimeRepository _iWorkingTimeRepository;
//        private readonly IGeneralRepository _iGeneralRepository;
//        private readonly IAdmServiceRepository _iAdmServiceRepository;
//        private readonly ISysValueRepository _iSysValueRepository;
//        private readonly IStatisticsRepository _iStatisticsRepository;
//        private readonly IUserRepository _iUserRepository;
//        private readonly ITransactionRepository _iTransactionRepository;
//        private readonly IApplicationPartyRepository _iApplicationPartyRepository;
//        private readonly IBlackListRepository _iBlackListRepository;
//        private readonly ILogger<QueueRepository> _iLogger;
//        private readonly IConfiguration _IConfiguration;
//        ValidatorException _exception;

    //        public QueueRepository(EngineCoreDBContext EngineCoreDBContext, IGlobalDayOffRepository iGlobalDayOffRepository, IWorkingTimeRepository iWorkingTimeRepository, ILogger<QueueRepository> iLogger,
    //                               IUserRepository iUserRepository, ISysValueRepository iSysValueRepository, IAdmServiceRepository iAdmServiceRepository, IGeneralRepository iGeneralRepository, IApplicationPartyRepository iApplicationPartyRepository,
    //                               ITransactionRepository iTransactionRepository, IConfiguration iConfiguration, IBlackListRepository iBlackListRepository, IStatisticsRepository iStatisticsRepository)
    //        {
    //            _EngineCoreDBContext = EngineCoreDBContext;
    //            _iGlobalDayOffRepository = iGlobalDayOffRepository;
    //            _iWorkingTimeRepository = iWorkingTimeRepository;
    //            _iGeneralRepository = iGeneralRepository;
    //            _iAdmServiceRepository = iAdmServiceRepository;
    //            _iUserRepository = iUserRepository;
    //            _iSysValueRepository = iSysValueRepository;
    //            _iStatisticsRepository = iStatisticsRepository;
    //            _iTransactionRepository = iTransactionRepository;
    //            _iApplicationPartyRepository = iApplicationPartyRepository;
    //            _iBlackListRepository = iBlackListRepository;
    //            _IConfiguration = iConfiguration;
    //            _iLogger = iLogger;
    //            _exception = new ValidatorException();
    //        }


    //        public async Task<int> AddQueueProcess(QueuePostDto queueDto)
    //        {
    //            // TODO: add validations.
    //            QueueProcesses queueProcess = queueDto.GetEntity();
    //            queueProcess.Status = (int)Constants.PROCESS_STATUS.PENDING;
    //            _EngineCoreDBContext.QueueProcesses.Add(queueProcess);
    //            await _EngineCoreDBContext.SaveChangesAsync();
    //            return queueProcess.Id;
    //        }

    //        public async Task<int> UpdateQueueProcess(int rowId, QueuePostDto queueDto)
    //        {
    //            int res = 0;
    //            QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.Id == rowId).FirstOrDefaultAsync();

    //            if (originalQueue == null)
    //            {
    //                return res;
    //            }

    //            originalQueue.ExpectedDateTime = queueDto.ExpectedDate;
    //            originalQueue.ProcessNo = queueDto.ProcessNo;
    //            originalQueue.ServiceKindNo = queueDto.ServiceKindNo;
    //            originalQueue.TicketId = queueDto.TicketId;
    //            originalQueue.Status = queueDto.Status;
    //            originalQueue.Provider = queueDto.Provider;
    //            originalQueue.Note = queueDto.Note;
    //            originalQueue.CreatedBy = queueDto.CreatedBy;
    //            originalQueue.CreatedDate = queueDto.CreatedDate;
    //            originalQueue.LastUpdatedBy = queueDto.UpdatedBy;
    //            originalQueue.LastUpdatedDate = queueDto.UpdatedDate;
    //            originalQueue.Note = queueDto.Note;
    //            originalQueue.NotifyHighLevel = queueDto.NotifyHighLevel;
    //            originalQueue.NotifyLowLevel = queueDto.NotifyLowLevel;
    //            originalQueue.NotifyMediumLevel = queueDto.NotifyLowLevel;

    //            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
    //            await _EngineCoreDBContext.SaveChangesAsync();
    //            res = originalQueue.Id;
    //            return res;
    //        }

    //        public async Task<QueueGetDto> GetQueueProcess(int processNo, int serviceKindNo)
    //        {
    //            QueueProcesses createdQueue = new QueueProcesses();
    //            createdQueue = await _EngineCoreDBContext.QueueProcesses.Where(d => d.ProcessNo == processNo && d.ServiceKindNo == serviceKindNo).FirstOrDefaultAsync();
    //            if (createdQueue == null)
    //            {
    //                throw new InvalidOperationException("No process match!");
    //            }
    //            return QueueGetDto.GetDTO(createdQueue);
    //        }

    //        public async Task<List<QueueProcesses>> GetQueueForStatus(Constants.PROCESS_STATUS processStatus)
    //        {
    //            return await _EngineCoreDBContext.QueueProcesses.Where(d => d.Status == (byte)processStatus).ToListAsync();
    //        }

    //        public async Task<QueueTodayQueueInfo> GetCurrentQueueStatistics(bool onlyMyApp)
    //        {
    //            //MeetingRepository meetingRepository = new MeetingRepository(_EngineCoreDBContext);
    //            //var allTodayQueue = await _EngineCoreDBContext.QueueProcesses.ToListAsync();

    //            //var pendingTickets = allTodayQueue.Where(x => x.Status == (byte)Constants.PROCESS_STATUS.PENDING).ToList();
    //            //var pendingTicketsForMeeting = 0;

    //            //List<QueueOnLineApp> onlineTickets = new List<QueueOnLineApp>();



    //            //foreach (var pend in pendingTickets)
    //            //{
    //            //    var isAttended = await meetingRepository.IsAttendedByAppNo(pend.Id);
    //            //    if (isAttended.IsOnline)
    //            //    {
    //            //        var app = await (
    //            //        from ap in _EngineCoreDBContext.Application
    //            //        join que in _EngineCoreDBContext.QueueProcesses
    //            //        on ap.Id equals que.ProcessNo
    //            //        join usr in _EngineCoreDBContext.User
    //            //        on ap.LastUpdatedBy equals usr.Id

    //            //        where ap.Id == pend.Id
    //            //        select new { UserName = usr.FullName, UserId = usr.Id }
    //            //             ).FirstOrDefaultAsync();

    //            //        pendingTicketsForMeeting++;
    //            //        QueueOnLineApp queueOnLineApp = new QueueOnLineApp
    //            //        {
    //            //            ApplicationId = pend.Id,
    //            //            TicketNo = pend.TicketId,
    //            //            ExpectedDate = pend.ExpectedDateTime,
    //            //            LastUpdateBy = app.UserName,
    //            //            IsLate = isAttended.IsLate,
    //            //            FirstLogin = isAttended.LastLogIn,
    //            //            LastUpdateId = app.UserId
    //            //        };

    //            //        if (isAttended.IsLate)
    //            //        {
    //            //            onlineTickets.Add(queueOnLineApp);
    //            //        }
    //            //        else
    //            //        {
    //            //            if (onlyMyApp)
    //            //            {
    //            //                if (queueOnLineApp.LastUpdateId == _iUserRepository.GetUserID())
    //            //                {
    //            //                    onlineTickets.Add(queueOnLineApp);
    //            //                }
    //            //            }
    //            //            else
    //            //            {
    //            //                onlineTickets.Add(queueOnLineApp);
    //            //            }
    //            //        }
    //            //    }
    //            //}


    //            string myDoneApplications = "0/0";
    //            List<QueueOnLineApp> onlineTickets = new List<QueueOnLineApp>();
    //            try
    //            {
    //                var userId = _iUserRepository.GetUserID();
    //                var interviewStages = await _iAdmServiceRepository.GetInterviewStagesId();


    //                var query = await (from app in _EngineCoreDBContext.Application.Where(x => interviewStages.Contains((int)x.CurrentStageId))
    //                                   join meet in _EngineCoreDBContext.Meeting on app.Id equals meet.OrderNo
    //                                   join MeetingLog in _EngineCoreDBContext.MeetingLogging.Where(x => x.IsModerator == false && x.LoginDate.AddSeconds(300) >= DateTime.Now) on meet.Id equals MeetingLog.MeetingId

    //                                   select new
    //                                   {
    //                                       app.Id,
    //                                       app.LastUpdatedBy,
    //                                       MeetingLog.FirstLogin
    //                                   }).ToListAsync();

    //                onlineTickets = query.GroupBy(y => y.Id).Where(g => g.Any(r => r.LastUpdatedBy == userId)).Select(f => new QueueOnLineApp { ApplicationId = f.Key, IsLate = false }).ToList();
    //                onlineTickets.AddRange(query.GroupBy(y => y.Id).Where(g => g.Any(r => r.FirstLogin.Date == DateTime.Now.Date &&  r.FirstLogin.AddMinutes(15) < DateTime.Now)).Select(f => new QueueOnLineApp { ApplicationId = f.Key, IsLate = true }).ToList());

    //                int notaryApplicationsPerDay = Constants.NOTARY_APPLICATIONS_PER_DAY_DEFAULT;
    //                if (_IConfiguration["NotaryApplicationsPerDay"] == null)
    //                {
    //                    _iLogger.LogInformation("Warning!!! NotaryApplicationsPerDay is missing");
    //                }
    //                else
    //                {
    //                    bool success = int.TryParse(_IConfiguration["NotaryApplicationsPerDay"], out int settingCount);
    //                    if (!success || settingCount < 1)
    //                    {
    //                        _iLogger.LogInformation("Warning NotaryApplicationsPerDay is invalid number or < 1 ");
    //                    }
    //                    else
    //                    {
    //                        notaryApplicationsPerDay = settingCount;
    //                    }
    //                }


    //                var doneApp = await _iStatisticsRepository.GetNotaryTodayDone(_iUserRepository.GetUserID(), interviewStages);

    //                myDoneApplications = doneApp.ToString() + " / " + notaryApplicationsPerDay.ToString();
    //            }
    //            catch (Exception ex)
    //            {
    //                var msg = ex.Message;
    //                if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
    //                {
    //                    msg += " inner is: " + ex.InnerException.Message;
    //                }
    //                _iLogger.LogInformation("Error  GetNotaryTodayDone  " + ex.Message);
    //            }


    //            QueueTodayQueueInfo todayProcesses = new QueueTodayQueueInfo()
    //            {
    //                MyDoneApplications = myDoneApplications,
    //                OnLineTickets = onlineTickets
    //            };
    //            return todayProcesses;
    //        }


    //        public async Task<QueueNextAppDto> GetNextOrder(int askedById)
    //        {

    //            QueueNextAppDto nextApp = new QueueNextAppDto();
    //            try
    //            {
    //                var blackListApps = await _iBlackListRepository.GetBlackListApplications();

    //                MeetingRepository meetingRepository = new MeetingRepository(_EngineCoreDBContext);

    //                //try // refresh employee login only from refresh token.
    //                //{
    //                //    await _iUserRepository.RefreshEmpLogin();
    //                //}
    //                //catch (Exception ex)
    //                //{
    //                //    var msg = " error is : " + ex.Message;
    //                //    if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
    //                //    {
    //                //        msg += "  " + ex.InnerException.Message;
    //                //    }
    //                //    _iLogger.LogInformation("Error in Refresh employee Login  " + msg);
    //                //}


    //                var interviewOrderStages = await _iAdmServiceRepository.GetInterviewStagesId();

    //                int rejectedStateId = await _iSysValueRepository.GetIdByShortcut("REJECTED");
    //                int autoRejectedStateId = await _iSysValueRepository.GetIdByShortcut("AutoCancel");


    //                var interviewApps = await _EngineCoreDBContext.Application.Where(x => x.StateId != rejectedStateId &&
    //                                                                                      x.StateId != autoRejectedStateId &&
    //                                                                                      interviewOrderStages.Contains((int)x.CurrentStageId) &&
    //                                                                                      !blackListApps.Contains(x.Id)
    //                                                                                      ).OrderBy(x => x.CreatedDate).ToListAsync();

    //                List<QueueNextAppDto> lateOrders = new List<QueueNextAppDto>();

    //                foreach (var pend in interviewApps)
    //                {
    //                    var isAttended = await meetingRepository.IsAttendedByAppNo(pend.Id);
    //                    if (isAttended.IsOnline)
    //                    {
    //                        try
    //                        {
    //                            var appTran = await _iTransactionRepository.GetByAppId(pend.Id);
    //                            if (appTran.Count < 1)
    //                            {
    //                                _iLogger.LogInformation("GetNextOrder fault configuration for application at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " !!!!!!!! no transaction for application " + pend.Id.ToString());
    //                                continue;
    //                            }


    //                            bool locked = true;

    //                            var signResult = await _iApplicationPartyRepository.IsSignEditByAnotherUser((int)appTran[0].Id, pend.Id, askedById);
    //                            locked = signResult.Result == Constants.AppStatus.LOCKED;

    //                            bool release = (DateTime.Now.Subtract((DateTime)pend.LastReadDate).TotalSeconds > Constants.LOCK_SECONDS_TIME) && (!locked);

    //                            // new App,                      done by me and free
    //                            if (pend.LastReadDate == null || (pend.LastUpdatedBy == askedById && release))
    //                            {
    //                                nextApp.ApplicationId = pend.Id;
    //                                nextApp.ServiceId = (int)pend.ServiceId;

    //                                // start process.
    //                                await ChangeTicketsStatusToInProgressByProcessNo(askedById, pend.Id);
    //                                _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned online application for me  is " + nextApp.ApplicationId.ToString());
    //                                return nextApp;
    //                            }

    //                            if (isAttended.IsLate && release)
    //                            {
    //                                QueueNextAppDto nextApp1 = new QueueNextAppDto
    //                                {
    //                                    ApplicationId = pend.Id,
    //                                    ServiceId = (int)pend.ServiceId
    //                                };
    //                                lateOrders.Add(nextApp1);
    //                            }
    //                            else if (isAttended.IsLate && locked)
    //                            {
    //                                _iLogger.LogInformation(" Warning!!: GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " for the application " + pend.Id.ToString() + " it is late but it is not return because it is locked contains signatures. ");
    //                            }
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            _iLogger.LogInformation("GetNextOrder fault configuration for locked at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " !!!!!!!! in IsSignEditByAnotherUser, error is " + ex.Message);
    //                            continue;
    //                        }
    //                    }
    //                }

    //                // no online for me in interview, check if online and late not to me.
    //                if (lateOrders.Count > 0)
    //                {
    //                    // start process.
    //                    await ChangeTicketsStatusToInProgressByProcessNo(askedById, lateOrders[0].ApplicationId);
    //                    _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned online application for other is " + lateOrders[0].ApplicationId.ToString());
    //                    return lateOrders[0];
    //                }

    //                // get Occupied application if exist.
    //                var applicationOcc = await _EngineCoreDBContext.Application.Where(x => x.Owner == askedById).FirstOrDefaultAsync();
    //                if (applicationOcc != null)
    //                {
    //                    nextApp.ApplicationId = applicationOcc.Id;
    //                    nextApp.ServiceId = (int)applicationOcc.ServiceId;
    //                    _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned occupied application for  is " + applicationOcc.Id.ToString());
    //                    return nextApp;
    //                }

    //                // no Application in interview stage get from reviewer stage.
    //                var reviewedStages = await _iAdmServiceRepository.GetReviewStagesId();
    //                var skippApps = await _EngineCoreDBContext.SkippApps.Where(x => x.UserId == askedById).Select(z=>z.AppId).ToListAsync();

    //                if (reviewedStages.Count == 0)
    //                {
    //                    _iLogger.LogInformation("GetNextOrder is wrong configuration at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " reviedStages is empty ");
    //                }

    //                var reviewApps = await _EngineCoreDBContext.Application.Where(x => x.StateId != rejectedStateId &&
    //                                                                              reviewedStages.Contains((int)x.CurrentStageId) &&
    //                                                                              !blackListApps.Contains(x.Id) &&
    //                                                                              !skippApps.Contains(x.Id)&&
    //                                                                              x.Owner == null
    //                                                                              ).OrderBy(x => x.CreatedDate).ToListAsync();
    //                foreach (var reviewApp in reviewApps)
    //                {
    //                    if (reviewApp.LastReader == askedById || reviewApp.LastReadDate == null || (DateTime.Now.Subtract((DateTime)reviewApp.LastReadDate).TotalSeconds > Constants.LOCK_SECONDS_TIME))
    //                    {
    //                        nextApp.ApplicationId = reviewApp.Id;
    //                        nextApp.ServiceId = (int)reviewApp.ServiceId;
    //                        _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned application for review is " + nextApp.ApplicationId.ToString());

    //                        return nextApp;
    //                    }
    //                }

    //                _iLogger.LogInformation("GetNextOrder is Not find any result !!! at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " application count in review is " + reviewApps.Count.ToString());

    //            }
    //            catch (Exception ex)
    //            {
    //                var msg = " error is : " + ex.Message;
    //                if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
    //                {
    //                    msg += "  " + ex.InnerException.Message;
    //                }
    //                _exception.AttributeMessages.Add("Failed in get Next Order number, call the admin please." + msg);
    //                _iLogger.LogInformation("Error General Exception in getNextOrder " + msg);

    //                throw _exception;
    //            }
    //            return nextApp;
    //        }

    //        public async Task<PickTicket> PickTicket(int processNo, int serviceKindNo, int userId, DateTime expectedDateStartFrom, bool bookTicket)
    //        {
    //            if (expectedDateStartFrom.AddMinutes(10) < DateTime.Now)
    //            {
    //                throw new InvalidOperationException("You can't pick a ticket for previous date");
    //            }

    //            bool availableDay = false;

    //            var serviceKind = _EngineCoreDBContext.ServiceKind.Where(x => x.Id == serviceKindNo).SingleOrDefault();
    //            if (serviceKind == null)
    //            {
    //                throw new InvalidOperationException("There is no configuration for the chosen service kind.");
    //            }

    //            int employeeCount = serviceKind.EmployeeCount;
    //            int minutesPerProcess = serviceKind.EstimatedTimePerProcess;
    //            string symbol = serviceKind.Symbol;

    //            // result information.
    //            PickTicket picTicket = new PickTicket();

    //            while (!availableDay)
    //            {

    //                if (expectedDateStartFrom.Date > DateTime.Now.AddDays(Constants.MAX_QUEUE_DAYS).Date)
    //                {
    //                    throw new InvalidOperationException("There is no possibility to pick a ticket after " + Constants.MAX_QUEUE_DAYS.ToString());
    //                }

    //                while (await _iGlobalDayOffRepository.IsDayOff(expectedDateStartFrom))
    //                {
    //                    // the day is a holiday, pick a ticket at another day.
    //                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
    //                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
    //                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
    //                }

    //                Dictionary<int, int> dic = new Dictionary<int, int>();

    //                List<WorkingTimeGetDto> workingHours = new List<WorkingTimeGetDto>();
    //                workingHours = await _iWorkingTimeRepository.GetWorkingForDate(expectedDateStartFrom.Date);

    //                // get the number of working hours.
    //                int workDayInMinutes = 0;
    //                foreach (var work in workingHours)
    //                {
    //                    dic.Add(work.StartFrom, work.FinishAt);
    //                    workDayInMinutes += work.FinishAt - work.StartFrom;
    //                }
    //                dic.OrderBy(k => k.Key);

    //                if (workDayInMinutes == 0)
    //                {
    //                    // no dayOff and working hours for this day, it is a holiday.
    //                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
    //                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
    //                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
    //                    continue;
    //                }


    //                // get non executed processes.
    //                var unExecutedProcesses = await _EngineCoreDBContext.QueueProcesses.Where(x => x.ExpectedDateTime.Date == expectedDateStartFrom.Date && x.Status != (int)Constants.PROCESS_STATUS.FINISHED && x.ServiceKindNo == serviceKindNo && x.ProcessNo != processNo).ToListAsync();
    //                int countOfUnExecutedProcesses = unExecutedProcesses.Count();

    //                int reservedTime = countOfUnExecutedProcesses / employeeCount * minutesPerProcess;

    //                if (reservedTime > workDayInMinutes - minutesPerProcess)
    //                {
    //                    // all the day is booked up.
    //                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
    //                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
    //                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
    //                    continue;
    //                }

    //                int askedMin = expectedDateStartFrom.Hour * 60 + expectedDateStartFrom.Minute;
    //                // if the picked date is currently (during the work) and no more processes available or after the working hours for the chosen day.
    //                if ((askedMin + reservedTime + minutesPerProcess) >= dic.Values.Last())
    //                {
    //                    // the expected time is after the working hours for the chosen day.
    //                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
    //                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
    //                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
    //                    continue;
    //                }

    //                // calculate estimated time.
    //                foreach (var element in dic)
    //                {
    //                    if (element.Value - element.Key >= reservedTime + minutesPerProcess)
    //                    {
    //                        if (askedMin == 0)
    //                        {
    //                            expectedDateStartFrom = expectedDateStartFrom.AddMinutes(element.Key + reservedTime);
    //                            break;
    //                        }
    //                        else
    //                        {
    //                            if (askedMin <= element.Key)
    //                            {
    //                                TimeSpan resetTime = new TimeSpan(element.Key / 60 + reservedTime / 60, element.Key % 60 + reservedTime % 60, 0);
    //                                expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
    //                                break;
    //                            }

    //                            if (askedMin <= element.Value && askedMin >= element.Key)
    //                            {
    //                                expectedDateStartFrom = expectedDateStartFrom.AddMinutes(reservedTime);
    //                                break;
    //                            }

    //                            reservedTime = Math.Max(reservedTime - (element.Value - element.Key), 0);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        reservedTime = Math.Max(reservedTime - (element.Value - element.Key), 0);
    //                    }
    //                }

    //                picTicket.ExpectDateTime = expectedDateStartFrom;
    //                picTicket.WorkingHours = dic;

    //                // validate the final expect time if in the working hours, should not reach.
    //                int expectedMin = picTicket.ExpectDateTime.Hour * 60 + picTicket.ExpectDateTime.Minute;
    //                bool wrongExpect = true;
    //                foreach (var element in dic)
    //                {
    //                    if (expectedMin >= element.Key || expectedMin <= element.Value)
    //                    {
    //                        wrongExpect = false;
    //                    }
    //                }
    //                if (wrongExpect)
    //                {
    //                    throw new InvalidOperationException("Wrong in the calculation of the estimated time.");
    //                }

    //                if (bookTicket)
    //                {
    //                    QueueProcesses proc = _EngineCoreDBContext.QueueProcesses.Where(x => x.ProcessNo == processNo && x.ServiceKindNo == serviceKindNo).FirstOrDefault();
    //                    int previousTicketsCount = _EngineCoreDBContext.QueueProcesses.Where(x => x.ExpectedDateTime.Date == expectedDateStartFrom.Date && x.ServiceKindNo == serviceKindNo).ToList().Count;

    //                    if (proc == null)
    //                    {
    //                        // add a new process to the queue.
    //                        QueueProcesses newProcess = new QueueProcesses
    //                        {
    //                            ServiceKindNo = serviceKindNo,
    //                            ExpectedDateTime = expectedDateStartFrom,
    //                            Status = (byte)Constants.PROCESS_STATUS.PENDING,
    //                            ProcessNo = processNo,
    //                            TicketId = previousTicketsCount + 1,
    //                            CreatedDate = DateTime.Now,
    //                            NotifyLowLevel = false,
    //                            NotifyMediumLevel = false,
    //                            NotifyHighLevel = false

    //                        };

    //                        if (userId != 0)
    //                        {
    //                            newProcess.CreatedBy = userId;
    //                        }
    //                        _EngineCoreDBContext.QueueProcesses.Add(newProcess);
    //                        try { await _EngineCoreDBContext.SaveChangesAsync(); }
    //                        catch (Exception e)
    //                        {
    //                            var s = e.Message.ToString();
    //                            var d = e.InnerException.ToString();
    //                        }

    //                        picTicket.TicketId = symbol + Convert.ToString(newProcess.TicketId);
    //                    }
    //                    else
    //                    {
    //                        // update the appointment.
    //                        string note = "change no from " + proc.TicketId.ToString() + " to " + (previousTicketsCount + 1).ToString();
    //                        QueuePostDto updatedQueue = new QueuePostDto
    //                        {
    //                            ServiceKindNo = proc.ServiceKindNo,
    //                            Status = (byte)Constants.PROCESS_STATUS.PENDING,
    //                            ExpectedDate = expectedDateStartFrom,
    //                            ProcessNo = proc.ProcessNo,
    //                            TicketId = previousTicketsCount + 1,
    //                            Note = (proc.Note != null && proc.Note.Length < 100) ? proc.Note + note : note,
    //                            CreatedBy = proc.CreatedBy,
    //                            CreatedDate = proc.CreatedDate,
    //                            UpdatedDate = DateTime.Now,
    //                            NotifyHighLevel = false,
    //                            NotifyMediumLevel = false,
    //                            NotifyLowLevel = false,                         
    //                        };

    //                        if (userId != 0)
    //                        {
    //                            updatedQueue.UpdatedBy = userId;
    //                        }
    //                        await UpdateQueueProcess(proc.Id, updatedQueue);
    //                        picTicket.TicketId = symbol + Convert.ToString(proc.TicketId);
    //                    }
    //                }
    //                availableDay = true;
    //            }

    //            return picTicket;
    //        }

    //        public async Task<bool> ChangeTicketsStatusToInProgressByProcessNo(int userId, int processNo)
    //        {
    //            try
    //            {
    //                QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
    //                if (originalQueue == null || originalQueue.Status == (byte)Constants.PROCESS_STATUS.INPROGRESS || originalQueue.Status == (byte)Constants.PROCESS_STATUS.FINISHED)
    //                {
    //                    return true;
    //                }

    //                originalQueue.Status = (byte)Constants.PROCESS_STATUS.INPROGRESS;
    //                originalQueue.LastUpdatedDate = DateTime.Now;
    //                originalQueue.LastUpdatedBy = userId;
    //                _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
    //                await _EngineCoreDBContext.SaveChangesAsync();
    //                return true;
    //            }
    //            catch
    //            {
    //                return false;
    //            }
    //        }

    //        public async Task<bool> ChangeTicketStatusBackToPendingByProcessNo(int userId, int processNo)
    //        {
    //            QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
    //            if (originalQueue == null || originalQueue.Status == (byte)Constants.PROCESS_STATUS.PENDING || originalQueue.Status == (byte)Constants.PROCESS_STATUS.FINISHED)
    //            {
    //                return false;
    //            }

    //            originalQueue.Status = (byte)Constants.PROCESS_STATUS.PENDING;
    //            originalQueue.LastUpdatedDate = DateTime.Now;
    //            originalQueue.LastUpdatedBy = userId;
    //            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
    //            await _EngineCoreDBContext.SaveChangesAsync();
    //            return true;
    //        }

    //        public async Task<bool> ChangeTicketStatusToDone(int userId, int processNo)
    //        {
    //            var originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
    //            if (originalQueue == null)
    //            {
    //                return false;
    //            }

    //            originalQueue.Status = (byte)Constants.PROCESS_STATUS.FINISHED;
    //            originalQueue.LastUpdatedDate = DateTime.Now;
    //            originalQueue.LastUpdatedBy = userId;
    //            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
    //            await _EngineCoreDBContext.SaveChangesAsync();
    //            return true;
    //        }

    //    }
}
