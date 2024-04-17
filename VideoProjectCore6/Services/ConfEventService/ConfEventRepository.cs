using Flurl;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Hubs;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Utility;
using static VideoProjectCore6.Services.Constants;

namespace VideoProjectCore6.Services.ConfEventService
{
    public class ConfEventRepository(IGeneralRepository GeneralRepository, OraDbContext dbContext, IConfiguration configuration, ILogger<ConfEventRepository> logger) : IConfEventRepository
    {

        private readonly OraDbContext _DbContext = dbContext;

        private readonly IConfiguration _IConfiguation = configuration;

        private readonly IGeneralRepository _IGeneralRepository = GeneralRepository;
        private readonly ILogger<ConfEventRepository> _ILogger = logger;

        public async Task<APIResult> AddProsodyEvent(ProsodyEventPostDto prosodyEventPostDto, IHubContext<EventHub> HubContext)
        {
            APIResult result = new();

            DateTime dateTime = DateTime.UtcNow;

            try
            {

                ConfEvent? confEvent = await ToConferenceEvent(prosodyEventPostDto, dateTime);

                if (confEvent == null)
                {
                    return result.FailMe(-1,  "NoMatchingRecord");
                }
                if(UserHandler.ConnectedIds.Count > 0)
                {
                    foreach(var connectedId in UserHandler.ConnectedIds)
                    {
                        var userEvents = await GetUserEventsStatus(Int32.Parse(connectedId), confEvent);

                        if (userEvents.Result.Count > 0)
                        {
                            await HubContext.Clients.User(connectedId).SendAsync("NotifyEventStatus", userEvents);
                        }
                    }
                }

                await _DbContext.AddAsync(confEvent);
                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Ok", true, APIResult.RESPONSE_CODE.OK, confEvent);

            }

            catch (Exception)
            {
                return result.FailMe(-1, "failedAdd");
            }

        }

        private async Task<ConfEvent?> ToConferenceEvent(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {

            ConfEvent? confEvent = null;

            if (prosodyEventPostDto.type.Equals(Constants.PROSODY_EVENT_ROOM_CREATED))
            {
                confEvent = ProcessRoomCreatedEvent(prosodyEventPostDto, dateTime);
            }

            else if (prosodyEventPostDto.type.Equals(Constants.PROSODY_EVENT_OCCUPANT_JOINED))
            {
                confEvent = await ProcessOccupantJoinedEvent(prosodyEventPostDto, dateTime);
            }

            else if (prosodyEventPostDto.type.Equals(Constants.PROSODY_EVENT_OCCUPANT_LEAVING))
            {
                confEvent = await ProcessOccupantLeavingEvent(prosodyEventPostDto, dateTime);
            }

            else if (prosodyEventPostDto.type.Equals(Constants.PROSODY_EVENT_ROOM_DESTROYED) || prosodyEventPostDto.type.Equals(Constants.PROSODY_EVENT_ROOM_FINISHED))
            {
                confEvent = await ProcessRoomDestroyed(prosodyEventPostDto, dateTime);
            }

            else if (prosodyEventPostDto.type.Equals(PROSODY_EVENT_USER_LEAVING_LOBBY))
            {
                confEvent = await processOccupantLeavingLobbyEvent(prosodyEventPostDto, dateTime);
            }

            return confEvent;

        }

        private async Task<ConfEvent?> ProcessRoomDestroyed(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {
            try
            {
                ConfEvent res = new()
                {
                    EventType = Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED,
                    MeetingId = prosodyEventPostDto.meetingId,
                    EventTime = dateTime
                };


                List<ConfUser> users = await _DbContext.ConfUsers.Where(c =>
                c.ProsodyId.Equals(prosodyEventPostDto.from)).OrderByDescending(item => item.ConfTime).ToListAsync();

                string userId = prosodyEventPostDto.from;

                if (users != null && users.Count > 0)
                {
                    userId = users.First().ConfId;
                }

                res.UserId = userId;
                res.EventInfo = prosodyEventPostDto.message;

                return res;

            }

            catch
            {


            }

            return null;
        }

        private async Task<ConfEvent?> ProcessOccupantLeavingEvent(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {

            try
            {
                ConfEvent res = new()
                {
                    EventType = Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE,
                    MeetingId = prosodyEventPostDto.meetingId,
                    EventTime = dateTime
                };

                List<ConfUser> users = await _DbContext.ConfUsers.Where(c => 
                c.ProsodyId.Equals(prosodyEventPostDto.from)).OrderByDescending(item => item.ConfTime).ToListAsync();

                string userId = prosodyEventPostDto.from;

                var email = prosodyEventPostDto.message.Split(">");

                var user = await _DbContext.ConfUsers.Where(u => u.Email.Equals(email[email.Length - 1])).FirstOrDefaultAsync();

                if (user != null)
                {
                    res.UserId = user.Id.ToString();
                }

                res.EventInfo = prosodyEventPostDto.message;

                return res;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        private async Task<ConfEvent?> processOccupantLeavingLobbyEvent(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {

            try
            {
                ConfEvent res = new()
                {
                    EventType = Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE_LOBBY,
                    MeetingId = prosodyEventPostDto.meetingId,
                    EventTime = dateTime
                };

                List<ConfUser> users = await _DbContext.ConfUsers.Where(c =>
                c.ProsodyId.Equals(prosodyEventPostDto.from)).OrderByDescending(item => item.ConfTime).ToListAsync();

                string userId = prosodyEventPostDto.from;

                var email = prosodyEventPostDto.message.Split(">");

                var user = await _DbContext.ConfUsers.Where(u => u.Email.Equals(email[email.Length - 1])).FirstOrDefaultAsync();

                if (user != null)
                {
                    res.UserId = user.Id.ToString();
                }

                res.EventInfo = prosodyEventPostDto.message;

                return res;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }


        private async Task<ConfEvent?> ProcessOccupantJoinedEvent(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {

            try
            {
                ConfEvent res = new()
                {
                    EventType = Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_JOIN,
                    MeetingId = prosodyEventPostDto.meetingId,

                    EventTime = dateTime
                };

                XmlDocument xDoc = new();

                xDoc.LoadXml(prosodyEventPostDto.message);


                XmlNodeList? nodeList = xDoc.SelectNodes("./presence");
                if (nodeList == null)
                    return null;


                ConfUser confUser = new()
                {
                    ProsodyId = prosodyEventPostDto.from,
                    ConfTime = DateTime.Now
                };

                var eventClaims = nodeList.Item(0);
                if (eventClaims == null)
                    return null;

                foreach (XmlNode node in eventClaims!)
                {
                    if (node.Name.Equals("stats-id"))
                    {
                        confUser.ConfId = node.InnerText;
                    }

                    if (node.Name.Equals("nick"))
                    {
                        confUser.Name = node.InnerText;
                    }

                    if (node.Name.Equals("email"))
                    {
                        confUser.Email = node.InnerText;
                    }

                    if (node.Name.Equals("avatar-url"))
                    {
                        confUser.Avatar = node.InnerText;
                    }

                }


                if (confUser.ProsodyId != null && confUser.ConfId != null)
                {

                    var user = await _DbContext.ConfUsers.Where(u => u.Email.Equals(confUser.Email)).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        res.UserId = user.Id.ToString();
                    }

                    else
                    {
                        await _DbContext.AddAsync(confUser);
                        await _DbContext.SaveChangesAsync();
                        res.UserId = confUser.Id.ToString();
                    }
                }

                else
                {
                    return null;
                }

                res.EventInfo = prosodyEventPostDto.message;
                return res;
            } 
            
            catch (Exception)
            {

            }

            return null;
        }


        private ConfEvent? ProcessRoomCreatedEvent(ProsodyEventPostDto prosodyEventPostDto, DateTime dateTime)
        {
            try
            {
                ConfEvent res = new()
                {
                    EventType = Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED,
                    EventInfo = prosodyEventPostDto.message,
                    MeetingId = prosodyEventPostDto.meetingId,
                    EventTime = dateTime
                };

                return res;
            }

            catch
            {

            }

            return null;

        }


        /// <summary>
        /// 
        /// Input: primary key, meetingId, lang
        /// 
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="meetingID"></param>
        /// <param name="lang"></param>
        /// 
        /// <returns>Active users depending on the id and meeting id of the room</returns>
        public async Task<APIResult> HandleGetRoom(DateTimeRange range, string pId, string meetingID)
        {
            APIResult res = new();

            List<ConfEvent> confEvents = await _DbContext.ConfEvents.Where(e => (e.EventTime > range.StartDateTime.Date && e.EventTime < range.EndDateTime.AddDays(1).Date)).ToListAsync();

            var lastState = confEvents.OrderByDescending(x=>x.Id).Where(x => x.MeetingId.ToString().Equals(pId)).FirstOrDefault();

            if(lastState == null)
            {
                return res.FailMe(-1, "The requested room is not existed");
            }

            if(lastState != null && lastState.EventType == EVENT_TYPE.EVENT_TYPE_CONF_FINISHED)
            {
                return res.FailMe(-1, "The requested room is finished");
            }


            

            var roomsJoinedUsers = confEvents.Where(c => c.EventType == EVENT_TYPE.EVENT_TYPE_CONF_USER_JOIN

              && c.MeetingId.ToString().Equals(pId)

              && confEvents.Where(p => p.EventType == EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE && p.MeetingId == c.MeetingId && c.UserId.Equals(p.UserId) && c.EventTime < p.EventTime).FirstOrDefault() != null
            ).ToList();


            var confUsers = _DbContext.ConfUsers.ToList();

            try
            {
                List<User> activeEvents = new List<User>();

                foreach (ConfUser confUser in confUsers)
                {
                    if (roomsJoinedUsers.Count > 0 && roomsJoinedUsers.Select(x=>x.UserId).Contains(confUser.Id.ToString()))
                    {
                        User newUser = new User()
                        {
                            FullName = confUser.Name,
                            Email = confUser.Email

                        };

                        if(!activeEvents.Select(a=> a.Email).Contains(newUser.Email)){
                            activeEvents.Add(newUser);
                        }
                    }
                }

                return res.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, activeEvents);
            }

            catch
            {
                return res.FailMe(-1, "Failed to get active participants");
            }
        }



        /// <param name="lang"></param>
        /// <returns>Return list of active rooms</returns>
        public async Task<APIResult> HandleListRoom(string lang)
        {
            var events = await _DbContext.Events.ToListAsync();

            APIResult res = new();

            try
            {
                var activeRooms = await
                    _DbContext.ConfEvents.AsNoTracking()
                    .GroupBy(confEv => confEv.MeetingId)
                    .Select(g => new
                    {
                        MeetingId = g.Key,
                        g.OrderByDescending(e => e.EventTime).First().EventType
                    })
                    .Where(x => x.EventType != EVENT_TYPE.EVENT_TYPE_CONF_FINISHED)
                    .Select(y => y.MeetingId)
                    .ToListAsync();

                var activeEvents = await
                    _DbContext.Events.AsNoTracking()
                    .Where(ev => activeRooms.Contains(ev.MeetingId))
                    .Select(x => new ActiveRoom
                    {
                        MeetingId = x.MeetingId,
                        Topic = x.Topic
                    })
                    .ToListAsync();

                res.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, activeEvents);
            }
            catch (Exception e)
            {
                _ILogger.LogError("Quering active rooms failed due to db error: {}", e.Message);
                res.FailMe(-1, "Failed to get active rooms", false, APIResult.RESPONSE_CODE.ERROR);
            }
            return res;
        }

        public async Task<APIResult> GetUserEventsStatus(int userId, ConfEvent confEvent)
        {
            APIResult result = new APIResult();

            var host = _IConfiguation["CONFOMEET_BASE_URL"];

            var allRooms = await _DbContext.ConfEvents.ToListAsync();

            var allUsers = await _DbContext.ConfUsers.ToListAsync();

            var events = await _DbContext.Events.Where(x => x.CreatedBy == userId).Include(x => x.Participants)
            .Include(x => x.Meeting)
            .Select(e => new EventFullView
            {
                Id = e.Id,
                CreatedBy = e.CreatedBy,
                Topic = e.Topic,
                SubTopic = e.SubTopic,
                Organizer = e.Organizer,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                TimeZone = e.TimeZone,
                Password = e.Meeting != null ? (e.Meeting.Password ?? null) : null,
                RecordingReq = e.Meeting != null && (e.Meeting.RecordingReq ?? false),
                SingleAccess = e.Meeting != null && (e.Meeting.SingleAccess ?? false),
                AllDay = e.AllDay != null && (bool)e.AllDay,
                AutoLobby = e.Meeting != null && (e.Meeting.AutoLobby ?? false),
                MeetingId = e.MeetingId,
                Status = e.RecStatus,
                Type = e.Type,
                MeetingStatus = _IGeneralRepository.CheckStatus(e.StartDate, e.EndDate, e.Id, e.MeetingId, "en", allRooms),
                MeetingLink = e.MeetingId != null && e.CreatedBy == userId ?
                Url.Combine(host, "join", e.Participants.Where(p => p.UserId == e.CreatedBy).Select(p => Url.Combine(p.Id.ToString(), p.Guid.ToString())).FirstOrDefault()) + "?redirect=0" : null,
                StatusText = e.RecStatus == null ? string.Empty : EventStatusValue.ContainsKey((EVENT_STATUS)e.RecStatus) ? EventStatusValue[(EVENT_STATUS)e.RecStatus]["en"] : string.Empty,
                Participants = e.Participants.Select(p => new ParticipantView
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = string.Empty,
                    Email = !p.Email.ToLower().StartsWith(INVALID_EMAIL_PREFIX.ToLower()) ? p.Email : string.Empty,
                    Mobile = p.Mobile,
                    IsModerator = p.IsModerator,
                    Description = p.Description,
                    ParticipantStatus = _IGeneralRepository.CheckParticipantStatus(p.Email, e.Id, e.MeetingId, allRooms, allUsers),
                    Note = p.Note,
                    GroupIn = p.GroupIn,
                    MeetingLink = e.MeetingId != null && (p.UserId == userId) ? Url.Combine(host, "join", Url.Combine(p.Id.ToString(), p.Guid.ToString())) + "?redirect=0" : null,
                    PartyId = p.PartyId,
                }).ToList(),
 
            }).ToListAsync();

            try
            {

                var changedEvents = events.Where(x =>
                (confEvent.MeetingId == x.Id.ToString())
                &&

                x.Participants.Select(e => e.UserId).Contains(userId)


                ).ToList();


                //List<EventStatusDetails> eventStatuses = new List<EventStatusDetails>();

                ////var allRooms = await _DbContext.ConfEvents.ToListAsync();

                //if (changedEvents.Count() > 0)
                //{
                //    foreach (var singleEvent in changedEvents)
                //    {

                //        var eventStatus = _IGeneralRepository.CheckStatus(singleEvent.StartDate, singleEvent.EndDate, singleEvent.Id, singleEvent.MeetingId, "en", allRooms);
                //        EventStatusDetails eventStatusDetails = new EventStatusDetails
                //        {
                //            MeetingId = singleEvent.Id.ToString(),
                //            Status = eventStatus.Status,
                //            Text = eventStatus.Text
                //        };

                //        eventStatuses.Add(eventStatusDetails);

                //    }
                //}

                return result.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, changedEvents);
            }
            catch
            {

                return result.FailMe(-1, "Failed to load statuses");
            }
        }
    }
}
