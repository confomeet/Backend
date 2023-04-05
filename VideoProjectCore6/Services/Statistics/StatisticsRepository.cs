using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IStatisticsRepository;
using VideoProjectCore6.Repositories.IUserRepository;
#nullable disable
namespace VideoProjectCore6.Services.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IUserRepository _IUserRepository;
        private readonly IEventRepository _IEventRepository;
        private readonly IConfEventRepository _IConfEventRepository;


        private readonly IGeneralRepository _IGeneralRepository;


        public StatisticsRepository(IGeneralRepository iGeneralRepository, IUserRepository iUserRepository, IConfEventRepository iConfEventRepository, IEventRepository iEventRepository, OraDbContext dbContext)
        {
            _IUserRepository = iUserRepository;
            _DbContext = dbContext;
            _IConfEventRepository = iConfEventRepository;
            _IEventRepository = iEventRepository;
            _IGeneralRepository = iGeneralRepository;

        }

        public async Task<List<ValueIdDesc>> EventsByApp(DateTimeRange range, string lang)
        {
            var eventSum = await _DbContext.Events
                .Where(e => e.MeetingId != null && e.AppId != null && e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date)
                .GroupBy(g => g.AppId)
                .Select(g => new ValueIdDesc { Id = (int)g.Key, Value = g.Count() }).ToListAsync();
            if (eventSum.Count > 0)
            {
                string v;
                var appName = await _DbContext.ClientInfos.Where(c => eventSum.Select(e => e.Id).ToList().Contains(c.Id)).ToDictionaryAsync(c => c.Id, c => c.ClientName);

                foreach (var app in eventSum)
                {
                    app.Description = appName.TryGetValue((ushort)app.Id, out v) ? v : string.Empty;
                }
            }
            return eventSum;
        }



        public async Task<List<ValueIdDesc>> ActiveRooms(DateTimeRange range, short? EventType ,string lang)
        {

            //var events = await _DbContext.Events.ToListAsync();


            var events = await _DbContext.Events.Select(s => new
            {
                s.StartDate,
                s.EndDate,
                s.Type,
                s.Id,
                s.MeetingId,
                s.CreatedDate
            }).Where(e => EventType != null ?
                ((e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date) && EventType == e.Type)

                : (e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date)

                ).AsNoTracking().ToListAsync();


            var allRooms = await _DbContext.ConfEvents.Where(e => (e.EventTime > range.StartDateTime.Date && e.EventTime < range.EndDateTime.AddDays(1).Date)).ToListAsync();


                List<ValueIdDesc> listOfStatus = new List<ValueIdDesc>();


                Dictionary<string, int> map = new Dictionary<string, int>
                {
                    {"activeRoomsNum" , 0 },
                    {"incomingRoomNum" , 0 },
                    {"finishedRoomNum" , 0 },
                    {"outsideRoomNum" , 0 },
                    {"allMeetingsTime", 0 },
                    {"succeededRecordingNum", 0 },
                    {"allEvents" , events.Count },
                };

                foreach (var ev in events)
                {
                    var eventStatus = _IGeneralRepository.CheckStatus(ev.StartDate, ev.EndDate, ev.Id, ev.MeetingId, lang, allRooms);

                    if(eventStatus.Status == 1)
                    {
                        map["activeRoomsNum"] += 1;
                    }
                    else if(eventStatus.Status == 3)
                    {
                        map["incomingRoomNum"] += 1;

                    } else if(eventStatus.Status == 2)
                    {
                        map["finishedRoomNum"] += 1;
                    }
                }


              map["outsideRoomNum"] = Math.Abs(AnyAny(allRooms.Select(o => o.ConfId).ToHashSet(), events.Select(i => i.MeetingId).ToHashSet()));

              map["allMeetingsTime"] = CalculateAllMeetingsTime(range, allRooms);

              map["succeededRecordingNum"] = NumOfSucceededRecordings();

                for (int i = 0; i < map.Count; i++)
              {

                listOfStatus.Add(new ValueIdDesc
                {
                    Description = map.ElementAt(i).Key.Equals("activeRoomsNum") ? lang.Equals("ar") ? "اتصالات نشطة" : "Active Rooms" 
                    
                    : map.ElementAt(i).Key.Equals("incomingRoomNum") ? lang.Equals("ar") ? "اتصالات قادمة" : "Incoming Meetings"
                    : map.ElementAt(i).Key.Equals("finishedRoomNum") ? lang.Equals("ar") ? "الاتصالات المنتهية" : "Finished Meetings" 
                    : map.ElementAt(i).Key.Equals("outsideRoomNum") ? lang.Equals("ar") ? "عدد الاتصالات خارج النظام" : "Meetings outside the system"
                    : map.ElementAt(i).Key.Equals("allMeetingsTime") ? lang.Equals("ar") ? "مدة جميع الاتصالات" : "All Rooms Time"
                    : map.ElementAt(i).Key.Equals("succeededRecordingNum") ? lang.Equals("ar") ? "عدد التسجيلات الناجحة" : "Number of succeeded recordings"
                    : map.ElementAt(i).Key.Equals("allEvents") ? lang.Equals("ar") ? "كل الاجتماعات" : "All Rooms" : "",

                    Id = i,
                    Value = map.ElementAt(i).Value
                });
              }
               
             return listOfStatus;
        }


        

        // OUTPUT EXAMPLE:
        //result=[ 
        //     {
        //meetingId,
        //topic,
        //allParticipants,
        //onlineParticipants
        //     },

        //     ]

        public async Task<ListCount> UsersByStatus(DateTimeRange range, string lang)
        {
            var events = await _DbContext.Events.Include(x => x.Participants).Where(e => e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date).ToListAsync();

            List<EventActiveUsers> listActiveUser = new List<EventActiveUsers>();

            
            foreach(var ev in events)
            {

                var singleRoomActiveUsers = await _IConfEventRepository.handleGetRoom(range, ev.Id.ToString(), ev.MeetingId, lang);

                EventActiveUsers eventActiveUsers = new EventActiveUsers
                {
                    MeetingId = ev.MeetingId,
                    Topic = ev.Topic,
                    AllParticipants = ev.Participants.Count,
                    OnlineParticipants = (singleRoomActiveUsers == null 
                    || singleRoomActiveUsers.Id == -1)  ? 0 : singleRoomActiveUsers.Result.Count
                };


                if(eventActiveUsers.OnlineParticipants > 0)
                {
                    listActiveUser.Add(eventActiveUsers);
                }
            }


            return new ListCount
            {
                Count = listActiveUser.Count,
                Items = listActiveUser
            };

        }

        public async Task<EventStatOfMeetings> IncomingEvents(DateTime startDate, DateTime endDate)
        {
            return await _IEventRepository.GetNumOfFutureMeetings(startDate, endDate);
        }

        public async Task<EventStatOfMeetings> FinishedEvents(DateTime startDate, DateTime endDate)
        {
            return await _IEventRepository.GetNumOfMeetingsDone(startDate, endDate);
        }

        int AnyAny(HashSet<string> A, HashSet<string> B)
        {
            int count = 0;
            //foreach (string i in A)
            //    if (B.Any(b => !b.Equals(i)))
            //    {
            //        count += 1;
            //    }

            int inter = A.Intersect(B).Count();


            return A.Count() - inter;
        }

        int CalculateAllMeetingsTime(DateTimeRange range, List<ConfEvent> allRooms)
        {
            TimeSpan ts = new TimeSpan();


            var rooms = allRooms.Where(e => (e.EventTime > range.StartDateTime.Date && e.EventTime < range.EndDateTime.AddDays(1).Date)).ToList();

            foreach (var singleEvent in rooms)
            {

                if(singleEvent.EventType == 2)
                {
                    var eventStart = allRooms.Where(x => x.EventType == 1 
                    && x.MeetingId == singleEvent.MeetingId 
                    && x.ConfId.Equals(singleEvent.ConfId)).FirstOrDefault();
                    

                    if (eventStart != null)
                    {
                        ts += (singleEvent.EventTime - eventStart.EventTime);
                    }

                }
            }
            return (int) Math.Round(ts.TotalHours);
        }

         int NumOfSucceededRecordings()
        {
            var allRecordings =  _DbContext.RecordingLogs.ToList();

            var recordingsUnknowStatus = allRecordings.Where(x => x.IsSucceeded == 0).Select(e => e.RecordingfileName.Split('_', System.StringSplitOptions.None)[0]).ToList();

            var succeededRecordings = allRecordings.Where(x => x.IsSucceeded == 1).Select(e => e.RecordingfileName.Split('_', System.StringSplitOptions.None)[0]).ToList();

            return succeededRecordings.Where(n => recordingsUnknowStatus.Contains(n)).ToList().Count();
        }
    }
}
