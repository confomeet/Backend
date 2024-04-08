﻿using System.Diagnostics;
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

        public async Task<List<ValueIdDesc>> EventsByApp(DateTimeRange range, string lang)
        {
            var eventSum = await _DbContext.Events
                .Where(e => e.MeetingId != null && e.AppId != null && e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date)
                .GroupBy(g => g.AppId)
                .Select(g => new ValueIdDesc { Id = g.Key ?? 0, Value = g.Count() }).ToListAsync();
            if (eventSum.Count > 0)
            {
                var appName = await _DbContext.ClientInfos.Where(c => eventSum.Select(e => e.Id).ToList().Contains(c.Id)).ToDictionaryAsync(c => c.Id, c => c.ClientName);

                foreach (var app in eventSum)
                {
                    app.Description = appName.TryGetValue((ushort)app.Id, out string? v) ? v : string.Empty;
                }
            }
            return eventSum;
        }

        public async Task<List<ValueIdDesc>> ActiveRooms(DateTimeRange range)
        {
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
                _logger.LogInformation("LastEvent: {} {}  {}", ev.MeetingId, ev.MeetingStartTime, ev.EventType);
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

        public async Task<ListCount> UsersByStatus(DateTimeRange range, string lang)
        {
            // var events = await _DbContext.Events.Include(x => x.Participants).Where(e => e.CreatedDate > range.StartDateTime.Date && e.CreatedDate < range.EndDateTime.AddDays(1).Date).ToListAsync();

            // List<EventActiveUsers> listActiveUser = new List<EventActiveUsers>();

            
            // foreach(var ev in events)
            // {

            //     var singleRoomActiveUsers = await _IConfEventRepository.handleGetRoom(range, ev.Id.ToString(), ev.MeetingId, lang);

            //     EventActiveUsers eventActiveUsers = new EventActiveUsers
            //     {
            //         MeetingId = ev.MeetingId,
            //         Topic = ev.Topic,
            //         AllParticipants = ev.Participants.Count,
            //         OnlineParticipants = (singleRoomActiveUsers == null 
            //         || singleRoomActiveUsers.Id == -1)  ? 0 : singleRoomActiveUsers.Result.Count
            //     };


            //     if(eventActiveUsers.OnlineParticipants > 0)
            //     {
            //         listActiveUser.Add(eventActiveUsers);
            //     }
            // }

            List<EventActiveUsers> items =
            [
                new() {MeetingId = "10", Topic = "topic10", AllParticipants = 15, OnlineParticipants = 2},
                new() {MeetingId = "20", Topic = "topic20", AllParticipants = 5, OnlineParticipants = 4},
                new() {MeetingId = "30", Topic = "topic30", AllParticipants = 10, OnlineParticipants = 10},

                new() {MeetingId = "40", Topic = "topic40", AllParticipants = 7, OnlineParticipants = 6},
                new() {MeetingId = "50", Topic = "topic50", AllParticipants = 3, OnlineParticipants = 1},
                new() {MeetingId = "60", Topic = "topic60", AllParticipants = 5, OnlineParticipants = 3},


                new() {MeetingId = "70", Topic = "topic70", AllParticipants = 5, OnlineParticipants = 1},
                new() {MeetingId = "80", Topic = "topic80", AllParticipants = 5, OnlineParticipants = 3},
                new() {MeetingId = "90", Topic = "topic90", AllParticipants = 11, OnlineParticipants = 7},
            ];

            return new ListCount
            {
                Count = items.Count,
                Items = items
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

        public class RelevantConfEvent
        {
            public DateTime MeetingStartTime;
            public string MeetingId = string.Empty;
            public Constants.EVENT_TYPE? EventType;
            public DateTime? EventTime;
        };
    }
}
