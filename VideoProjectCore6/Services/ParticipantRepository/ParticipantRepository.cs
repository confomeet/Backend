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
    public async Task<List<ParticipantSearchView>> SearchByWork(int workId, string lang)
    {
        List<ParticipantSearchView> result = new List<ParticipantSearchView>();
        return await (from w in _DbContext.PartyWorks
                      join u in _DbContext.Users
                      on w.PartyId equals u.Id
                      where w.WorkId == workId
                      select new ParticipantSearchView
                      { Id = u.Id, Email = u.Email, Mobile = u.PhoneNumber, Name = u.FullName }).ToListAsync();

    }
    public async Task<List<ParticipantSearchView>> SearchBySpeciality(int specialityId, string lang)
    {
        List<ParticipantSearchView> result = new List<ParticipantSearchView>();
        return await (from s in _DbContext.PartyWorkSpecialities
                      join pw in _DbContext.PartyWorks
                      on s.PartyWorkId equals pw.Id
                      join u in _DbContext.Users
                      on pw.PartyId equals u.Id
                      where s.SpecialityId == specialityId
                      select new ParticipantSearchView
                      {
                          Id = u.Id,
                          Email = u.Email,
                          Mobile = u.PhoneNumber,
                          Name = u.FullName
                      }).ToListAsync();
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
                UserType = dto.UserType,
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
        return result.SuccessMe(1, "Done", true, APIResult.RESPONSE_CODE.OK, receivers);
    }
    //--Add PP participants
    //--Parameter ToExistEvent : Used to check if there is already a cabin 
    public async Task<APIResult> AddParticipants(List<ParticipantWParent> dtos, int eventId, int addBy, string lang, List<EntityCabine> toUseCabins, DateTimeRange range, string meetingId, bool checkDate = false, bool checkWorkTime = false, bool toExistEvent = false)
    {
        List<string> errorMsgs = new List<string>();
        APIResult result = new();
        if (toExistEvent)
        {
            var oldParticipants = await _DbContext.Participants.Where(p => p.EventId == eventId).ToListAsync();
            List<int> oldCabins = oldParticipants.Where(x => x.GroupIn != null).Select(x => (int)x.GroupIn).Distinct().ToList();
            //dtos.RemoveAll(x => x.UserId != null && oldParticipants.Select(y => y.PartyId).ToList().Contains(x.UserId));

            var oldIds=oldParticipants.Where(y=>y.PartyId!=null).Select(y => y.PartyId).Distinct().ToList();
            foreach (var newParty in dtos.ToList())
            {
                if (newParty.UserId != null && oldIds.Contains(newParty.UserId))
                {
                    //var currnetOldParticipant = oldParticipants.Where(o => o.PartyId == newParty.UserId).FirstOrDefault();
                    //if (dtos != null && newParty.EntityType != null && currnetOldParticipant.GroupIn == null)
                    //{
                    //    var cabins = await _IWorkRepository.GetUsersOfEntityByWork((uint)newParty.EntityId, (int)newParty.EntityType, "WORK_CABINET");
                    //    var common = cabins.Select(x => x.Id).ToList().Intersect(oldCabins).ToList();
                    //    if (common.Any())// the required cabine is already exist
                    //    {
                            
                    //        foreach (var pnr in dtos.Where(x => x.EntityId == newParty.EntityId && x.EntityType == newParty.EntityType).ToList())
                    //        {// Set prisoner in cabine no need to entity anymore 
                    //            pnr.GroupIn = common.First();
                    //            pnr.EntityId = null;//To not search cabin for this participant again
                    //            pnr.EntityType = null;
                    //        }
                    //    currnetOldParticipant.GroupIn= common.First();
                    //    currnetOldParticipant.LastUpdatedBy = addBy;
                    //    currnetOldParticipant.LastUpdatedDate = DateTime.Now;
                    //    _DbContext.Update(currnetOldParticipant);
                    //    dtos.Remove(newParty);
                    //    }
                        
                    //}
                    //else
                    //{
                        dtos.Remove(newParty);
                    //}
                }
            }
            await _DbContext.SaveChangesAsync();

            //var prisoners = dtos.Where(x => x.EntityId != null && x.EntityType != null).ToList();
            //if (prisoners.Any())
            //{
            //    foreach (var prisoner in prisoners)
            //    {
            //        if (prisoner.GroupIn == null)// initially this value is null but may filled inside loop
            //        {
            //            var cabins = await _IWorkRepository.GetUsersOfEntityByWork((uint)prisoner.EntityId, (int)prisoner.EntityType, "WORK_CABINET");
            //            var common = cabins.Select(x => x.Id).ToList().Intersect(oldCabins).ToList();
            //            if (common.Any())// the required cabine is already exist
            //            {
            //                foreach (var pnr in prisoners.Where(x => x.EntityId == prisoner.EntityId && x.EntityType == prisoner.EntityType).ToList())
            //                {// Set prisoner in cabine no need to entity anymore 
            //                    pnr.GroupIn = common.First();
            //                    pnr.EntityId = null;//To not search cabin for this participant again
            //                    pnr.EntityType = null;
            //                }
            //            }
            //        }
            //    }
            //}
        }
        //var ptc = dtos.Where(p => !string.IsNullOrEmpty(p.Email)).Select(c => c.Email).ToList();
        //if (ptc.Count() != ptc.Distinct().Count())
        //{
        //    return result.FailMe(-1, Translation.getMessage(lang, "SameEmail"));
        //}
        //--------STOP CHECK DUBLICATE EMIRATE ID OR UUID--------------
        //ptc = dtos.Where(p => !string.IsNullOrEmpty(p.EmiratesId)).Select(c => c.EmiratesId).ToList();
        //if (ptc.Count() != ptc.Distinct().Count())
        //{
        //    return result.FailMe(-1, Translation.getMessage(lang, "SameEmiratesId"));
        //}
        //ptc = dtos.Where(p => !string.IsNullOrEmpty(p.UUID)).Select(c => c.UUID).ToList();
        //if (ptc.Count() != ptc.Distinct().Count())
        //{
        //    return result.FailMe(-1, Translation.getMessage(lang, "SameUUIDId"));
        //}
        List<string> departments = new();
        List<Participant> ps = new();
        List<Receiver> receivers = new();
        List<ParticipantWParent> Workdtos = new();
        var userGroupDic = new Dictionary<int, int>();
        var CabinDic = new Dictionary<string, int>();
        foreach (var dto in dtos)
        {
            if (dto.LocalUserId == null)
            {
                //--------------------- CHECK EMAIL IF NOT CORRECT AND CLEAR IT---------
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var addr = _IUserRepository.CheckEMailAddress(dto.Email, lang);
                    if (addr.Id < 0)
                    {
                        dto.Email = null;
                    }
                }
                //---------------------------
                APIResult u = await _IUserRepository.CreateParticipantUser(
                    new BasicUserInfo
                    {
                        FullName = dto.FullName,
                        //UserId = dto.UserId,
                        UserType = dto.UserType,
                        UserName = dto.Email != null ? dto.Email : dto.Mobile,
                        Email = dto.Email,
                        PhoneNumber = dto.Mobile,
                        EmiratesId = dto.EmiratesId,
                        UUID = dto.UUID,
                        PasswordHash = "P@ssw0rd_"
                    }, "ar", true);
                if (u.Id > 0)
                {
                    dto.LocalUserId = u.Id;
                    if (string.IsNullOrEmpty(dto.Email))
                    {
                        dto.Email = u.Result;
                    }
                }
                else
                {
                    //****___________Add error message instead of exit__________
                    errorMsgs.Add(dto.UserId.ToString() + " خطأ في إضافة المشترك كمستخدم  |" + u.Message[0]);
                    continue;
                }
            }
            else
            {
                if (checkDate && !await isAvalableDateRange(new DateTimeRange { StartDateTime = range.StartDateTime, EndDateTime = range.EndDateTime }, (int)dto.LocalUserId))
                {
                    return result.FailMe(-1, Translation.getMessage(lang, "NotAvailableDateRange") + " : " + dto.FullName);
                }
            }


            //if (dto.LocalUserId != null && string.IsNullOrEmpty(dto.Email))
            //{
            //    var user = await _DbContext.Users.Where(x => x.Id == dto.LocalUserId).AsNoTracking().Select(x => new { Email = x.Email, Mobile = x.PhoneNumber, UserId = x.UserId, UserType = x.UserType, EmiratesId = x.EmiratesId }).FirstOrDefaultAsync();
            //    dto.Email = user.Email;
            //    dto.UserId = user.UserId;
            //    dto.UserType = user.UserType;
            //    dto.EmiratesId = user.EmiratesId;
            //}


            //if (dto.EntityId != null && dto.EntityType != null)
            //{
            //    var e = ((int)dto.EntityId).ToString() + "-" + ((int)dto.EntityType).ToString();
            //    var c = toUseCabins==null || !toUseCabins.Any() ? null: toUseCabins.Where(x => x.EntityId == dto.EntityId && x.EntityType == dto.EntityType).FirstOrDefault();
            //    if (c !=null /*&& !departments.Contains(e)*/)
            //    {
            //        dto.GroupIn = c.CabineId;
            //        if (!departments.Contains(e))
            //        { 
            //         departments.Add(e);
            //         ps.Add(new Participant
            //            {
            //                Guid = Guid.NewGuid(),
            //                CreatedBy = addBy,
            //                CreatedDate = DateTime.Now,
            //                Email = c.Email,
            //                Mobile = c.Mobile,
            //                EventId = eventId,
            //                IsModerator = false,
            //                UserId = c.CabineId,
            //                UserType=c.UserType,
            //                Note=c.Note,
            //            });
            //        }
            //    }
            //    else
            //    {
            //        //var e = ((int)dto.EntityId).ToString() + "-" + ((int)dto.EntityType).ToString();
            //        if (!departments.Contains(e))
            //        {
            //            var p = await getUserByWorkAsParticipant(eventId, "WORK_CABINET", addBy, /*checkWorkTime ?*/ range /*: null*/, checkWorkTime, new OuterUser { UserId = (uint)dto.EntityId, UserType = (int)dto.EntityType }, lang);
            //            if (p != null)
            //            {
            //                //p.Description = p.Description + " " + dto.Email;
            //                var pt = p.asParticipant();
            //                pt.Guid = Guid.NewGuid();
            //                pt.CreatedBy = addBy;
            //                pt.CreatedDate = DateTime.Now;
            //                ps.Add(pt);
            //                Workdtos.Add(new ParticipantWParent
            //                {
            //                    LocalUserId = p.LocalUserId,
            //                    UserId = p.UserId,
            //                    UserType = p.UserType,
            //                });
            //                departments.Add(e);
            //                CabinDic.Add(e, (int)p.LocalUserId);
            //                try
            //                {
            //                    //------------   occure when the same user is duplicate as a participant  *** TO CHECK ***
            //                    userGroupDic.Add((int)dto.LocalUserId, (int)p.LocalUserId);
            //                }
            //                catch { }
            //                toUseCabins.Add(new EntityCabine { 
            //                    EntityId = (int)dto.EntityId,
            //                    EntityType = (int)dto.EntityType,
            //                    CabineId = (int)p.LocalUserId,
            //                    Email=p.Email,
            //                    Mobile= p.Mobile,
            //                    UserType=p.UserType,
            //                    Note=p.Note,
            //                });

            //            }
            //            else
            //            {
            //                //return result.FailMe(-1, Translation.getMessage(lang, "NoAvailableCabin"));
            //                errorMsgs.Add($"{ Translation.getMessage(lang, "NoAvailableCabin")} >> entityId {dto.EntityId} , entityType {dto.EntityType}");
            //            }
            //        }
            //        else
            //        {
            //            int v;                        
            //            try
            //            {
            //                userGroupDic.Add((int)dto.LocalUserId, CabinDic.TryGetValue(e, out v) ? v : 0);
            //            }
            //            catch
            //            {
            //            }
            //        }
            //    }
            //}
            var newPart = new Participant
            {
                EventId = eventId,
                UserId = (int)dto.LocalUserId,
                UserType = dto.UserType,
                CreatedBy = addBy,
                CreatedDate = DateTime.Now,
                Guid = Guid.NewGuid(),
                Note = dto.FullName+" "+dto.Note,
                Description = dto.Description,
                IsModerator = dto.IsModerator,
                Email = dto.Email,
                Mobile = dto.Mobile,
                PartyId = dto.UserId,//------- UserId as it come from PP Request
                GroupIn = dto.GroupIn,
                Charge=dto.Charge,
            };

            ps.Add(newPart);
        }
        dtos.AddRange(Workdtos);
        try
        {
            int v;

            foreach (var o in ps)
            {
                if (o.GroupIn == null)
                {
                    o.GroupIn = userGroupDic.TryGetValue(o.UserId, out v) ? v : null;
                }
                //var gIn = userGroupDic.TryGetValue(o.UserId, out v) ? v : 0;
                //if (v != 0)
                //{
                //    o.GroupIn = ps.Where(x => x.UserId == v).Select(x => x.Id).FirstOrDefault();
                //}
            }
            _DbContext.Participants.AddRange(ps);
            await _DbContext.SaveChangesAsync();
            foreach (var p in ps)
            {
                receivers.Add(new Receiver
                {
                    Email = p.Email,
                    Id = p.UserId,
                    Mobile = p.Mobile,
                    Name = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.FullName).FirstOrDefault(),
                    ParticipantId = p.Id,
                    //UserId = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.UserId).FirstOrDefault(),
                    UserType = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.UserType).FirstOrDefault(),
                    EmiratesId = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.EmiratesId).FirstOrDefault(),
                    UuId = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.UUID).FirstOrDefault(),
                    Tokens = null,
                    UserId = dtos.Where(d => d.LocalUserId == p.UserId).Select(c => c.UserId).FirstOrDefault(),
                    IsModerator = p.IsModerator,
                    Charge=p.Charge
                });
            }
            //if (errorMsgs.Any()) result.Message.AddRange(errorMsgs);
            //result.SuccessMe(1, Translation.getMessage(lang, "sucsessAdd"), false, APIResult.RESPONSE_CODE.OK, receivers);
        }
        catch (Exception ex)
        {
            var m = ex.Message != null ? ex.Message : "";
            var m2 = "";
            if (ex.InnerException != null)
            {
                m2 = ex.InnerException.Message;
            }
            return result.FailMe(1, Translation.getMessage(lang, "ParticipantAddFail"), true, APIResult.RESPONSE_CODE.ERROR);
        }
        if (errorMsgs.Any()) result.Message.AddRange(errorMsgs);
        return result.SuccessMe(1, Translation.getMessage(lang, "sucsessAdd"), false, APIResult.RESPONSE_CODE.OK, receivers);
    }
    private async Task<bool> isAvalableDateRange(DateTimeRange range, int userId)
    {
        var res = await (from p in _DbContext.Participants
                         join e in _DbContext.Events
                         on p.EventId equals e.Id
                         where p.UserId == userId && ((e.StartDate < range.EndDateTime && range.StartDateTime < e.EndDate) || e.EndDate > range.StartDateTime && range.EndDateTime > e.StartDate)
                         select new EventGetDto
                         {
                             Id = e.Id,
                             UserId = p.UserId,
                             Topic = e.Topic,
                             Description = e.Description,
                             StartDate = e.StartDate,
                             EndDate = e.EndDate,
                             MeetingId = e.MeetingId,
                             Type = e.Type,

                         }).ToListAsync();
        return res.Count() == 0;
    }
    //private async Task<OuterParticipant> getUserByWorkAsParticipant(int eventId, string workType, int addBy, DateTimeRange range, bool checkAvailability, OuterUser entity, string lang)
    //{
    //    int cabinetWorkId = await _DbContext.Works.AsNoTracking().Where(x => x.Shorcut == workType).Select(w => w.Id).FirstOrDefaultAsync();
    //    var cabinets = await _IWorkRepository.GetAvailableUserByWork(cabinetWorkId, range, checkAvailability, 0, entity, lang);
    //    if (cabinets.Id > 0)
    //    {
    //        var random = new Random();
    //        int index = random.Next(cabinets.Result.Count);
    //        int cabinetId = cabinets.Result[index].Id;
    //        var cabinetUser = _DbContext.Users.Where(x => x.Id == cabinetId).FirstOrDefault();
    //        return new OuterParticipant()
    //        {
    //            EventId = eventId,
    //            Email = cabinetUser.Email,
    //            LocalUserId = cabinetUser.Id,
    //            MeetingToken = "",
    //            Note = cabinetUser.EntityId == null ? "" : await _DbContext.Users.Where(x => x.Id == cabinetUser.EntityId).Select(x => x.FullName).FirstOrDefaultAsync(),
    //            Description = cabinetUser.FullName,
    //            IsModerator = false,
    //            UserId = cabinetUser.UserId,
    //            UserType = cabinetUser.UserType,
    //            Mobile = !string.IsNullOrWhiteSpace(cabinetUser.PhoneNumber) ? cabinetUser.PhoneNumber : !string.IsNullOrWhiteSpace(cabinetUser.TelNo) ? cabinetUser.TelNo : string.Empty
    //        };
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
    public async Task<List<MeetingUserLink>> NotifyParticipants(List<Receiver> participants, string? mettingId, List<NotificationLogPostDto> notifications_,
        Dictionary<string, string> parameters, string template, bool send, bool isDirectInvitation ,string? cisco=null)
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
            //Check if participant with cisco -------
            string ciscoLink = null;
            string ciscoPass = null;
            var meetingId = await _DbContext.Events.Where(x => x.Id == participant.EventId).Select(x => x.MeetingId).FirstOrDefaultAsync();
            if (meetingId.ToUpper().Contains("C"))
            {
                var ciscoData = await _DbContext.Meetings.Where(m => m.MeetingId == meetingId).Select(m => new { m.Password, m.MeetingLog }).FirstOrDefaultAsync();
                if (ciscoData != null)
                {
                    ciscoLink = ciscoData.MeetingLog;
                    ciscoPass = ciscoData.Password;
                }
            }
            //--------------------------------------
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
                   { FROM_DATE, e.StartDate.ToString("dd-MM-yyyy")},
                   { TO_DATE, e.EndDate.ToString("dd-MM-yyyy")},
                   { FROM_TIME, e.StartDate.ToString("hh:mm tt")},
                   { TO_TIME, e.EndDate.ToString("hh:mm tt")},
                   { TOPIC, e.Topic},
                   { TIMEZONE, e.TimeZone},
                   {PASSCODE ,ciscoPass},
                   {MEETING_ID ,e.MeetingId},
                 };
            int envitationActionId = await _DbContext.Actions.Where(x => x.Shortcut == SEND_INVITATION_ACTION).Select(x => x.Id).FirstOrDefaultAsync();
            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(envitationActionId, e.Id);
            var a = await NotifyParticipants(receivers, e.MeetingId, toSendNoti, parameters, INVITATION_TEMPLATE, true, false, ciscoLink);
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
                return result.FailMe(-1, "حدث خطأ في إرسال الاشعار ", true);
            }
            else
            {
                return result.SuccessMe(1, "تم إشعار المستخدم");
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
                        }, null, false, "ar", false);

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
    public async Task<APIResult> ParticipantsAsUser(List<OuterParticipant> participants, int addBy, string lang)
    {
        APIResult result = new();
        foreach (var p in participants)
        {
            if (p.LocalUserId == null)
            {
                if (!string.IsNullOrWhiteSpace(p.Email))
                {
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
                            FullName = p.FullName,
                            UserName = p.Email,
                            Email = p.Email,
                            PhoneNumber = p.Mobile,
                            PasswordHash = "P@ssw0rd_"
                        }, null, false, "ar", false);

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
        catch (Exception e)
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
