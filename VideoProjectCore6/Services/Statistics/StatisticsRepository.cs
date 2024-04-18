using System.Diagnostics;
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

namespace VideoProjectCore6.Services.Statistics
{
    public class StatisticsRepository(
            IGeneralRepository iGeneralRepository,
            IUserRepository iUserRepository,
            IConfEventRepository iConfEventRepository,
            IEventRepository iEventRepository,
            OraDbContext dbContext,
            ILogger<StatisticsRepository> logger
    ) : IStatisticsRepository
    {
        private readonly OraDbContext _DbContext = dbContext;
        private readonly IUserRepository _IUserRepository = iUserRepository;
        private readonly IEventRepository _IEventRepository = iEventRepository;
        private readonly IConfEventRepository _IConfEventRepository = iConfEventRepository;
        private readonly ILogger _logger = logger;


        private readonly IGeneralRepository _IGeneralRepository = iGeneralRepository;

        public async Task<List<ValueIdDesc>> ByApp(DateTimeRange range)
        {
            // TODO: need to store app_id somewhere and perform some group by here.
            // Probably the best way is to have JWT with app_id and app_display_name encoded in the token.
            // And also we need some table where we can store info about event creation.
            // We must not make app_id part of Event model.
            return await Task.FromResult(new List<ValueIdDesc>());
        }

        public async Task<List<ValueIdDesc>> ByMeetingStatus(DateTimeRange range)
        {
            if (range.EndDateTime < range.StartDateTime)
                return [];

            var now = DateTime.UtcNow;

            // We likely can hold 10^7 records of RelevangConfEvent in memory
            // 10^6 conferenes over 10 year means there are 2730 conferneces every day.
            // Reaching such high number of conferences seems to be quite far perspective.
            const int ConfEventsLimit = 10 * 1000 * 1000;
            var relevantEvents = await (
                from event_ in _DbContext.Events.AsNoTracking()
                join confEvent in _DbContext.ConfEvents.AsNoTracking()
                    on event_.MeetingId equals confEvent.MeetingId into allEvents
                from joinedEvent in allEvents.DefaultIfEmpty()
                orderby joinedEvent.EventTime ascending
                where
                    (event_.EndDate > range.StartDateTime && event_.StartDate < range.EndDateTime) &&
                    (joinedEvent.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED ||
                     joinedEvent.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED ||
                     joinedEvent.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_CREATED ||
                     joinedEvent == null
                    )
                select new RelevantConfEvent
                {
                    MeetingId = event_.MeetingId,
                    EventType = joinedEvent.EventType,
                    EventTime = joinedEvent.EventTime,
                    MeetingStartTime = event_.StartDate,
                })
                .Take(ConfEventsLimit)
                .ToListAsync();

            if (relevantEvents.Count == ConfEventsLimit) {
                _logger.LogCritical("Module with statistics reached builtin limitations.");
            }

            var lastEvents = (
                from ev in relevantEvents
                group ev by new { ev.MeetingId, ev.MeetingStartTime } into meetingEvents
                select new RelevantConfEvent
                {
                    MeetingId = meetingEvents.Key.MeetingId,
                    MeetingStartTime = meetingEvents.Key.MeetingStartTime,
                    EventType = meetingEvents.OrderByDescending(e => e.EventTime!).FirstOrDefault()?.EventType ?? null,
                    EventTime = meetingEvents.OrderByDescending(e => e.EventTime!).FirstOrDefault()?.EventTime ?? null,
                })
            .ToList();  // relevant events are already loaded in memory

            int ActiveMeetingsNum = 0;
            int UpcomingMeetingsNum = 0;
            int FinishedMeetingsNum = 0;
            foreach (var ev in lastEvents)
            {
                if (ev.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED)
                    ActiveMeetingsNum += 1;
                else if (ev.EventType == null && ev.MeetingStartTime > now)
                    UpcomingMeetingsNum += 1;
                else if (ev.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED)
                    FinishedMeetingsNum += 1;
            }

            List<ValueIdDesc> result =
            [
                new ValueIdDesc() { Description = "ActiveMeetingsNum", Id = 0, Value = ActiveMeetingsNum },
                new ValueIdDesc() { Description = "UpcomingMeetingsNum", Id = 1, Value = UpcomingMeetingsNum },
                new ValueIdDesc() { Description = "FinishedMeetingsNum", Id = 2, Value = FinishedMeetingsNum },
                new ValueIdDesc() { Description = "AllMeetingsTime", Id = 3, Value = CalculateAllMeetingsTime(relevantEvents) },
                new ValueIdDesc() { Description = "RecordedMeetingsNum", Id = 4, Value = await NumOfSucceededRecordings(lastEvents) },
                new ValueIdDesc() { Description = "AllMeetingsNum", Id = 5, Value = lastEvents.Count },
            ];
            return result;
        }

        public async Task<ListCount> ByOnlineUsers(DateTimeRange range)
        {
            if (range.EndDateTime < range.StartDateTime)
            {
                return new ListCount
                {
                    Count = 0,
                    Items = new List<object>()
                };
            }

            var events = await _DbContext.Events.Include(x => x.Participants).Where(e => e.StartDate <= range.EndDateTime && e.EndDate >= range.EndDateTime).ToListAsync();

            List<EventActiveUsers> listActiveUser = [];

            foreach(var ev in events)
            {

                var singleRoomActiveUsers = await _IConfEventRepository.HandleGetRoom(range, ev.Id.ToString(), ev.MeetingId);

                EventActiveUsers eventActiveUsers = new()
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

        // sortedEvents is sorted by event time ascending
        static int CalculateAllMeetingsTime(List<RelevantConfEvent> sortedEvents)
        {
            TimeSpan ts = new();

            foreach (var singleEvent in sortedEvents)
            {
                if (singleEvent.EventTime == null)
                    continue;
                DateTime endTime = singleEvent.EventTime.Value;
                if (singleEvent.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED)
                {
                    RelevantConfEvent? eventStart = sortedEvents
                        .Where(x =>
                            x.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED
                            && x.MeetingId == singleEvent.MeetingId
                            && x.EventTime < singleEvent.EventTime
                        ).OrderByDescending(x => x.EventTime)
                        .FirstOrDefault();
                    DateTime startTime = eventStart?.EventTime!.Value ?? DateTime.Now;

                    ts += endTime - startTime;
                }
            }
            return (int) Math.Round(ts.TotalHours);
        }

        // Expecting that relevant confs only include one conf per row.
        private async Task<int> NumOfSucceededRecordings(List<RelevantConfEvent> relevantConfs)
        {
            var allRecordings = await _DbContext.RecordingLogs
                .Where(x => x.Status == RecordingStatus.Recorded || x.Status == RecordingStatus.Uploaded)
                .ToListAsync();

            int res = 0;
            foreach (var rec in allRecordings) {
                var meetingId = rec.RecordingfileName.Split('_')[0];
                if (relevantConfs.Any(conf => meetingId == conf.MeetingId))
                    ++res;
            }

            return res;
        }

        private class RelevantConfEvent
        {
            public DateTime MeetingStartTime;
            public string MeetingId = string.Empty;
            public Constants.EVENT_TYPE? EventType;
            public DateTime? EventTime;
        };
    }
}
