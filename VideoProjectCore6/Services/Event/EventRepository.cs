using Flurl;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.MeetingDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.ParticipantDto;

using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IMeetingRepository;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IParticipantRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.Common;
using VideoProjectCore6.Utilities.Time;
using static VideoProjectCore6.Services.Constants;

namespace VideoProjectCore6.Services.Event;

public class EventRepository(IMeetingRepository iMeetingRepository
        , OraDbContext DBContext
        , IParticipantRepository iParticipantRepository
        , IUserRepository userRepository
        , IConfiguration iConfiguration
        , INotificationSettingRepository iNotificationSettingRepository
        , ISendNotificationRepository iSendNotificationRepository
        , IGeneralRepository iGeneralRepository
        , IGroupRepository iGroupRepository
) : IEventRepository
{
    private readonly OraDbContext _DbContext = DBContext;
    private readonly IMeetingRepository _IMeetingRepository = iMeetingRepository;
    private readonly INotificationSettingRepository _INotificationSettingRepository = iNotificationSettingRepository;
    private readonly ISendNotificationRepository _ISendNotificationRepository = iSendNotificationRepository;
    private readonly IParticipantRepository _IParticipantRepository = iParticipantRepository;
    private readonly IUserRepository _IUserRepository = userRepository;
    private readonly IConfiguration _IConfiguration = iConfiguration;
    private readonly IGroupRepository _IGroupRepository = iGroupRepository;
    private readonly IGeneralRepository _IGeneralRepository = iGeneralRepository;

    public async Task<APIResult> AddEvent(EventPostDto dto, int addBy, string lang)
    {
        APIResult result = new();
        var validate = ValidateEvent(dto, lang);
        var user = await _DbContext.Users.Where(e => e.Id == addBy).FirstOrDefaultAsync();

        if (!validate.Result)
        {
            return validate;
        }
        if ((dto.Type==null || dto.Type==0) && (dto.TypeOrder!=null))
        {
            var et = await _DbContext.SysLookupTypes.Where(x => x.Value == "event_type").Select(x => x.Id).FirstOrDefaultAsync();
            dto.Type = (short?)await _DbContext.SysLookupValues.Where(x => x.LookupTypeId == et && x.Order == dto.TypeOrder).Select(x => x.Id).FirstOrDefaultAsync();
        }
        try
        {
            Models.Event e = new()
            {
                ParentEvent = dto.ParentEventId,
                CreatedBy = addBy,
                Description = dto.Description,
                Topic = dto.Topic,
                SubTopic = dto.SubTopic,
                Organizer = dto.Organizer,
                CreatedDate = DateTime.Now,
                EndDate = dto.EndDate,
                StartDate = dto.StartDate,
                MeetingId = dto.MeetingId,
                AllDay = dto.AllDay,
                TimeZone = dto.TimeZone,
                RecStatus = dto.Status == null ? (sbyte)EVENT_STATUS.ACTIVE : dto.Status,
                AppId = dto.AppId,
                Type = dto.Type
            };
            _DbContext.Events.Add(e);
            await _DbContext.SaveChangesAsync();
            result.SuccessMe(e.Id, Translation.getMessage(lang, "sucsessAdd"));
        }
        catch (Exception ex)
        {
            result.FailMe(-1, Translation.getMessage(lang, "UnknownError") + ex.Message + " " + ex.InnerException);
        }
        return result;
    }

    public async Task<APIResult> AddMeetingEvent(EventWParticipant dto, int addBy, bool sendNotification, string lang)
    {
        APIResult eventResult = new();
        if (dto.ParentEventId != null)
        {
            var parentEvt = await _DbContext.Events.Where(x => x.Id == dto.ParentEventId).FirstOrDefaultAsync();
            if (!await isOkSubEventDate(dto.StartDate, dto.EndDate, (int)dto.ParentEventId))
            {
                return eventResult.FailMe(-1, "تاريخ الحدث الفرعي يجب أن يكون متوافق مع تاريخ الحدث الأب");
            }

            dto.MeetingId = parentEvt?.MeetingId;
            dto.AllDay = parentEvt?.AllDay;
        }

        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
        if (dto.ParentEventId == null && dto.MeetingRequired)
        {
            var meetingResult = await _IMeetingRepository.AddMeeting(new MeetingPostDto
            {
                Password = dto.Password,
                PasswordReq = dto.PasswordReq,//!string.IsNullOrWhiteSpace(dto.Password)
                RecordingReq = dto.RecordingReq,
                SingleAccess = dto.SingleAccess,
                AllDay = dto.AllDay,
                Status = (sbyte)MEETING_STATUS.PENDING,
                AutoLobby = dto.AutoLobby,
                Topic = dto.Topic,
                TimeZone = dto.TimeZone,
            }, addBy, lang);

            if (meetingResult.Id < 0)
            {
                return meetingResult;
            }

            dto.MeetingId = meetingResult.Result;
        }
        eventResult = await AddEvent(dto, addBy, lang);
        if (eventResult.Id < 0)
        {
            return eventResult;
        }
        if (!dto.Participants.Any(x => x.LocalUserId == addBy)) //Add Creator as participant
        {
            dto.Participants.Add(new ParicipantDto
            {
                LocalUserId = addBy,
                IsModerator = true,
                Email = await _DbContext.Users.Where(x => x.Id == addBy).Select(x => x.Email).FirstOrDefaultAsync()
            });
        }

        if(dto.GroupIds != null && dto.GroupIds.Count > 0)
        {
            foreach(var groupId in dto.GroupIds)
            {
                var users = await _IGroupRepository.GetUsersByGroupId(groupId, null, null);
                foreach(var user in users.Items)
                {
                    dto.Participants.Add(new ParicipantDto { LocalUserId = user.Id, Email = user.Email});
                }
            }
        }
        dto.Participants = dto.Participants.DistinctBy(p => p.Email).ToList();

        if (dto.Participants != null && dto.Participants.Count > 0)
        {
            var ParentEventAddParticipant = await AddParticipantsToEvents(dto.Participants, eventResult.Id, addBy, lang, sendNotification, false);
            if (ParentEventAddParticipant.Id < 0)
            {
                return ParentEventAddParticipant;
            }
        }
        scope.Complete();
        return eventResult;
    }
    public async Task<APIResult> AddParticipantsToEvents(List<ParicipantDto> dtos, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll)
    {
        var result = new APIResult();
        List<Receiver> receivers = [];

        var e = await _DbContext.Events.Where(x => x.Id == eventId).AsNoTracking().FirstOrDefaultAsync();
        if (e == null)
        {
            return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
        }
        foreach (var dto in dtos)
        {
            if (dto.Id == null) // New participant
            {
                dto.EventId = eventId;
                var uName = await _IUserRepository.CheckEMailAddress(dto.Email, lang);
                if (uName.Id < 0)
                {
                    if (string.IsNullOrWhiteSpace(dto.Mobile) || dto.Mobile.Length < 9)
                    {
                        uName.Message.Add("يرجى إدخال معلومات صحيحة للمشترك : الايميل أو رقم الموبايل");
                        return uName;
                    }
                    if (string.IsNullOrWhiteSpace(dto.Email))
                    {
                        dto.Email = INVALID_EMAIL_PREFIX + _IGeneralRepository.GetNewValueBySec() + INVALID_EMAIL_SUFFIX;
                    }

                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dto.FullName) && uName.Id > 0)
                    {
                        dto.FullName = uName.Result;
                    }
                }
            }
            else
            {
                var updateResult = await _IParticipantRepository.Update((int)dto.Id, dto, addBy, lang);
                if (updateResult.Id < 0)
                {
                    return updateResult;
                }
            }
        }
        var distinctEmailCount = dtos.DistinctBy(e => e.Email).Count();
        if (distinctEmailCount != dtos.Count)
        {
            return result.FailMe(-1, Translation.getMessage(lang, "SameEmail"));
        }
        var pu = await _IParticipantRepository.ParticipantsAsUser(dtos.Where(d => d.Id == null && d.Id != addBy).ToList(), addBy, lang);
        if (pu.Id < 0)
        {
            return pu;
        }
        result = await _IParticipantRepository.AddParticipants(dtos.Where(d => d.Id == null && d.Id != addBy).ToList(), addBy, lang);
        if (result.Id < 0)
        {
            return result;
        }
        //ADD LOGS
        var logs = new List<Models.EventLog>();
        int actionId = await _DbContext.Actions.Where(x => x.Shortcut == SEND_INVITATION_ACTION).Select(x => x.Id).FirstOrDefaultAsync();
        foreach (var email in result.Result)
        {
            logs.Add(new Models.EventLog
            {
                EventId = eventId,
                Note = email.Email,
                ActionId = (short)actionId,
                CreatedBy = addBy,
                ObjectType = OPJECT_TYPE_PARTICIPANT,
                RelatedId = email.ParticipantId,
                CreatedDate = DateTime.Now,
            });
        }
        // await _IEventLogRepository.AddEventLogs(logs, lang);

        var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(actionId, eventId);
        if (sendNotification)
        {
            string[] MeetingIdSplit = e.MeetingId.ToUpper().Split('C', 2);
            var clearMeetingID = MeetingIdSplit.Last();
            var parameters = new Dictionary<string, string>

            //var parameters = new Dictionary<string, string>
                 {
                   { FROM_DATE, e.StartDate.ToString("dd-MM-yyyy")},
                   { TO_DATE, e.EndDate.ToString("dd-MM-yyyy")},
                   { FROM_TIME, e.StartDate.ToString("hh:mm tt")},
                   { TO_TIME, e.EndDate.ToString("hh:mm tt")},
                   { MEETING_ID, clearMeetingID},
                   { TOPIC, e.Topic},
                   { TIMEZONE, e.TimeZone},
                 };


            if (sendToAll)
            {

                //var EventParticipants = await _DbContext.Participants.Where(p => p.EventId == eventId).ToListAsync();

                //foreach (var p in EventParticipants)
                //{
                //    receivers.Add(new Receiver
                //    {
                //        Email = p.Email,
                //        Id = p.UserId,
                //        Mobile = p.Mobile,
                //        Name = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.FullName).FirstOrDefault(),
                //        ParticipantId = p.Id,
                //        Tokens = null
                //    });
                //}

                //var a = await _IParticipantRepository.NotifyParticipants(receivers, e.MeetingId, toSendNoti, parameters, INVITATION_TEMPLATE, sendNotification, false);
                //if (a == null)
                //{
                //    return result.FailMe(-1, "Error sending notification", true);
                //}
            }

            else
            {
                var a = await _IParticipantRepository.NotifyParticipants(result.Result, e.MeetingId, toSendNoti, parameters,
                                                                         INVITATION_TEMPLATE, sendNotification, false);
                

                if (a == null)
                {
                    return result.FailMe(a, "Error sending notification", true);
                }

            }
        }

        
        return result.SuccessMe(eventId, Translation.getMessage(lang, "ParticipantAdded"), true, APIResult.RESPONSE_CODE.OK, result.Result);
    }

    public async Task<APIResult> UpdateEvent(int id, int updatedBy, MeetingEventDto dto, UpdateOption? opt, string lang)
    {
        APIResult result = new APIResult();
        result = ValidateEvent(dto, lang);
        if (result.Id < 0)
        {
            return result;
        }

        string message = string.Empty;
        bool dateChanged = false, minuteChanged = false;
        double daysStart = 0, daysEnd = 0, minutsStart = 0, minutsEnd = 0;
        string fixSubEventMsg = string.Empty;

        //bool notifySubParticipant = false;
        List<Models.Event> subEvents = new();
        Models.Event? evt = await _DbContext.Events.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (evt == null)
        {
            return result.FailMe(-1, "الحدث غير موجود");
        }
        var originStartDate = evt.StartDate;
        string? newTimeZone = dto.TimeZone != evt.TimeZone ? dto.TimeZone : null;

        dateChanged = dto.StartDate.Date != evt.StartDate.Date || dto.EndDate.Date != evt.EndDate.Date;
        minuteChanged = dto.StartDate.TimeOfDay != evt.StartDate.TimeOfDay || dto.EndDate.TimeOfDay != evt.EndDate.TimeOfDay;
        bool timeChanged = dateChanged || minuteChanged || dto.TimeZone != evt.TimeZone;
        if (timeChanged)
        {
            if (dateChanged)
            {
                daysStart = (dto.StartDate.Date - evt.StartDate.Date).TotalDays;
                daysEnd = (dto.EndDate.Date - evt.EndDate.Date).TotalDays;
            }
            if (minuteChanged)
            {
                minutsStart = (dto.StartDate.TimeOfDay - evt.StartDate.TimeOfDay).TotalMinutes;
                minutsEnd = (dto.EndDate.TimeOfDay - evt.EndDate.TimeOfDay).TotalMinutes;
            }
            if (evt.ParentEvent == null)
            {
                subEvents = await _DbContext.Events.Where(a => a.ParentEvent == id).
                     Include(x => x.Participants)
                    .ToListAsync();
            }
            else
            {
                if (!await isOkSubEventDate(dto.StartDate, dto.EndDate, (int)evt.ParentEvent))
                {
                    return result.FailMe(-1, "تاريخ الحدث الفرعي يجب أن يكون متوافق مع تاريخ الحدث الأب");
                }
            }
           // return await Reschedule(id, dto, updatedBy, lang);
        }
        int actionId = await _DbContext.Actions.Where(x => x.Shortcut == UPDATE_EVENT_ACTION).Select(x => x.Id).FirstOrDefaultAsync();

        // using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            if (subEvents.Any() && timeChanged)
            {
                dateChanged = dto.StartDate.Date != evt.StartDate.Date || dto.EndDate.Date != evt.EndDate.Date;
                minuteChanged = dto.StartDate.TimeOfDay != evt.StartDate.TimeOfDay || dto.EndDate.TimeOfDay != evt.EndDate.TimeOfDay;
                var conflictedSEvent = subEvents.Where(x => x.EndDate > dto.EndDate || x.StartDate < dto.StartDate).Select(x => x.Id).ToList();
                if (conflictedSEvent.Any())
                {
                    message = Translation.getMessage(lang, "SubEventDateConflict");
                    //return result.FailMe(-1, Translation.getMessage(lang, "SubEventDateConflict"), true, APIResult.RESPONSE_CODE.BadRequest, conflictedSEvent);
                }

                //foreach (var sub in subEvents)
                //{
                //    if (dateChanged)
                //    {
                //        sub.StartDate = sub.StartDate.AddDays((dto.StartDate.Date - event_.StartDate.Date).TotalDays);
                //        sub.EndDate = sub.EndDate.AddDays((dto.EndDate.Date - event_.EndDate.Date).TotalDays);
                //        notifySubParticipant = true;
                //    }
                //    if (minuteChanged)
                //    {
                //        var fromS = (dto.StartDate.TimeOfDay - event_.StartDate.TimeOfDay).TotalMinutes;
                //        var fromE = (dto.EndDate.TimeOfDay - event_.EndDate.TimeOfDay).TotalMinutes;
                //        if (fromS == fromE)
                //        {
                //            sub.StartDate = sub.StartDate.AddMinutes(fromS);
                //            sub.EndDate = sub.EndDate.AddMinutes(fromE);
                //            notifySubParticipant = true;
                //        }
                //        else
                //        {
                //            sub.StartDate = dto.StartDate;
                //            sub.EndDate = dto.EndDate;
                //            fixSubEventMsg = Translation.getMessage(lang, "FixSubEventDate");
                //            notifySubParticipant = false;
                //        }
                //    }
                //    sub.LastUpdatedDate = DateTime.Now;
                //    sub.LastUpdatedBy = updatedBy;
                //}
                //_DbContext.UpdateRange(subEvents);
            }
            if (timeChanged || evt.Topic != dto.Topic || evt.SubTopic != dto.SubTopic 
                || evt.Organizer != dto.Organizer
                || evt.Description != dto.Description || evt.AllDay != dto.AllDay /*|| evt.RecStatus != dto.Status*/)
            {
                evt.Topic = dto.Topic;
                evt.SubTopic = dto.SubTopic;
                evt.Description = dto.Description;
                evt.Organizer = dto.Organizer;
                evt.StartDate = dto.StartDate;
                evt.EndDate = dto.EndDate;
                evt.TimeZone = dto.TimeZone;
                evt.ParentEvent = dto.ParentEventId;
                evt.AllDay = dto.AllDay;
                evt.RecStatus = dto.Status != null ? dto.Status : evt.RecStatus;
                evt.LastUpdatedDate = DateTime.Now;
                evt.LastUpdatedBy = updatedBy;
                _DbContext.Events.Update(evt);

                //---------------------------LOG-----------------------------------
                //var addLog = await _IEventLogRepository.AddEventLog(new Models.EventLog
                //{
                //    ActionId = (short)actionId,
                //    CreatedBy = updatedBy,
                //    CreatedDate = DateTime.Now,
                //    EventId = id,
                //    ObjectType = OPJECT_TYPE_EVENT,
                //    RelatedId = id
                //}, lang);
                //if (addLog.Id < 0)
                //{
                //    return addLog;
                //}
            }
            if (evt.MeetingId != null)
            {

                var meeting = await _DbContext.Meetings.Where(m => m.MeetingId == evt.MeetingId).FirstOrDefaultAsync();
                bool needUpdate = false;
                if (meeting != null)
                {
                    if (timeChanged && evt.ParentEvent == null)
                    {
                        meeting.StartDate = dto.StartDate;
                        meeting.EndDate = dto.EndDate;
                        meeting.TimeZone = dto.TimeZone;
                        needUpdate = true;
                    }
                    if (meeting.Password != dto.Password 
                        || meeting.PasswordReq != dto.PasswordReq
                        || meeting.RecordingReq != dto.RecordingReq
                        || meeting.SingleAccess != dto.SingleAccess 
                        || meeting.AutoLobby != dto.AutoLobby)
                    {
                        meeting.Password = dto.Password;
                        meeting.PasswordReq = dto.PasswordReq;
                        meeting.RecordingReq = dto.RecordingReq;
                        meeting.SingleAccess = dto.SingleAccess;
                        meeting.AutoLobby = dto.AutoLobby;

                        needUpdate = true;
                    }
                    if (needUpdate)
                    {
                        meeting.LastUpdatedDate = DateTime.Now;
                        meeting.LastUpdatedBy = updatedBy;
                        _DbContext.Meetings.Update(meeting);
                    }
                }
            }

            List<int?> newParticipant = [];
            using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
            {
                await _DbContext.SaveChangesAsync();

                if (opt != null && timeChanged && !opt.JustCurrent && opt.HasOneOption())
                {
                    result = await ShiftRecurrenceEvents(id, updatedBy, originStartDate, daysStart, daysEnd, minutsStart, minutsEnd, newTimeZone, opt, false, lang);
                    if (result.Id < 0)
                    {
                        return result;
                    }
                }
                if (dto.Participants != null && dto.Participants.Count > 0)
                {
                    var ParentEventAddParticipant = await AddParticipantsToEvents(dto.Participants, id, updatedBy, lang, true, false);
                    if (ParentEventAddParticipant.Id < 0)
                    {
                        return ParentEventAddParticipant;
                    }
                    List<Receiver> r = ParentEventAddParticipant.Result;
                    newParticipant = r.Select(x => x.ParticipantId).ToList();
                    //int envitationActionId = await _DbContext.Actions.Where(x => x.Shortcut == "SEND_INVITATION").Select(x => x.Id).FirstOrDefaultAsync();
                    //var notification = await _INotificationSettingRepository.GetNotificationsForAction(envitationActionId, id);
                    //Receiver newReceiver = ParentEventAddParticipant.Result;


                    //   scope.Dispose();
                }

                scope.Complete();
                scope.Dispose();
            }
            if (timeChanged)
            {

                int updateEventActionId = await _DbContext.Actions.Where(x => x.Shortcut == "UPDATE_EVENT").Select(x => x.Id).FirstOrDefaultAsync();

                var receivers = await _DbContext.Participants.Where(p => p.EventId == id & !newParticipant.Contains(p.Id)).AsNoTracking().Select(x => new Receiver
                {
                    Email = x.Email,
                    Id = x.UserId,
                    Mobile = x.Mobile,
                    ParticipantId = x.Id
                }).ToListAsync();
                var parameters = new Dictionary<string, string>
                {
                    [FROM_DATE] = dto.StartDate.ToString("dd-MM-yyyy"),
                    [TO_DATE] = dto.EndDate.ToString("dd-MM-yyyy"),
                    [FROM_TIME] = dto.StartDate.ToString("hh:mm tt"),
                    [TO_TIME] = dto.EndDate.ToString("hh:mm tt"),
                    [TOPIC] = dto.Topic,
                    [DESCRIPTION] = dto.Description,
                    [TIMEZONE] = dto.TimeZone,
                    [MEETING_ID] = dto.MeetingId
                };

                var notification = await _INotificationSettingRepository.GetNotificationsForAction(actionId, id);
                var n = await _IParticipantRepository.NotifyParticipants(receivers, evt.MeetingId, notification, parameters, INVITATION_TEMPLATE, true, false);

                //--------Notify sub event participants-------------
                //if (notifySubParticipant)
                //{
                //    foreach (var subE in subEvents)
                //    {

                //        var parametersSub = new Dictionary<string, string>();
                //        var subReceivers = subE.Participants.Select(x => new Receiver
                //        {
                //            Email = x.Email,
                //            Id = x.UserId,
                //            Mobile = x.Mobile,
                //            ParticipantId = x.Id
                //        }).ToList();
                //        parametersSub[FROM_DATE] = subE.StartDate.ToString("dd-MM-yyyy");
                //        parametersSub[TO_DATE] = subE.EndDate.ToString("dd-MM-yyyy");
                //        parametersSub[FROM_TIME] = subE.StartDate.ToString("hh:mm tt");
                //        parametersSub[TO_TIME] = subE.EndDate.ToString("hh:mm tt");
                //        parametersSub[TOPIC] = subE.Topic;
                //        parametersSub[DESCRIPTION] = subE.Description;
                //        parametersSub[TIMEZONE] = subE.TimeZone;
                //        var notificationSub = await _INotificationSettingRepository.GetNotificationsForAction(updateEventActionId, subE.Id);
                //        var b = await _IParticipantRepository.NotifyParticipants(subReceivers, subE.MeetingId, notificationSub, parametersSub, INVITATION_TEMPLATE, true, false);
                //    }
                //}
            }
            result.SuccessMe(id, /*fixSubEventMsg != string.Empty ? fixSubEventMsg :*/ Translation.getMessage(lang, "sucsessUpdate"));
        }
        catch (Exception)
        {
            result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
        }
        return result;
    }
    private APIResult ValidateEvent(EventPostDto dto, string lang)
    {
        APIResult result = new();
        if (dto.EndDate == DateTime.MinValue || dto.StartDate == DateTime.MinValue || string.IsNullOrWhiteSpace(dto.TimeZone))
        {
            return result.FailMe(-1, Translation.getMessage(lang, "MissingEventDate"));
        }
        if (dto.EndDate < dto.StartDate)
        {
            return result.FailMe(-1, Translation.getMessage(lang, "ErrorEventDate"));
        }

        if (dto.MeetingId != null)
        {
            var meet = _DbContext.Meetings.Where(x => x.MeetingId == dto.MeetingId).SingleOrDefault();
            if (meet == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "MettingNotFound"));
            }
        }
        return result.SuccessMe(1);
    }

    private APIResult ValidateEvent(EventDto dto, string lang)
    {
        var postDto = new EventPostDto
        {
            Description = dto.Description,
            EndDate = dto.EndDate,
            StartDate = dto.StartDate,
            MeetingId = dto.MeetingId,
            TimeZone = dto.TimeZone,
            Topic = dto.Topic,
        };
        return ValidateEvent(postDto, lang);
    }

    public async Task<List<EventFullView>> GetAllOfUser(int userId, EventSearchObject? obj = null, string lang = "ar")
    {
        bool applyRelatedEvent = false;
        bool serverSearch = obj != null && obj.HasDateSearch();
        bool localSearch = obj != null && obj.HasStringSearch();
        var relatedUsers = new List<int>();

        var host = _IConfiguration["CONFOMEET_BASE_URL"];

        var allRooms = await _DbContext.ConfEvents.ToListAsync();

        var allUsers = await _DbContext.ConfUsers.ToListAsync();

        List<EventFullView> events = await _DbContext.Events.AsNoTracking()
            .Include(x => x.Participants)
            .Include(x => x.Meeting)
            .Include(x => x.InverseParentEventNavigation)
            .Where(x =>
             //(!serverSearch || (obj.StartDate == null || x.StartDate >= obj.StartDate.Value.Date) && (obj.StartDate == null || x.StartDate < obj.EndDate.Value.AddDays(1).Date)) &&
             (!serverSearch
                || (x.StartDate >= obj!.StartDate!.Value.Date && x.StartDate < obj!.EndDate!.Value.AddDays(1).Date)
                || (x.EndDate > obj.StartDate.Value.Date && x.EndDate <= obj!.EndDate!.Value.AddDays(1).Date)
                || (obj.StartDate.Value.Date >= x.StartDate && obj!.EndDate!.Value.Date <= x.EndDate)
             
             //|| (obj.Organizer != null && x.Organizer.Contains(obj.Organizer))
             //|| (obj.Participant != null && x.Participants.Select(e => e.User.FullName).Contains(obj.Participant))
             //|| (obj.Email != null && x.Participants.Select(e => e.Email).Contains(obj.Email))
             //|| (obj.PhoneNumber != null && x.Participants.Select(e => e.Mobile).Contains(obj.PhoneNumber))
             //|| (obj.EventType != null && x.Type == obj.EventType)


             ) &&
             ((!applyRelatedEvent && (x.CreatedBy == userId || x.Participants.Any(p => p.UserId == userId)))
             || relatedUsers.Contains(x.CreatedBy) || x.Participants.Select(p => p.UserId).Distinct().Any(n => relatedUsers.Any(y => y == n))/*|| relatedUsers.Intersect(x.Participants.Select(p => p.Id).ToList()).Any()*/
             ))
            .AsNoTracking()
            .OrderByDescending(e => e.StartDate)
            .Select(e => new EventFullView
            {
                Id = e.Id,
                ByMe = e.CreatedBy == userId,
                CreatedBy = e.CreatedBy,
                Topic = e.Topic,
                SubTopic = e.SubTopic,
                Organizer = e.Organizer,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                TimeZone = e.TimeZone,
                Password = e.Meeting != null ? (e.Meeting.Password ?? null) : null,
                PasswordReq = e.Meeting != null && e.Meeting.PasswordReq,
                RecordingReq = e.Meeting != null && (e.Meeting.RecordingReq ?? false),
                SingleAccess = e.Meeting != null && (e.Meeting.SingleAccess ?? false),
                AllDay = e.AllDay != null && (bool)e.AllDay,
                AutoLobby = e.Meeting != null && (e.Meeting.AutoLobby ?? false),
                MeetingId = e.MeetingId,
                Status = e.RecStatus,
                Type = e.Type,
                MeetingStatus = _IGeneralRepository.CheckStatus(e.StartDate, e.EndDate, e.Id, e.MeetingId, lang, allRooms),
                MeetingLink = e.MeetingId != null && e.CreatedBy == userId && e.ParentEvent == null ?
                (e.Meeting != null ? e.Meeting.MeetingLog : null) ?? (Url.Combine(host, "join", e.Participants.Where(p => p.UserId == e.CreatedBy).Select(p => Url.Combine(p.Id.ToString(), p.Guid.ToString())).FirstOrDefault()) + "?redirect=0") : null,

                ParentEventId = e.ParentEvent,
                StatusText = e.RecStatus == null ? string.Empty : EventStatusValue.ContainsKey((EVENT_STATUS)e.RecStatus) ? EventStatusValue[(EVENT_STATUS)e.RecStatus][lang] : string.Empty,
                Participants = e.Participants.Select(p => new ParticipantView
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = p.User.FullName,
                    Email = !p.Email.StartsWith(INVALID_EMAIL_PREFIX, StringComparison.OrdinalIgnoreCase) ? p.Email : string.Empty,
                    Mobile = p.Mobile,
                    IsModerator = p.IsModerator,
                    Description = p.Description,
                    ParticipantStatus = _IGeneralRepository.CheckParticipantStatus(p.Email, e.Id, e.MeetingId, allRooms, allUsers),
                    Note = p.Note,
                    GroupIn = p.GroupIn,
                    //MeetingLink = e.MeetingId != null && (p.UserId == userId || relatedUsers.Contains(p.UserId)) ? Url.Combine(host, "join", Url.Combine(p.Id.ToString(), p.Guid.ToString())) + "?redirect=0" : null,
                    MeetingLink = (e.Meeting != null ? e.Meeting.MeetingLog : null) ?? (e.MeetingId != null && (p.UserId == userId || relatedUsers.Contains(p.UserId)) ? Url.Combine(host, "join", Url.Combine(p.Id.ToString(), p.Guid.ToString())) + "?redirect=0" : null),
                    PartyId = p.PartyId,
                }).ToList(),
                SubEventCount = e.InverseParentEventNavigation != null ? e.InverseParentEventNavigation.Count() : 0,
                //SubEvents = e.InverseParentEventNavigation != null ? e.InverseParentEventNavigation.Select(sub => new EventFullView
                //{
                //    Id = sub.Id,
                //    Description = sub.Description,
                //    Topic = sub.Topic,
                //    SubTopic = sub.SubTopic,
                //    Organizer = sub.Organizer,
                //    StartDate = sub.StartDate,
                //    EndDate = sub.EndDate,
                //    MeetingId = sub.MeetingId,
                //    MeetingLink = null,
                //    ParentEventId = sub.ParentEvent,
                //    StatusText = sub.RecStatus == null ? string.Empty : EventStatusValue.ContainsKey((EVENT_STATUS)sub.RecStatus) ? EventStatusValue[(EVENT_STATUS)sub.RecStatus][lang] : string.Empty,
                //    Participants = sub.Participants.Select(p => new ParticipantView
                //    {
                //        Id = p.Id,
                //        UserId = p.UserId,
                //        FullName = "",
                //        Email = p.Email,
                //        Mobile = p.Mobile,
                //        IsModerator = p.IsModerator,
                //        Description = p.Description,
                //        Note = p.Note
                //    }).ToList(),
                //}).ToList() : null,
            }).ToListAsync();
        List<int> parentIds = events.Where(e => e.ParentEventId != null).Select(e => e.ParentEventId ?? -1).Distinct().ToList();
        var exitingParentIds = events.Where(e => e.SubEventCount > 0).Select(e => e.Id).Distinct().ToList();

        foreach (var e in events)
        {
            if (e.ParentEventId != null && exitingParentIds.Contains((int)e.ParentEventId))
            {
                e.ToHide = true;
            }
            e.VideoLogs = await FetchVideoLogs(e.MeetingId, _DbContext, null);
        }
        if (events != null && events.Count > 0)
        {
            if (localSearch)
            {
                events = events.Where(e =>
                (string.IsNullOrWhiteSpace(obj?.Topic) || e.Topic.Contains(obj.Topic, StringComparison.CurrentCultureIgnoreCase))
                &&
                (string.IsNullOrWhiteSpace(obj?.SubTopic) || (e.SubTopic != null && e.SubTopic.Contains(obj.SubTopic, StringComparison.CurrentCultureIgnoreCase)))


             && (string.IsNullOrWhiteSpace(obj?.MeetingId) || e.MeetingId.Equals(obj.MeetingId))
             && (string.IsNullOrWhiteSpace(obj?.Organizer) || (e.Organizer != null && e.Organizer.Contains(obj.Organizer, StringComparison.CurrentCultureIgnoreCase)))
             && (string.IsNullOrWhiteSpace(obj?.Participant) || e.Participants.Select(e => e.FullName.ToLower()).Contains(obj.Participant.ToLower()))
             && (string.IsNullOrWhiteSpace(obj?.Email) || e.Participants.Select(e => e.Email).Contains(obj.Email))
             && (string.IsNullOrWhiteSpace(obj?.Entity) || (e.Organizer != null && e.CreatedByName.Contains(obj.Entity)))
             && (string.IsNullOrWhiteSpace(obj?.PhoneNumber) || e.Participants.Select(e => e.Mobile).Any(p => p.Equals(obj.PhoneNumber)))

             && (obj?.EventType == null || e.Type == obj.EventType)
                ).ToList();
            }

            var usersId1 = events.SelectMany(x => x.Participants.Select(p => p.UserId)).Distinct().ToList();
            //var usersId2 = events.SelectMany(x => x.SubEvents.SelectMany(p => p.Participants.Select(g => g.UserId).ToList())).Distinct().ToList();
            //usersId1.AddRange(usersId2);
            usersId1.AddRange(events.Select(x => x.CreatedBy).ToList().Distinct());
            var userName = _DbContext.Users.Where(u => usersId1.Contains(u.Id)).Select(x => new { x.Id, x.FullName }).ToDictionary(x => x.Id, x => x.FullName);
            foreach (var e in events)
            {
                e.CreatedByName = userName.TryGetValue(e.CreatedBy, out string? v) ? v : string.Empty;

                foreach (var par in e.Participants)
                {
                    par.FullName = userName.TryGetValue(par.UserId, out v) ? v : string.Empty;
                }
                //foreach (var sub in e.SubEvents)
                //{
                //    foreach (var par in sub.Participants)
                //    {
                //        b = userName.TryGetValue(par.UserId, out v);
                //        par.FullName = b ? v : string.Empty;
                //    }
                //}
            }

            //if (localSearch && obj.Participants != null && obj.Participants.Any())
            //{
            //    events = events.Where(e => obj.allParticipant ? !obj.Participants.Except(e.Participants.Select(p => p.FullName).ToList()).Any()
            //    : e.Participants.Select(p => p.FullName).ToList().Intersect(obj.Participants).Any()).ToList();
            //}
            //if (localSearch && !string.IsNullOrWhiteSpace(obj.Participant))/*obj.Participants != null && obj.Participants.Any()*/
            //{
            //    events = events.Where(e => e.Participants.Any(p => p!=null && p.FullName.Contains(obj.Participant) || p.Email.ToLower().Contains(obj.Participant.ToLower()))).ToList();
            //}
        }
        return [.. events!.OrderByDescending(x=>x.StartDate)];
    }

    public async Task<ListCount> GetAll(int CurrentUserId, EventSearchObject? obj = null, int pageIndex = 1, int pageSize = 25, string lang = "ar")
    {
        bool serverSearch = obj != null && obj.HasDateSearch();
        bool localSearch = obj != null && obj.HasStringSearch();
        var host = _IConfiguration["CONFOMEET_BASE_URL"];
        List<int> idsList = [];

        var allRooms = await _DbContext.ConfEvents.Where(cv => obj == null || obj.EndDate == null || obj.StartDate == null || (cv.EventTime >= obj.StartDate.Value.Date && cv.EventTime < obj.EndDate.Value.AddDays(1).Date)).ToListAsync();

        var allUsers = await _DbContext.ConfUsers.ToListAsync();

        var events = await _DbContext.Events
         .Include(x => x.Participants).ThenInclude(u => u.User)
         .Include(x => x.Meeting)
             .Include(x => x.InverseParentEventNavigation)
         .Where(x => (!serverSearch
                || (x.StartDate >= obj!.StartDate!.Value.Date && x.StartDate < obj!.EndDate!.Value.AddDays(1).Date)
                || (x.EndDate > obj.StartDate.Value.Date && x.EndDate <= obj!.EndDate!.Value.AddDays(1).Date)
                || (obj.StartDate.Value.Date >= x.StartDate && obj!.EndDate!.Value.Date <= x.EndDate))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.Topic) || x.Topic.Contains(obj!.Topic, StringComparison.CurrentCultureIgnoreCase))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.Entity) || (x.Organizer != null && x.User.FullName.Contains(obj!.Entity, StringComparison.CurrentCultureIgnoreCase)))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.SubTopic) || (x.SubTopic != null && x.SubTopic.Contains(obj!.SubTopic, StringComparison.CurrentCultureIgnoreCase)))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.MeetingId) || x.MeetingId.Equals(obj!.MeetingId))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.Organizer) || (x.Organizer != null && x.Organizer.Contains(obj!.Organizer, StringComparison.CurrentCultureIgnoreCase)))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.Email) || x.Participants.Select(e => e.Email).Contains(obj!.Email))
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.PhoneNumber) || x.Participants.Select(e => e.Mobile).Any(p => p.Equals(obj!.PhoneNumber)))
         && (obj == null || obj.EventType == null || x.Type == obj.EventType)
         && (string.IsNullOrWhiteSpace(obj == null ? null : obj.Participant) || x.Participants.Select(e => e.User.FullName.ToLower()).Contains(obj!.Participant.ToLower()))
         )
         .OrderByDescending(e => e.StartDate)
        .Select(e => new EventFullView
       {
           Id = e.Id,
           ByMe = e.CreatedBy == CurrentUserId,
           CreatedBy = e.CreatedBy,
           CreatedByName = e.User.FullName,
           Topic = e.Topic,
           SubTopic = e.SubTopic,
           Organizer = e.Organizer,
           Description = e.Description,
           StartDate = e.StartDate,
           EndDate = e.EndDate,
           TimeZone = e.TimeZone,
           Password = e.Meeting.Password,
           PasswordReq = e.Meeting != null && e.Meeting.PasswordReq,
           RecordingReq = e.Meeting != null && (e.Meeting.RecordingReq ?? false),
           SingleAccess = e.Meeting != null && (e.Meeting.SingleAccess ?? false),
           MeetingId = e.MeetingId,
           Status = e.RecStatus,
           Type = e.Type,
           AllDay = e.AllDay,
           MeetingLink = e.MeetingId != null && e.CreatedBy == CurrentUserId && e.ParentEvent == null ? Url.Combine(host, "join", e.Participants.Where(p => p.UserId == e.CreatedBy).Select(p => Url.Combine(p.Id.ToString(), p.Guid.ToString())).FirstOrDefault()) + "?redirect=0" : null,
           ParentEventId = e.ParentEvent,
           MeetingStatus = _IGeneralRepository.CheckStatus(e.StartDate, e.EndDate, e.Id, e.MeetingId, lang, allRooms),
           StatusText = e.RecStatus == null ? string.Empty : EventStatusValue.ContainsKey((EVENT_STATUS)e.RecStatus) ? EventStatusValue[(EVENT_STATUS)e.RecStatus][lang] : string.Empty,
           Participants = e.Participants.Select(p => new ParticipantView
           {
               Id = p.Id,
               UserId = p.UserId,
               FullName = p.User.FullName,
               Email = p.Email,
               ParticipantStatus = _IGeneralRepository.CheckParticipantStatus(p.Email, e.Id, e.MeetingId, allRooms, allUsers),
               Mobile = p.Mobile,
               IsModerator = p.IsModerator,
               Description = p.Description,
               Note = p.Note
           }).ToList(),
           SubEventCount = e.InverseParentEventNavigation != null ? e.InverseParentEventNavigation.Count() : 0,
       }).ToListAsync();

        var total = events.Count;
        var items = (obj != null && obj.Pagination) ? PaginatedList<EventFullView>.Create(events.AsQueryable(), pageIndex > 0 ? pageIndex : 1, pageSize > 0 ? pageSize : 25, total) : events;

        var parentIds = items.Where(e => e.ParentEventId != null).Select(e => e.ParentEventId).ToHashSet();
        var exitingParentIds = items.Where(e => e.SubEventCount > 0).Select(e => e.Id).ToHashSet();

        foreach (var e in items)
        {
            if (e.ParentEventId != null && exitingParentIds.Contains((int)e.ParentEventId))
            {
                e.ToHide = true;
            }
        }
        var filteredResult = PaginatedList<EventFullView>.Create(events.AsQueryable(), pageIndex > 0 ? pageIndex : 1, pageSize > 0 ? pageSize : 25, total);

        //if (events != null && events.Any())
        //{
        //    var usersId1 = events.Select(x => x.CreatedBy).ToList().Distinct();
        //    var userName = _DbContext.Users.Where(u => usersId1.Contains(u.Id)).ToDictionary(x => x.Id, x => x.FullName);
        //    string v; 
        //    foreach (var e in events)
        //    {
        //        e.CreatedByName = userName.TryGetValue(e.CreatedBy, out v) ? v : string.Empty;
        //    }
        //}
        return new ListCount
        {
            Count = total,
            Items = items
        };
    }

    private async Task<bool> isOkSubEventDate(DateTime start, DateTime end, int parentEvtId)
    {
        var parentEvt = await _DbContext.Events.Where(x => x.Id == parentEvtId).FirstOrDefaultAsync();
        if (parentEvt == null)
        {
            return false;
        }
        if (start < parentEvt.StartDate || end > parentEvt.EndDate)
        {
            return false;
        }
        return true;
    }

    public async Task<EventFullView?> EventDetails(int id, int userId, string timeZoneId)
    {
        var allUsers = await _DbContext.ConfUsers.ToListAsync();

        var host = _IConfiguration["CONFOMEET_BASE_URL"];
        var e = await _DbContext.Events.AsNoTracking()
            .Include(x => x.Participants).ThenInclude(y => y.User)
            .Include(x => x.Meeting)
            .Include(x => x.User)
            .Where(e => e.Id == id)
            .Select(e => new EventFullView
            {
                Id = e.Id,
                CreatedBy = e.CreatedBy,
                CreatedByName = e.User.FullName,
                Topic = e.Topic,
                SubTopic = e.SubTopic,
                Organizer = e.Organizer,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                TimeZone = e.TimeZone,
                Password = e.Meeting.Password,
                PasswordReq = e.Meeting != null && e.Meeting.PasswordReq,
                RecordingReq = e.Meeting != null && (e.Meeting.RecordingReq ?? false),
                SingleAccess = e.Meeting != null && (e.Meeting.SingleAccess ?? false),
                MeetingId = e.MeetingId,
                Status = e.RecStatus,
                AllDay = e.AllDay,
                Type = e.Type,
                EventLogs = new List<ConfEventCompactGet>(),
                ParentEventId = e.ParentEvent,
                Participants = e.Participants.Select(p => new ParticipantView
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = p.User.FullName,
                    Email = !p.Email.StartsWith(INVALID_EMAIL_PREFIX, StringComparison.OrdinalIgnoreCase) ? p.Email : string.Empty,
                    Mobile = p.Mobile,
                    IsModerator = p.IsModerator,
                    Description = p.Description,
                    Note = p.Note,
                    GroupIn = p.GroupIn,
                    MeetingLink = e.MeetingId != null && p.UserId == userId ? Url.Combine(host, "join", Url.Combine(p.Id.ToString(), p.Guid.ToString())) + "?redirect=0" : null,
                    PartyId = p.PartyId,
                }).ToList(),
            }).FirstOrDefaultAsync();
        if (e != null)
        {
            // The number of events specific for a room is not very large since meeting is usually constrained by some timeframe.
            var roomEvents = await
                _DbContext.ConfEvents.AsNoTracking()
                .Where(ce => ce.MeetingId == e.MeetingId)
                .ToListAsync();
            e.VideoLogs = await FetchVideoLogs(e.MeetingId, _DbContext, timeZoneId);
            if (_IUserRepository.IsAdmin())
                e.EventLogs = EventLog(e.MeetingId, allUsers, roomEvents, "en", timeZoneId);
            e.MeetingStatus = _IGeneralRepository.CheckStatus(e.StartDate, e.EndDate, e.Id, e.MeetingId, "en", roomEvents);
            var usersId1 = e.Participants.Select(p => p.UserId).Distinct().ToList();
            usersId1.Add(e.CreatedBy);
            var userName = _DbContext.Users.Where(u => usersId1.Distinct().Contains(u.Id)).Select(x => new { x.Id, x.FullName }).ToDictionary(x => x.Id, x => x.FullName);
            bool b = false;
            b = userName.TryGetValue(e.CreatedBy, out string? userFullName);
            e.CreatedByName = b ? userFullName : string.Empty;
            bool isModerator = e.Participants.Where(x => x.UserId == userId).Select(x => x.IsModerator).FirstOrDefault();
            foreach (var par in e.Participants)
            {
                par.Remind = isModerator && par.UserId != userId ? true : false;
                b = userName.TryGetValue(par.UserId, out userFullName);
                par.FullName = b ? userFullName : string.Empty;
                par.ParticipantStatus = _IGeneralRepository.CheckParticipantStatus(par.Email, e.Id, e.MeetingId, roomEvents, allUsers);
            }
        }
        return e;
    }

    private async Task<List<EventWParticipant>> BuildRecurrenceEventsListAsync(EventWParticipant dto, List<DateTimeRange> dates, bool MeetingRequired, int addBy, string lang)
    {
        var eventsList = new List<EventWParticipant>();
        APIResult meetingResult = new();
        foreach (var d in dates)
        {
            if (MeetingRequired)
            {
                meetingResult = await _IMeetingRepository.AddMeeting(new MeetingPostDto
                {
                    Password = dto.Password,
                    PasswordReq = dto.PasswordReq,
                    RecordingReq = dto.RecordingReq,
                    SingleAccess = dto.SingleAccess,
                    AllDay = dto.AllDay,
                    AutoLobby = dto.AutoLobby,
                    Status = (sbyte)MEETING_STATUS.PENDING,
                    Topic = dto.Topic,
                    TimeZone = dto.TimeZone,
                }, addBy, lang);

                if (meetingResult.Id < 0)
                {
                    return [];
                }
            }
            eventsList.Add(new EventWParticipant
            {
                StartDate = d.StartDateTime,
                EndDate = d.EndDateTime,
                Topic = dto.Topic,
                SubTopic = dto.SubTopic,
                Description = dto.Description,
                AllDay = dto.AllDay,
                MeetingRequired = dto.MeetingRequired,
                Organizer = dto.Organizer,
                PasswordReq = dto.PasswordReq,
                Password = dto.Password,
                TimeZone = dto.TimeZone,
                Type = dto.Type,
                RecordingReq = dto.RecordingReq,
                SingleAccess = dto.SingleAccess,
                AutoLobby = dto.AutoLobby,
                MeetingId = meetingResult.Result,
            });
        }
        return eventsList;
    }
    public async Task<APIResult> AddRecurrenceEvents(EventWParticipant dto, DateTimeOfRule rDates, int addBy, bool sendNotification, string lang)
    {
        APIResult result = new();
        if (rDates.Dates.Any(x => x.StartDateTime >= x.EndDateTime))
        {
            return result.FailMe(-1, "تحقق من تاريخ الحدث");
        }
        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
        var eventsList = await BuildRecurrenceEventsListAsync(dto, rDates.Dates, dto.MeetingRequired, addBy, lang);
        List<Models.Event> RecurrenceEvents = [];
        try
        {
            foreach (var evt in eventsList)
            {
                RecurrenceEvents.Add(new Models.Event
                {
                    ParentEvent = evt.ParentEventId,
                    CreatedBy = addBy,
                    Description = evt.Description,
                    Topic = evt.Topic,
                    SubTopic = evt.SubTopic,
                    Organizer = evt.Organizer,
                    CreatedDate = DateTime.Now,
                    EndDate = evt.EndDate,
                    StartDate = evt.StartDate,
                    MeetingId = evt.MeetingId,
                    AllDay = evt.AllDay,
                    TimeZone = evt.TimeZone,
                    RecStatus = evt.Status == null ? (sbyte)EVENT_STATUS.ACTIVE : dto.Status,
                    AppId = evt.AppId,
                    Type = evt.Type,
                });
            }
            _DbContext.Events.AddRange(RecurrenceEvents);
            await _DbContext.SaveChangesAsync();
            var eventsIds = RecurrenceEvents.Select(x => x.Id).ToList();

            if (dto.Participants.Count != 0)
            {
                var ps = new List<Participant>();
                var pu = await _IParticipantRepository.ParticipantsAsUser(dto.Participants.Where(d => d.Id == null).ToList(), addBy, lang);
                if (pu.Id < 0)
                {
                    return pu;
                }
                foreach (var eId in eventsIds)
                {
                    foreach (var p in dto.Participants)
                    {
                        ps.Add(new Participant
                        {
                            EventId = eId,
                            Description = p.Description,
                            Email = p.Email,
                            IsModerator = p.IsModerator,
                            UserId = (int)p.LocalUserId!,
                            Mobile = p.Mobile,
                            Note = p.Note,
                            Guid = Guid.NewGuid(),
                            CreatedBy = addBy,
                            CreatedDate = DateTime.Now
                        });
                    }
                }
                _DbContext.Participants.AddRange(ps);
                await _DbContext.SaveChangesAsync();
            }

            scope.Complete();
            scope.Dispose();

            var parameters = new Dictionary<string, string>
                 {
                   { FROM_DATE, rDates.Dates[0].StartDateTime.ToString("dd-MM-yyyy")},
                   { TO_DATE, rDates.Dates[0].EndDateTime.ToString("dd-MM-yyyy")},
                   { FROM_TIME, rDates.Dates[0].StartDateTime.ToString("hh:mm tt")},
                   { TO_TIME, rDates.Dates[0].EndDateTime.ToString("hh:mm tt")},
                   { MEETING_ID, dto.MeetingId},
                   { TOPIC, dto.Topic},
                   { TIMEZONE, dto.TimeZone},
                 };

            var receivers = new List<Receiver>();
            foreach (var p in dto.Participants)
            {
                receivers.Add(new Receiver
                {
                    Email = p.Email,
                    Id = p.LocalUserId,
                    Mobile = p.Mobile,
                    //Name = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.FullName).FirstOrDefault(),
                    ParticipantId = p.Id,
                    Tokens = null
                });
            }
            int actionId = await _DbContext.Actions.Where(x => x.Shortcut == SEND_INVITATION_ACTION).Select(x => x.Id).FirstOrDefaultAsync();
            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(actionId, RecurrenceEvents[0].Id);
            var a = await _IParticipantRepository.NotifyParticipants(receivers, RecurrenceEvents[0].MeetingId, toSendNoti, parameters,
                                                                     INVITATION_TEMPLATE, sendNotification, false);

            //if (a == null)
            //{
            //    return result.FailMe(a, "Error sending notification", true);
            //}


            return result.SuccessMe(1, "تمت اضافة الاحداث", true, APIResult.RESPONSE_CODE.CREATED, eventsIds);
        }
        catch
        {
            return result.FailMe(-1, "خطأ في إضافة الأحداث");
        }
    }

    public async Task<APIResult> ShiftRecurrenceEvents(int eventId, int updatedBy, DateTime originStartDate, double daysStart, 
        double daysEnd, double minutsStart, double minutsEnd, string? timeZone, UpdateOption opt, bool updateThis = true, string lang = "ar")
    {
        APIResult result = new();
        var evt = await _DbContext.Events.Where(e => e.Id == eventId).FirstOrDefaultAsync();
        if (evt == null)
        {
            return result.FailMe(-1, "الحدث غير موجود");
        }

        List<Models.Event> events = new();
        switch (opt.GetCurrentStatus())
        {
            case 0:
                events = await _DbContext.Events.Where(e => updateThis && e.Id == eventId).ToListAsync();
                break;
            case 1:
                events = await _DbContext.Events.Where(e => (updateThis || e.Id != eventId) && e.StartDate > DateTime.Now && e.StartDate > originStartDate).ToListAsync();
                break;
            case -1:
                events = await _DbContext.Events.Where(e => (updateThis || e.Id != eventId) && e.StartDate > DateTime.Now && e.StartDate < originStartDate).ToListAsync();
                break;
            case 2:
                events = await _DbContext.Events.Where(e => (updateThis || e.Id != eventId) && e.StartDate > DateTime.Now).ToListAsync();
                break;
            default:

                break;
        }
        foreach (var gEvt in events)
        {
            gEvt.StartDate = gEvt.StartDate.AddDays(daysStart);
            gEvt.EndDate = gEvt.EndDate.AddDays(daysEnd);
            gEvt.StartDate = gEvt.StartDate.AddMinutes(minutsStart);
            gEvt.EndDate = gEvt.EndDate.AddMinutes(minutsEnd);
            if (timeZone != null)
            {
                gEvt.TimeZone = timeZone;
            }
            gEvt.LastUpdatedDate = DateTime.Now;
            gEvt.LastUpdatedBy = updatedBy;
        }
        _DbContext.UpdateRange(events);
        try
        {
            await _DbContext.SaveChangesAsync();
            return result.SuccessMe(1, Translation.getMessage(lang, "sucsessUpdate"));
        }
        catch
        {
            return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
        }
    }

    public async Task<APIResult> Cancel(int eventId, int updatedBy,  string lang)
    {
        APIResult result = new();
        var evt = await _DbContext.Events.Where(e => e.Id == eventId).FirstOrDefaultAsync();
        if (evt == null)
        {
            return result.FailMe(-1, "الحدث غير موجود");
        }

        if(evt.RecStatus == -2)
        {
            return result.FailMe(-1, "حدث ملغي بالفعل");
        }

        evt.RecStatus = (sbyte?) EVENT_STATUS.CANCELED;
        evt.LastUpdatedBy = updatedBy;
        evt.LastUpdatedDate = DateTime.Now;

        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            _DbContext.Events.Update(evt);
            await _DbContext.SaveChangesAsync();

            await NotifyParticipantsCancel(eventId, updatedBy, lang);

            scope.Complete();

            return result.SuccessMe(eventId, "تم إلغاء الحدث");

        }

        catch
        {
            return result.FailMe(-1, "خطأ في إلغاء الحدث");
        }

    }

    public async Task<APIResult> NotifyParticipantsCancel(int eventId, int addBy, string lang)
    {
        APIResult result = new();
        var receivers = new List<Receiver>();
        var participants = await _DbContext.Participants.Include(e => e.Event).Where(x => x.EventId == eventId).AsNoTracking().ToListAsync();

        

        var notificationsDto = new List<NotificationLogPostDto>();

        try
        {

            if (participants.Count != 0)
            {
                foreach (var participant in participants)
                {

                    int emailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = emailChannel,
                        UserId = participant.UserId,
                        Lang = "ar",/*defLang.Trim().ToLower()*/
                        NotificationTitle = Constants.OTP_TITLE_AR,
                        NotificationBody = Constants.CANCEL_BODY_AR + participant.Event.MeetingId,
                        ToAddress = participant.Email,
                        EventId = eventId,
                        Template = Constants.DEFAULT_TEMPLATE,
                        CreatedBy = addBy
                    });

                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = emailChannel,
                        UserId = participant.UserId,
                        Lang = "en",
                        NotificationTitle = Constants.OTP_TITLE_EN,
                        NotificationBody = Constants.CANCEL_BODY_EN + participant.Event.MeetingId,
                        ToAddress = participant.Email,
                        EventId = eventId,
                        Template = Constants.DEFAULT_TEMPLATE,
                        CreatedBy = addBy
                    });
                }


                await _ISendNotificationRepository.DoSend(notificationsDto, true, true, null);
                
                return result.SuccessMe(eventId, "تم إشعار الاطراف");
            }

            else
            {
                return result.FailMe(-1, "الحدث غير موجود");
            }

        } 

        catch
        {
            return result.FailMe(-1, "حدث خطأ في إرسال الاشعار ", true);
        }
    }

    private static string EventTypeToStatusText(ConfEvent ev, string lang)
        {
            int langIndex = lang == "ar" ? 0 : 1;
            if (!MeetingStatusValue.TryGetValue(ev.EventType, out string[]? value))
                return "";
            return value[langIndex];
        }

    private static List<ConfEventCompactGet> EventLog(string meetingId, List<ConfUser> allUsers, List<ConfEvent> allRooms, string lang, string timeZoneId)
    {
        List<ConfEventCompactGet> eventLogs = allRooms
            .OrderByDescending(o => o.Id)
            .Where(x => x.MeetingId == meetingId)
            .Select(s => new ConfEventCompactGet
            {
                EventTime = timeZoneId != null ? TimeConverter.ConvertFromUtc(s.EventTime, timeZoneId) : s.EventTime,
                Status = EventTypeToStatusText(s, lang),
                UserName = s.UserId,
                KickedBy = s.EventInfo.Split("<")[0] != "" ? s.EventInfo.Split("<")[0] : null,
            })
        .ToList();


        foreach (var eventLog in eventLogs)
        {
            var userName = allUsers.Where(e => e.Id.ToString().Equals(eventLog.UserName)).Select(o => o.Name).FirstOrDefault();

            eventLog.UserName = userName != null ? userName : "Room";
        }

        return eventLogs;
    }

    private static async Task<List<RecordingLog>> FetchVideoLogs(string meetingId, OraDbContext dbContext, string? timeZoneId)
    {
        // FIXME: we need meeting_id in table RecordingLogs or at least index for RecordingLogs.RecordingFileName
        var videoRecords = await dbContext.RecordingLogs.Where(rl => rl.RecordingfileName.StartsWith(meetingId)).ToListAsync();
        foreach (var log in videoRecords)
        {
            var logTime = log.CreatedDate;

            log.CreatedDate = timeZoneId != null ? TimeConverter.ConvertFromUtc(logTime, timeZoneId) : logTime;

        }
        return videoRecords;
    }
}
