using Flurl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OtpNet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.JWTDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;

using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IUserRepository;

using VideoProjectCore6.Services.Meeting;
namespace VideoProjectCore6.Services.NotificationService
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public List<UserInfoDetails> Adresses { get; set; } = [];
        public string UserName { get; set; } = "";
    }

    public class UserInfoDetails
    {
        public int ChannelNo { get; set; }
        public string Address { get; set; } = "";
    }

    public class SendNotificationRepository : ISendNotificationRepository
    {
        private readonly IOptions<ChannelMailFirstSetting> _mailSetting;
        private readonly IOptions<ChannelSMSSetting> _smsSetting;
        private readonly OraDbContext _DbContext;
        private readonly INotificationLogRepository _iNotificationLogRepository;
        private readonly ILogger<SendNotificationRepository>? _logger;
        private readonly ILogger<MeetingRepository>? _loggerM;
        private readonly IGeneralRepository _generalRepository;
        private readonly IConfiguration? _configuration;
        private readonly IUserRepository? _iUserRepository;


        private readonly ValidatorException? _exception;
        private readonly IFilesUploaderRepository? _IFilesUploaderRepository;


        public SendNotificationRepository(OraDbContext DBContext,
            IOptions<ChannelMailFirstSetting> mailSettings,
            IOptions<ChannelSMSSetting> smsSettings,
            INotificationLogRepository iNotificationLogRepository,
            ILogger<SendNotificationRepository>? logger,
            IGeneralRepository generalRepository,
            IConfiguration configuration,
            IUserRepository iUserRepository,
            IFilesUploaderRepository iFilesUploaderRepository,
            ILogger<MeetingRepository> iloggerM)
        {
            _mailSetting = mailSettings;
            _smsSetting = smsSettings;
            _DbContext = DBContext;
            _iNotificationLogRepository = iNotificationLogRepository;
            _logger = logger;
            _generalRepository = generalRepository;
            _configuration = configuration;
            _iUserRepository = iUserRepository;
            _exception = new ValidatorException();
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _loggerM = iloggerM;
        }

        public SendNotificationRepository(OraDbContext dBContext,
            IOptions<ChannelMailFirstSetting> mailSettings,
            IOptions<ChannelSMSSetting> smsSettings,
            IGeneralRepository generalRepository,
            INotificationLogRepository iNotificationLogRepository)
        {
            _DbContext = dBContext;
            _mailSetting = mailSettings;
            _smsSetting = smsSettings;
            _generalRepository = generalRepository;
            _iNotificationLogRepository = iNotificationLogRepository;
            _logger = null;
            _loggerM = null;
            _iUserRepository = null;
            _configuration = null;
            _IFilesUploaderRepository = null;
        }

        public async Task DoSend(List<NotificationLogPostDto> notifications, bool sendImmediately, bool addOrUpdateNotificationslog, string? key)
        {
            int mailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int smsChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int internalChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int fcmChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_FCM_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int webFcmChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.WEB_NOTIFICATION_FCM_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();


            List<NotificationLogPostDto> notifyByEmail = new();
            List<NotificationLogPostDto> notifyBySMS = new();
            List<NotificationLogPostDto> notifyByInternal = new();
            List<NotificationLogPostDto> notifyByFCM = new();
            List<NotificationLogPostDto> notifyByWebFCM = new();

            notifications.RemoveAll(x => string.IsNullOrWhiteSpace(x.ToAddress) || x.ToAddress.Contains(Constants.INVALID_EMAIL_SUFFIX));

            notifyByEmail.AddRange(notifications.Where(x => x.NotificationChannelId == mailChannel));
            notifyBySMS.AddRange(notifications.Where(x => x.NotificationChannelId == smsChannel));
            notifyByInternal.AddRange(notifications.Where(x => x.NotificationChannelId == internalChannel));
            notifyByFCM.AddRange(notifications.Where(x => x.NotificationChannelId == fcmChannel));
            notifyByWebFCM.AddRange(notifications.Where(x => x.NotificationChannelId == webFcmChannel));


            ControlNotification Con = new();
            if (notifyByEmail.Count > 0)
            {
                INotificationObserver objEmail = new MailNotification(notifyByEmail, _mailSetting, _DbContext);
                Con.AddService(objEmail);

                
            }

            //if (notifyBySMS.Count > 0)
            //{
            //    INotificationObserver objSMS = new SMSNotification(notifyBySMS, _generalRepository, _configuration/*, _smsSetting*/);
            //    Con.AddService(objSMS);
            //}

            if (notifyByInternal.Count > 0)
            {
                INotificationObserver objInternal = new InternalNotification(notifyByInternal, _DbContext);
                Con.AddService(objInternal);
            }

            if (notifyByFCM.Count > 0)
            {
                INotificationObserver objFCM = new FCMNotification(notifyByFCM, _generalRepository);
                Con.AddService(objFCM);
            }

            if (notifyByWebFCM.Count > 0)
            {

                WriteLog("Log0.txt", "inside Do Send");

                INotificationObserver objFCM = new FCMNotification(notifyByWebFCM, _generalRepository);
                Con.AddService(objFCM);
            }


            var res = Con.ExecuteNotifier(sendImmediately, key ?? "");

            try
            {
                if (addOrUpdateNotificationslog)
                {
                    await _iNotificationLogRepository.AddNotificationsLog(res);
                }
                else
                {
                    await _iNotificationLogRepository.UpdateNotificationsLog(res);
                }
            }

            catch (Exception ex)
            {
                /*var adminUser = await _DbContext.Users.Where(x => x.UserName.ToLower().Contains("notaryadmin")).FirstOrDefaultAsync();
                if (adminUser != null)
                {
                    await SendMailToAdmin(adminUser.Id, "dangerous error in notification log job resend " + ex.Message);
                }*/
                _logger?.LogInformation(" Error important ****************************** needs admin in  DoSend is : " + ex.ToString());
            }
        }

        public static bool WriteLog(string strFileName, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream(strFileName, FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task ReSend(string channelName, int size)
        {
            int channel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == channelName).Select(x => x.Id).FirstOrDefaultAsync();
            // TODO size from appSetting.
            var failedNotifications = await _DbContext.NotificationLogs.Where(x => x.IsSent != (byte)Constants.NOTIFICATION_STATUS.SENT && x.SentCount < Constants.MAX_NOTIFY_SEND_ATTEMPTS && x.NotificationChannelId == channel).OrderBy(x => x.CreatedDate).Take(size).ToListAsync();

            if (failedNotifications.Count == 0)
            {
                return;
            }

            List<NotificationLogPostDto> notificationsDto = new List<NotificationLogPostDto>();
            foreach (var fail in failedNotifications)
            {
                notificationsDto.Add(NotificationLogPostDto.GetDto(fail));
            }

            _logger?.LogInformation("call resend for " + notificationsDto.Count + " notifications " + " on channel" + channelName + $"{DateTime.Now:hh:mm:ss} ============");
            await DoSend(notificationsDto, true, false, null);
        }
        public async Task<bool> SendOTP(int userId, string mobile, string email, int eventId, string lang)
        {
            try
            {
                int otpPeriodInMinutes = Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING;

                if (_configuration == null || _configuration["OtpPeriodInMinutes"] == null)
                {
                    _logger?.LogInformation("Warning!!! OtpPeriodInMinutes is missing");
                }
                else
                {
                    bool success = int.TryParse(_configuration["OtpPeriodInMinutes"], out int settingPeriod);
                    if (!success || settingPeriod < 1)
                    {
                        _logger?.LogInformation("Warning OtpPeriodInMinutes is invalid number or < 1 minute");
                    }
                    else
                    {
                        otpPeriodInMinutes = settingPeriod;
                    }
                }

                var totp = new Totp(Base32Encoding.ToBytes(Constants.otpBase32Secret));
                var code = totp.ComputeTotp();
                var oldOtp = await _DbContext.OtpLogs.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (oldOtp != null)
                {
                    if (oldOtp.GeneratedDate.AddMinutes(otpPeriodInMinutes) > DateTime.Now)
                    {
                        code = oldOtp.OtpCode;
                        oldOtp.GeneratedDate = DateTime.Now;
                        _DbContext.OtpLogs.Update(oldOtp);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        oldOtp.OtpCode = code;
                        oldOtp.GeneratedDate = DateTime.Now;
                        _DbContext.OtpLogs.Update(oldOtp);
                        await _DbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    OtpLog otp = new OtpLog
                    {
                        GeneratedDate = DateTime.Now,
                        OtpCode = code,
                        UserId = userId,
                    };
                    await _DbContext.OtpLogs.AddAsync(otp);
                    await _DbContext.SaveChangesAsync();
                }

                var notificationsDto = new List<NotificationLogPostDto>();
                var defLang = "en";
                if (lang != null)
                {
                    defLang = lang;
                }

                var title = (defLang.Trim().ToLower() == "ar") ? Constants.OTP_TITLE_AR : Constants.OTP_TITLE_EN;
                var bodyD = (defLang.Trim().ToLower() == "ar") ? Constants.OTP_BODY_AR : Constants.OTP_BODY_EN;

                var user = await _DbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user != null)
                {
                    bodyD += user.FullName + "  ";
                }

                byte[] bytes = Encoding.Default.GetBytes(bodyD);
                var body = Encoding.UTF8.GetString(bytes);

                if (mobile != null)
                {
                    int smsChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    //foreach (var phone in phones)
                    //{
                    //notificationsDto.Add(new NotificationLogPostDto()
                    //{
                    //    NotificationChannelId = smsChannel,
                    //    UserId = userId,
                    //    Lang = "ar",/*defLang.Trim().ToLower()*/
                    //    NotificationTitle = Constants.OTP_TITLE_AR,
                    //    NotificationBody = (Constants.OTP_BODY_AR + code).Trim(),
                    //    ToAddress = mobile,
                    //    EventId = eventId
                    //});
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = smsChannel,
                        UserId = userId,
                        Lang = "en",
                        NotificationTitle = Constants.OTP_TITLE_EN,
                        NotificationBody = (Constants.OTP_BODY_EN + code).Trim(),
                        ToAddress = mobile,
                        EventId = eventId
                    });

                    // }
                }

                if (email != null)
                {
                    int emailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    //foreach (var email in emails)
                    //{
                    //notificationsDto.Add(new NotificationLogPostDto()
                    //{
                    //    NotificationChannelId = emailChannel,
                    //    UserId = userId,
                    //    Lang = "ar",/*defLang.Trim().ToLower()*/
                    //    NotificationTitle = Constants.OTP_TITLE_AR,
                    //    NotificationBody = Constants.OTP_BODY_AR + code,
                    //    ToAddress = email,
                    //    EventId = eventId,
                    //    Template = Constants.DEFAULT_TEMPLATE
                    //});
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = emailChannel,
                        UserId = userId,
                        Lang = "en",
                        NotificationTitle = Constants.OTP_TITLE_EN,
                        NotificationBody = Constants.OTP_BODY_EN + code,
                        ToAddress = email,
                        EventId = eventId,
                        Template = Constants.DEFAULT_TEMPLATE
                    });

                    //}
                }

                await DoSend(notificationsDto, true, true, null);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogInformation("Error in generate OTP " + ex.Message);
                return false;
            }
        }

        public async Task SendMailToAdmin(int userId, string extraText)
        {
            int mailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            string adminEmail = "yhab.shaker@infostrategic.com";
            if (_configuration?["SupportEmail"] != null)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(_configuration["SupportEmail"]!);
                    adminEmail = addr.Address;
                }
                catch
                {

                }
            }

            NotificationLogPostDto notificationLogPostDto = new()
            {
                //ApplicationId = appId,
                CreatedDate = DateTime.Now,
                IsSent = 0,
                NotificationBody = " Error from  system  " + extraText + " , send time is " + DateTime.Now.ToString(),
                NotificationTitle = "Error from  system",
                Lang = "en",
                ToAddress = adminEmail,
                UserId = userId,
                NotificationChannelId = mailChannel
            };
            List<NotificationLogPostDto> notificationLogPostDtos = new List<NotificationLogPostDto> { notificationLogPostDto };
            await DoSend(notificationLogPostDtos, false, true, null);
        }
        public async Task<List<NotificationLogPostDto>> BuildNotifications(List<NotificationLogPostDto> notifications, List<Receiver> receivers, List<string> notyBody, bool addPublicLink)
        {
            List<NotificationLogPostDto> ToSendNotification = new List<NotificationLogPostDto>();
            if (notifications == null || notifications.Count == 0)
            {
                return ToSendNotification;
            }
            var smsChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            var emailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            string link;
            var namePlaceHolder = "@" + Constants.NAME;
            var ChargePlaceHolder = "@" + Constants.CHARGE;
            //--------------Send public link to moderator-------------------
            string publicLink = string.Empty;
            var host = _configuration?["CurrentHostName"] ?? "";
            var evtMeetingId = await _DbContext.Events.Where(x => x.Id == notifications[0].EventId).Select(x => x.MeetingId).FirstOrDefaultAsync();
            if (evtMeetingId != null && addPublicLink)
            {
                publicLink = $"join?meetingId={evtMeetingId}";
                var pw = await _DbContext.Meetings.Where(x => x.MeetingId == evtMeetingId).Select(x => x.Password).FirstOrDefaultAsync();
                if (pw != null)
                {
                    publicLink = publicLink + "&password=" + pw;

                }
                publicLink = Url.Combine(host, publicLink);
            }
            // var 

            for (int i = 0; i < receivers.Count; ++i)
            {
                var pLink = receivers[i].IsModerator ? publicLink : string.Empty;
                var exrta = string.Empty;
                foreach (var notify in notifications)
                {
                    var newNoti = notify.ShallowCopy();
                    if (newNoti.NotificationChannelId == smsChannel || newNoti.NotificationChannelId == emailChannel)
                    {
                        newNoti.ToAddress = notify.NotificationChannelId == smsChannel ? receivers[i].Mobile : receivers[i].Email;
                        newNoti.NotificationBody = newNoti.NotificationBody.Replace(namePlaceHolder, receivers[i].Name);
                        if (newNoti.NotificationBody.Contains(ChargePlaceHolder))
                        {
                            newNoti.NotificationBody = " , " + Translation.getMessage("ar", "Charge") + "  " + newNoti.NotificationBody.Replace(ChargePlaceHolder, receivers[i].Charge);
                        }
                        if (notyBody.Count > i)
                        {
                            //if (pLink != string.Empty && notify.NotificationChannelId != 2)
                            //{
                            //    exrta = newNoti.Lang == "ar" ? "رابط الاجتماع العام : " + publicLink : "Meeting public link : " + publicLink;
                            //}

                            link = notyBody[i];//newNoti.NotificationChannelId == smsChannel ? notyBody[i] : $"<a href='{notyBody[i]}'>{notyBody[i]}</a>";
                            newNoti.NotificationBody = $"{newNoti.NotificationBody}, {exrta}";/* {link}*/;
                            newNoti.NotificationLink = link;
                        }
                        newNoti.UserId = receivers[i].Id;
                        if (newNoti.ToAddress != null)
                        {
                            ToSendNotification.Add(newNoti);
                        }
                    }
                }
            }
            return ToSendNotification;
        }
        public async Task<List<MeetingUserLink>> FillAndSendNotification(List<NotificationLogPostDto> notifications, List<Receiver> receivers, Dictionary<string, string> Parameters,
            string? meetingId, bool addAppLink, string lang, bool send, bool isDirectInvitation, string? cisco = null)
        {

            List<MeetingUserLink> links = new List<MeetingUserLink>();
            MeetingRepository m = new MeetingRepository(_DbContext, _exception, _loggerM, _IFilesUploaderRepository, _configuration, _iUserRepository);
            string host = _configuration?["CurrentHostName"] ?? "", meetingLink;
            List<string> meetingLinks = new List<string>();
            if (Parameters != null)
            {
                foreach (var n in notifications)
                {
                    n.NotificationBody = Constants.ReplaceParemeterByValues(Parameters, n.NotificationBody);
                }
            }

            if (meetingId != null)
            {
                if (string.IsNullOrEmpty(cisco))
                {
                    foreach (Receiver r in receivers)
                    {
                        var g = await _DbContext.Participants.Where(p => p.Id == r.ParticipantId).Select(p => p.Guid).FirstOrDefaultAsync();

                        if (isDirectInvitation)
                        {
                            var hashEmail = _generalRepository.Base64Encode(r.Email);
                            meetingLink = string.Format("{0}/{1}/{2}/{3}", host, "privatejoin", meetingId, hashEmail);

                        }

                        else
                        {
                            meetingLink = Url.Combine(host, "join", r.ParticipantId.ToString(), g.ToString());
                            if (r.UserId != null)
                            {
                                meetingLink = meetingLink + "?partyId=" + r.UserId.ToString();
                            }
                        }

                        meetingLinks.Add(meetingLink);
                        links.Add(new MeetingUserLink
                        {
                            Email = r.Email,
                            Mobile = r.Mobile,
                            Id = r.Id,
                            UserId = r.UserId,
                            UserType = r.UserType,
                            MeetingLink = meetingLink,
                            EmiratesId = r.EmiratesId,
                            UuId = r.UuId,
                            MeetingId = meetingId,
                        });
                    }
                }
                else //Cisco meeting
                {
                    foreach (Receiver r in receivers)
                    {
                        meetingLinks.Add(cisco);
                        links.Add(new MeetingUserLink
                        {
                            Email = r.Email,
                            Mobile = r.Mobile,
                            Id = r.Id,
                            UserId = r.UserId,
                            UserType = r.UserType,
                            MeetingLink = cisco,
                            EmiratesId = r.EmiratesId,
                            UuId = r.UuId,
                            MeetingId = meetingId,
                        });
                    }
                }
            }
            notifications = await BuildNotifications(notifications, receivers, meetingLinks, cisco == null);
            try
            {
                if (send) await DoSend(notifications, true, true, null);
                return links;
            }
            catch
            {
                return [];
            }

        }

        public async Task<APIResult> SendOTPCode(int userId, string otpCode, string mobile, string email, string lang)
        {
            APIResult result = new APIResult();

            try
            {
                var notificationsDto = new List<NotificationLogPostDto>();
                //var defLang = "en";
                //if (lang != null)
                //{
                //    defLang = lang;
                //}

                //var title = (defLang.Trim().ToLower() == "ar") ? Constants.OTP_TITLE_AR : Constants.OTP_TITLE_EN;
                var bodyD = Constants.OTP_BODY_EN;

                var user = await _DbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user != null)
                {
                    bodyD += user.FullName + "  ";
                }

                byte[] bytes = Encoding.Default.GetBytes(bodyD);
                var body = Encoding.UTF8.GetString(bytes);

                if (mobile != null)
                {
                    int smsChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    //foreach (var phone in phones)
                    //{
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = smsChannel,
                        UserId = userId,
                        Lang = "ar",/*defLang.Trim().ToLower()*/
                        NotificationTitle = "",
                        NotificationBody = "",
                        ToAddress = mobile
                    });
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = smsChannel,
                        UserId = userId,
                        Lang = "en",
                        NotificationTitle = Constants.OTP_TITLE_EN,
                        NotificationBody = (Constants.OTP_BODY_EN + otpCode).Trim(),
                        ToAddress = mobile
                    });

                    // }
                }

                if (email != null)
                {
                    int emailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    //foreach (var email in emails)
                    //{
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = emailChannel,
                        UserId = userId,
                        Lang = "ar",/*defLang.Trim().ToLower()*/
                        NotificationTitle = "",
                        NotificationBody = "",
                        ToAddress = email,
                        Template = Constants.DEFAULT_TEMPLATE
                    });
                    notificationsDto.Add(new NotificationLogPostDto()
                    {
                        NotificationChannelId = emailChannel,
                        UserId = userId,
                        Lang = "en",
                        NotificationTitle = Constants.OTP_TITLE_EN,
                        NotificationBody = Constants.OTP_BODY_EN + otpCode + " ",
                        ToAddress = email,
                        Template = Constants.DEFAULT_TEMPLATE
                    });

                    //}
                }

                await DoSend(notificationsDto, true, true, null);

                return result.FailMe(1, "Success", false, APIResult.RESPONSE_CODE.OK, true);
            }
            catch (Exception ex)
            {
                return result.FailMe(-1, "Error in generate OTP " + ex.Message, false, APIResult.RESPONSE_CODE.ERROR, false);
            }
        }



    }
}
