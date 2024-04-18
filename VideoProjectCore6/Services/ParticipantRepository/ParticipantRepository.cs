using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IParticipantRepository;
using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.DTOs.NotificationDto;
using static VideoProjectCore6.Services.Constants;
using Flurl;
using VideoProjectCore6.Repositories.IEventLogRepository;
using Newtonsoft.Json;
using System.Text;
using VideoProjectCore6.DTOs;
using System.Linq;
using VideoProjectCore6.Utilities.Time;

namespace VideoProjectCore6.Services.ParticipantRepository;
#nullable disable
public class ParticipantRepository : IParticipantRepository
{
    private readonly OraDbContext _DbContext;
    private readonly IUserRepository _IUserRepository;
    //private readonly IWorkRepository _IWorkRepository;
    private readonly ISendNotificationRepository _ISendNotificationRepository;
    private readonly INotificationSettingRepository _INotificationSettingRepository;
    private readonly IConfiguration _IConfiguration;
    private readonly IEventLogRepository _IEventLogRepository;
    public ParticipantRepository(OraDbContext OraDbContext, IUserRepository iUserRepository, ISendNotificationRepository iNotificationRepository, /*IWorkRepository iWorkRepository,*/ IConfiguration iConfiguration
        , INotificationSettingRepository iNotificationSettingRepository, IEventLogRepository iEventLogRepository)
    {
        _DbContext = OraDbContext;
        _IUserRepository = iUserRepository;
        _ISendNotificationRepository = iNotificationRepository;
        //_IWorkRepository = iWorkRepository;
        _IConfiguration = iConfiguration;
        _INotificationSettingRepository = iNotificationSettingRepository;
        _IEventLogRepository = iEventLogRepository;

    }

    public async Task<List<ParticipantSearchView>> SearchParticipant(string toSearch)
    {
        List<ParticipantSearchView> result = new List<ParticipantSearchView>();
        IQueryable<User> users = Enumerable.Empty<User>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(toSearch))
        {
            toSearch = toSearch.Trim().ToLower();
            users = double.TryParse(toSearch, out _) ? _DbContext.Users.Where(x => x.PhoneNumber != null && x.PhoneNumber.Contains(toSearch)).AsNoTracking()
                 : _DbContext.Users.Where(x => x.Email != null && x.Email.ToLower().Contains(toSearch) || x.FullName != null && x.FullName!.Contains(toSearch)).AsNoTracking();
        }
        result = await users.Select(x => new ParticipantSearchView { Id = x.Id, Email = x.Email, Mobile = x.PhoneNumber, Name = x.FullName }).ToListAsync();
        return result;
    }

    public async Task<APIResult> AddParticipants(List<ParicipantDto> dtos, int addBy, string lang)
    {
        var Participants = new List<Participant>();
        List<Receiver> receivers = new();
        APIResult result = new();
        foreach (ParicipantDto dto in dtos)
        {
            Participants.Add(new()
            {
                EventId = dto.EventId,
                CreatedBy = addBy,
                UserId = (int)dto.LocalUserId,
                Description = dto.Description,
                Note = dto.Note,
                Email = dto.Email,
                Mobile = dto.Mobile,
                CreatedDate = DateTime.Now,
                Guid = Guid.NewGuid(),
                IsModerator = dto.IsModerator,
                RecStatus = 1,
            });
        }
        try
        {
            _DbContext.Participants.AddRange(Participants);
            await _DbContext.SaveChangesAsync();
            foreach (var p in Participants)
            {
                receivers.Add(new Receiver
                {
                    Email = p.Email,
                    Id = p.UserId,
                    Mobile = p.Mobile,
                    Name = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.FullName).FirstOrDefault(),
                    ParticipantId = p.Id,
                    Tokens = null,
                    IsModerator = p.IsModerator
                });
            }

        }
        catch (Exception ex)
        {
            var m = ex.Message != null ? ex.Message : "";
            var m2 = "";
            if (ex.InnerException != null)
            {
                m2 = ex.InnerException.Message;
            }
            return result.FailMe(-1, "Error adding participants: " + m + "***" + m2);
        }
        return result.SuccessMe(1, Translation.getMessage(lang, "Done"), true, APIResult.RESPONSE_CODE.OK, receivers);
    }

    public async Task<List<MeetingUserLink>> NotifyParticipants(List<Receiver> participants, string mettingId, List<NotificationLogPostDto> notifications_,
        Dictionary<string, string> parameters, string template, bool send, bool isDirectInvitation ,string cisco=null)
    {
        foreach (var n in notifications_)
        {
            n.LinkCaption = Translation.getMessage(n.Lang, "Join");
            n.Template = template;
        }
        var a = await _ISendNotificationRepository.FillAndSendNotification(notifications_, participants, parameters, mettingId, true, "ar", send, isDirectInvitation,cisco);
        return a;
    }

    public async Task<APIResult> ReNotifyParticipant(int id, string email, string lang)
    {

        var participant = await _DbContext.Participants.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

        return await Notify(participant, email, lang);

    }

    public async Task<APIResult> Notify(Participant participant, string email, string lang)
    {
        APIResult result = new();
        var receivers = new List<Receiver>();
        //var participant = await _DbContext.Participants.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        if (participant != null)
        {
            var secondEmail = false;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var check = await _IUserRepository.CheckEMailAddress(email, lang);
                if (check.Id < 0) return check;
                else secondEmail = true;
            }

            var receiver = new Receiver
            {
                Email = secondEmail ? email : participant.Email,
                Id = participant.UserId,
                Mobile = participant.Mobile,
                Name = string.Empty,
                ParticipantId = participant.Id,
                Tokens = null
            };
            receivers.Add(receiver);
            var e = await _DbContext.Events.FindAsync(participant.EventId);
            var parameters = new Dictionary<string, string>
                 {
                   { FROM_DATE, TimeConverter.ConvertFromUtc(e.StartDate, e.TimeZone).ToString("dd-MM-yyyy")},
                   { TO_DATE, TimeConverter.ConvertFromUtc(e.EndDate, e.TimeZone).ToString("dd-MM-yyyy")},
                   { FROM_TIME, TimeConverter.ConvertFromUtc(e.StartDate, e.TimeZone).ToString("HH:mm")},
                   { TO_TIME, TimeConverter.ConvertFromUtc(e.EndDate, e.TimeZone).ToString("HH:mm")},
                   { TOPIC, e.Topic},
                   { TIMEZONE, e.TimeZone},
                   {MEETING_ID ,e.MeetingId},
                 };
            int envitationActionId = await _DbContext.Actions.Where(x => x.Shortcut == SEND_INVITATION_ACTION).Select(x => x.Id).FirstOrDefaultAsync();
            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(envitationActionId, e.Id);
            var a = await NotifyParticipants(receivers, e.MeetingId, toSendNoti, parameters, INVITATION_TEMPLATE, true, false, null);
            //-------------***********--------Temp code to send sms
            //APIResult smsResult = new();
            //if (!string.IsNullOrWhiteSpace(receiver.Mobile))
            //{
            //    smsResult = await InvokeSMSService(new SMSDto
            //    {
            //        pLanguage = "AR1",
            //        pMobileNO = a[0].Mobile,
            //        pMessage_TEXT = " يرجى الانضمام للاجتماع من خلال الرابط : " + a[0].MeetingLink,
            //    });
            //}
            if (a == null/* && smsResult.Id<0*/)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "InvitationNotSent"), true);
            }
            else
            {
                return result.SuccessMe(1, Translation.getMessage(lang, "InvitationSent"));
            }
        }
        else
        {
            return result.FailMe(-1, Translation.getMessage(lang, "partyNotFound"));
        }
    }

    public async Task<APIResult> ParticipantsAsUser(List<ParicipantDto> participants, int addBy, string lang)
    {
        APIResult result = new();
        foreach (var p in participants)
        {
            if (p.LocalUserId == null)
            {
                if (!string.IsNullOrWhiteSpace(p.Email))
                {
                    var uName = await _IUserRepository.CheckEMailAddress(p.Email, lang);
                    if (uName.Id < 0)
                    {
                        return uName;
                    }
                    var u = await _IUserRepository.Search(p.Email);
                    if (u != null)
                    {
                        p.LocalUserId = u.Id;
                        p.FullName = u.FullName;
                        p.Mobile = u.PhoneNumber;
                    }
                    else
                    {
                        result = await _IUserRepository.CreateUser(
                        new UserPostDto
                        {
                            FullName = string.IsNullOrWhiteSpace(p.FullName) ? uName.Result : p.FullName,
                            UserName = p.Email,
                            Email = p.Email,
                            PhoneNumber = p.Mobile,
                            PasswordHash = "P@ssw0rd_"
                        }, null, false, "ar");

                        if (result.Id > 0)
                        {
                            p.LocalUserId = result.Id;
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
            }
        }
        return result.SuccessMe(participants.Count(), "Done", true);
    }
    public async Task<APIResult> Delete(int id, int deletedBy, string lang)
    {
        var result = new APIResult();
        var participant = await _DbContext.Participants.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (participant == null)
        {
            return result.FailMe(-1, Translation.getMessage(lang, "partyNotFound"));
        }
        try
        {
            _DbContext.Participants.Remove(participant);
            await _DbContext.SaveChangesAsync();
            int actionId = await _DbContext.Actions.Where(x => x.Shortcut == DELETE_PARTICIPANT_ACTION).Select(x => x.Id).FirstOrDefaultAsync();
            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(actionId, participant.EventId);
            var e = await _DbContext.Events.Where(e => e.Id == participant.EventId).FirstOrDefaultAsync();
            //var addLog = await _IEventLogRepository.AddEventLog(new Models.EventLog
            //{
            //    ActionId = (short)actionId,
            //    CreatedBy = deletedBy,
            //    CreatedDate = DateTime.Now,
            //    EventId = e.Id,
            //    ObjectType = Constants.OPJECT_TYPE_PARTICIPANT,
            //    RelatedId = id,
            //    Note = participant.Email,
            //}, lang);
            //if (addLog.Id < 0)
            //{
            //    return addLog;
            //}
            var parameters = new Dictionary<string, string> { { TOPIC, e.Topic } };
            var a = await NotifyParticipants(new List<Receiver>() { new Receiver { Email = participant.Email, ParticipantId = id } }, e.MeetingId, toSendNoti, parameters, UNSUBSCRIBE_TEMPLATE, true, false);

            return result.SuccessMe(id, Translation.getMessage(lang, "sucsessDelete"), true, APIResult.RESPONSE_CODE.OK);

        }
        catch
        {
            return result.FailMe(-1, Translation.getMessage(lang, "PartyDeleteFail"));
        }
    }
    public async Task<APIResult> Update(int id, ParicipantDto dto, int updatedBy, string lang)
    {
        APIResult result = new APIResult();
        Participant participant = await _DbContext.Participants.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (participant == null)
        {
            return result.FailMe(-1, "No participant found");
        }
        //participant.Email = dto.Email;
        //participant.Mobile = dto.Mobile;
        participant.LastUpdatedBy = updatedBy;
        participant.LastUpdatedDate = DateTime.Now;
        participant.IsModerator = dto.IsModerator;
        participant.Description = dto.Description;
        participant.Note = dto.Note;
        try
        {
            _DbContext.Participants.Update(participant);
            await _DbContext.SaveChangesAsync();
            //var l = await _IEventLogRepository.AddEventLog(new Models.EventLog
            //{
            //    ActionId = (short)await _DbContext.Actions.Where(x => x.Shortcut == UPDATE_PARTICIPANT_ACTION).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync(),
            //    EventId = participant.EventId,
            //    CreatedBy = updatedBy,
            //    CreatedDate = DateTime.Now,
            //    Note = participant.Email,
            //    ObjectType = OPJECT_TYPE_PARTICIPANT,
            //    RelatedId = id
            //}, lang);
            //if (l.Id < 0)
            //{
            //    return l;
            //}
            return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
        }
        catch
        {
            return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
        }
    }

    public async Task<APIResult> Liberate(int participantId, int updatedBy, string lang)
    {
        APIResult result = new();
        var participant = await _DbContext.Participants.Where(x => x.Id == participantId).FirstOrDefaultAsync();
        if (participant == null)
        {
            return result.FailMe(-1, "المشترك غير موجود");
        }
        participant.GroupIn = null;
        participant.LastUpdatedBy = updatedBy;
        participant.LastUpdatedDate = DateTime.Now;
        try
        {
            _DbContext.Update(participant);
            await _DbContext.SaveChangesAsync();
            return result.SuccessMe(participantId, "تم التعديل بنجاح");
        }
        catch (Exception)
        {
            return result.FailMe(-1, "خطأ في التعديل");
        }

    }
    public async Task<APIResult> UpdateNote(int id, string note, int updatedBy, string lang)
    {
        APIResult result = new APIResult();
        Participant participant = await _DbContext.Participants.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (participant == null)
        {
            return result.FailMe(-1, "No participant found");
        }
        participant.Note = note;
        participant.LastUpdatedDate = DateTime.Now;
        participant.LastUpdatedBy = updatedBy;
        try
        {
            _DbContext.Participants.Update(participant);
            await _DbContext.SaveChangesAsync();
            return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
        }
        catch
        {
            return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
        }
    }
}
