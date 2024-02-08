using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.MeetingDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Repositories.IMeetingRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.DTOs.CommonDto;
using Microsoft.AspNetCore.Identity;
using JWT.Algorithms;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json;

using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.DTOs.NotificationDto;
using Flurl;
using VideoProjectCore6.DTOs.ParticipantDto;

using VideoProjectCore6.Repositories.IRoleRepository;
using VideoProjectCore6.Repositories;
#nullable disable
namespace VideoProjectCore6.Services.Meeting
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IUserRepository _iUserRepository;
        private readonly ILogger<MeetingRepository> _iLogger;
        private readonly IFilesUploaderRepository _IFilesUploaderRepository;
        ValidatorException _exception;
        private readonly IConfiguration _IConfiguration;
        private readonly ISendNotificationRepository _ISendNotificationRepository;
        private readonly IRoleRepository _IRoleRepository;

        private readonly IGeneralRepository _IGeneralRepository;

        public MeetingRepository(OraDbContext DbContext, IUserRepository iUserRepository,
            IFilesUploaderRepository iFilesUploaderRepository, IConfiguration iConfiguration,
            ILogger<MeetingRepository> iLogger, ISendNotificationRepository iNotificationRepository, IRoleRepository roleRepository, IGeneralRepository generalRepository)
        {
            _DbContext = DbContext;
            _iUserRepository = iUserRepository;
            _exception = new ValidatorException();
            _iLogger = iLogger;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IConfiguration = iConfiguration;
            _ISendNotificationRepository = iNotificationRepository;
            _IRoleRepository = roleRepository;
            _IGeneralRepository = generalRepository;
        }

        public MeetingRepository(OraDbContext DbContext, ValidatorException exception, ILogger<MeetingRepository> iLogger,
            IFilesUploaderRepository iFilesUploaderRepository, IConfiguration iConfiguration, IUserRepository iUserRepository)
        {
            _DbContext = DbContext;
            _exception = exception;
            _iLogger = iLogger;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IConfiguration = iConfiguration;
            _iUserRepository = iUserRepository;
        }


        public async Task<APIResult> AddMeeting(EventPostDto dto, int addBy, string lang)
        {
            APIResult result = new APIResult();
            ValidateMeeting(dto, lang);
            Models.Event meetingEvent = dto.GetEntity();
            meetingEvent.CreatedBy = addBy;
            meetingEvent.CreatedDate = DateTime.Now;
            int meetingId = GetNewValueByMeetingSec();
            int sumDigits = 0;
            int temp = meetingId;
            while (temp > 0)
            {
                sumDigits += temp % 10;
                temp /= 10;
            }
            sumDigits %= 100;
            meetingEvent.MeetingId = sumDigits < 10 ? meetingId.ToString() + "0" + sumDigits.ToString() : meetingId.ToString() + sumDigits.ToString();
            try
            {
                await _DbContext.Events.AddAsync(meetingEvent);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(meetingEvent.Id, "Meeting created", true, APIResult.RESPONSE_CODE.CREATED, meetingEvent.MeetingId);
            }
            catch
            {
                return result.FailMe(-1, "Error adding meeting");
            }
        }

        public async Task<APIResult> AddMeeting(MeetingPostDto dto, int addBy, string lang)
        {
            APIResult result = new APIResult();
            ValidateMeeting(dto, lang);

            Models.Meeting meeting = dto.GetEntity();
            meeting.CreatedBy = addBy;
            meeting.CreatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(dto.MeetingId))
            {
                int meetingId = GetNewValueByMeetingSec();
                int sumDigits = 0;
                int temp = meetingId;
                while (temp > 0)
                {
                    sumDigits += temp % 10;
                    temp /= 10;
                }
                sumDigits %= 100;
                meeting.MeetingId = sumDigits < 10 ? meetingId.ToString() + "0" + sumDigits.ToString() : meetingId.ToString() + sumDigits.ToString();
            }
            else
            {
             meeting.MeetingId = dto.MeetingId;
            }

            try
            {
                await _DbContext.Meetings.AddAsync(meeting);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(meeting.Id, Translation.getMessage(lang, "sucsessAdd"), true, APIResult.RESPONSE_CODE.CREATED, meeting.MeetingId);
            }
            catch(Exception e)
            {
                string error=string.Empty;
                if (!string.IsNullOrEmpty(e.Message))
                {
                    error = e.Message;
                }
                if (e.InnerException!=null && !string.IsNullOrEmpty(e.InnerException.Message))
                {
                    error = error +" "+ e.InnerException.Message;
                }
                _iLogger.LogInformation(string.Format(" *** AddMeeting Function Error : {0}", error));
                return result.FailMe(-1, Translation.getMessage(lang, "MeetingAddError") + " " + error);  
            }
        }

        public async Task<APIResult> UpdateMeeting(int id, MeetingPostDto dto, int userId, string lang)
        {
            APIResult result = new APIResult();
            result = ValidateMeeting(dto, lang);
            if (result.Id < 0)
            {
                return result;
            }
            Models.Meeting meeting = await _DbContext.Meetings.Where(a => a.Id == id).FirstOrDefaultAsync();

            if (meeting == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "zeroResult"), true);
            }

            meeting.Topic = dto.Topic;
            meeting.StartDate = dto.StartDate;
            meeting.EndDate = dto.EndDate;
            meeting.TimeZone = dto.TimeZone;
            meeting.Description = dto.Description;
            meeting.Password = dto.Password;
            meeting.Status = dto.Status;
            meeting.RecordingReq = dto.RecordingReq;
            meeting.SingleAccess = dto.SingleAccess;
            meeting.AutoLobby = dto.AutoLobby;
            meeting.LastUpdatedBy = userId;
            meeting.LastUpdatedDate = DateTime.Now;

            try
            {
                _DbContext.Meetings.Update(meeting);
                await _DbContext.SaveChangesAsync();
                result.SuccessMe(meeting.Id, "Meeting updated successfuly", true);
            }
            catch
            {
                result.FailMe(-1, "Error editing meeting");
            }

            return result;
        }

        public async Task<List<MeetingGetDto>> GetMeetings(int userId, string lang)
        {
            // TODO: get authorized user meeting.
            var isExist = _DbContext.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (isExist == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "zeroResult") + " " + userId);
                throw _exception;
            }

            List<Models.Meeting> createdMeeting = new List<Models.Meeting>();

            createdMeeting = await _DbContext.Meetings.ToListAsync();

            List<MeetingGetDto> result = new List<MeetingGetDto>();
            foreach (var meeting in createdMeeting)
            {
                result.Add(MeetingGetDto.GetDTO(meeting));
            }
            return result;
        }

        public async Task<MeetingGetDto> GetMeetingById(int id, string lang)
        {
            Models.Meeting createdMeeting = new Models.Meeting();
            createdMeeting = await _DbContext.Meetings.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "zeroResult") + " " + id);
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<MeetingGetDto> GetMeetingByMeetingId(string meetingId, string lang)
        {
            Models.Meeting createdMeeting = new Models.Meeting();
            createdMeeting = await _DbContext.Meetings.Where(d => d.MeetingId == meetingId).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "zeroResult"));
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<List<MeetingGetDto>> GetMeetingByOrderNo(int orderNo)
        {
            List<MeetingGetDto> res = new List<MeetingGetDto>();

            var createdMeeting = await _DbContext.Meetings.Where(d => d.EventId == orderNo).ToListAsync();
            if (createdMeeting == null)
            {
                return res;
            }

            foreach (var meet in createdMeeting)
            {
                res.Add(MeetingGetDto.GetDTO(meet));
            }
            return res;
        }

        public async Task<MeetingGetDto> GetMeetingByMeetingIdAndPassword(string meetingId, string password, string lang)
        {
            Models.Meeting createdMeeting = new Models.Meeting();
            createdMeeting = await _DbContext.Meetings.Where(d => d.Password == password && d.MeetingId == meetingId).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "zeroResult"));
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<IsAttended> IsAttendedByAppNo(int orderNo)
        {
            IsAttended isAttended = new IsAttended
            {
                IsLate = false,
                IsOnline = false,
                LastLogIn = null
            };

            var meets = await _DbContext.Meetings.Include(x => x.MeetingLoggings).Where(x => x.EventId == orderNo).OrderByDescending(x => x.Id).ToListAsync();

            if (meets.Count > 0)
            {
                var meet = meets[0];
                var meetLooging = meet.MeetingLoggings.Where(x => x.IsModerator == false && x.LoginDate.AddSeconds(300) >= DateTime.Now).OrderBy(x => x.FirstLogin).ToList();
                if (meetLooging != null && meetLooging.Count() > 0)
                {
                    isAttended.IsOnline = true;
                    isAttended.LastLogIn = meetLooging[0].FirstLogin;
                    if (meet.MeetingLoggings.Any(x => x.IsModerator == false && x.FirstLogin.AddMinutes(15) < DateTime.Now))
                    {
                        isAttended.IsLate = true;
                    }
                }
            }
            return isAttended;
        }

        public async Task<bool> LogInToMeeting(string meetingNo)
        {
            try
            {
                var meet = await _DbContext.Meetings.Include(x => x.MeetingLoggings).Where(x => x.MeetingId == meetingNo).FirstOrDefaultAsync();
                if (meet != null)
                {
                    var userId = _iUserRepository.GetUserID();
                    var oldLogging = meet.MeetingLoggings.Where(x => x.UserId == userId).FirstOrDefault();
                    if (oldLogging == null)
                    {
                        MeetingLogging meetLog = new MeetingLogging
                        {
                            IsModerator = false,
                            UserId = userId,
                            LoginDate = DateTime.Now,
                            MeetingId = meet.Id,
                            FirstLogin = DateTime.Now
                        };

                        await _DbContext.MeetingLoggings.AddAsync(meetLog);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        oldLogging.LoginDate = DateTime.Now;
                        _DbContext.MeetingLoggings.Update(oldLogging);
                        await _DbContext.SaveChangesAsync();
                    }
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> InviteToMeeting(string meetingId, List<string> contacts, string lang = "en", bool local = true)
        {
            string name = local ? _iUserRepository.GetUserName() : "Some one ";
            var notifications_ = new List<NotificationLogPostDto>();
            var receivers = new List<Receiver>();

            notifications_.Add(new NotificationLogPostDto
            {
                LinkCaption = lang == "ar" ? "دخول الاجتماع" : "Join meeting",
                CreatedDate = DateTime.Now,
                Lang = lang,
                NotificationBody = name + "is inviting you to join a video call happening now",
                NotificationLink = Url.Combine(_IConfiguration["CurrentHostName"], "join", meetingId),
                NotificationChannelId = 1,
                NotificationTitle = "invitation",
                Template = "invitation.html"
            });
            foreach (var email in contacts)
            {
                receivers.Add(new Receiver(email));
            }
            var a = await _ISendNotificationRepository.FillAndSendNotification(notifications_, receivers, null, meetingId, true, "ar", true, false);
            return true;
        }

        public async Task<bool> ChangeMeetingStatusBackToPendingByAppId(int applicationId)
        {
            var originalMeetingList = await _DbContext.Meetings.Where(a => a.EventId == applicationId).ToListAsync();
            foreach (var originalMeeting in originalMeetingList)
            {
                if (originalMeeting == null || originalMeeting.Status == (int)Constants.MEETING_STATUS.FINISHED || originalMeeting.Status == (int)Constants.MEETING_STATUS.PENDING)
                {
                    continue;
                }

                originalMeeting.LastUpdatedBy = _iUserRepository.GetUserID();
                originalMeeting.LastUpdatedDate = DateTime.Now;
                originalMeeting.Status = (int)Constants.MEETING_STATUS.PENDING;
                _DbContext.Meetings.Update(originalMeeting);
                await _DbContext.SaveChangesAsync();

                // TODO later from lilac.
                var deleteLogging = await _DbContext.MeetingLoggings.Where(a => a.MeetingId == originalMeeting.Id).ToListAsync();
                _DbContext.MeetingLoggings.RemoveRange(deleteLogging);
                await _DbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<int> SetMeetingStatus(string meetingId, Constants.MEETING_STATUS newStatus, bool changeAppointment, string lang)
        {
            Models.Meeting originalMeeting = await _DbContext.Meetings.Where(a => a.MeetingId == meetingId).FirstOrDefaultAsync();

            if (originalMeeting == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "zeroResult" + " " + meetingId));
                throw _exception;
            }

            originalMeeting.Status = newStatus switch
            {
                Constants.MEETING_STATUS.FINISHED => (sbyte)Constants.MEETING_STATUS.FINISHED,
                Constants.MEETING_STATUS.PENDING => (sbyte)Constants.MEETING_STATUS.PENDING,
                Constants.MEETING_STATUS.STARTED => (sbyte)Constants.MEETING_STATUS.STARTED,
                _ => throw _exception,
            };

            if (changeAppointment)
            {
                originalMeeting.StartDate = DateTime.Now.AddMinutes(-10);
                originalMeeting.EndDate = DateTime.Now;
            }

            originalMeeting.LastUpdatedBy = _iUserRepository.GetUserID();
            originalMeeting.LastUpdatedDate = DateTime.Now;
            _DbContext.Meetings.Update(originalMeeting);
            await _DbContext.SaveChangesAsync();
            return originalMeeting.Id;
        }

        public async Task<bool> MeetingHasPassword(string meetingId)
        {
            return await _DbContext.Meetings.AnyAsync(x => x.MeetingId == meetingId && x.PasswordReq);
        }

        public async Task<bool> IfExistMeeting(string meetingId)
        {
            return await _DbContext.Meetings.AnyAsync(x => x.MeetingId == meetingId);
        }
        public async Task<APIResult> MeetingJWT(int participantId, Guid guid, int? partyId, List<Attendee> attendees, string lang)
        {
            APIResult result = new();
            var participant = await _DbContext.Participants.Where(p => p.Id == participantId && p.Guid == guid).FirstOrDefaultAsync();
            if (participant == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NotParticipant"), false, APIResult.RESPONSE_CODE.BadRequest);
            }
            var event_ = await _DbContext.Events.Where(d => d.Id == participant.EventId).FirstOrDefaultAsync();
            var eventMeeting = await _DbContext.Meetings.Where(x => x.MeetingId == event_.MeetingId).Select(x => new
            {
                Id = x.Id,
                RecordingReq = x.RecordingReq,
                SingleAccess = x.SingleAccess,
                AutoLobby = x.AutoLobby
            }).FirstOrDefaultAsync();

            if (event_ == null || string.IsNullOrEmpty(event_.MeetingId))
            {
                return result.FailMe(-1, "No Meeting Found!", false, APIResult.RESPONSE_CODE.BadRequest);
            }

            var user = await _DbContext.Users.Where(u => u.Id == participant.UserId).FirstOrDefaultAsync();

            UserStruct userInfo = new UserStruct()
            {
                id = user != null ? user.Id.ToString() : Guid.NewGuid().ToString(),
                avatar = string.Empty,
                name = user.FullName,
                email = string.IsNullOrWhiteSpace(participant.Email) ? user.Email : participant.Email,
                groupId = event_.Id.ToString(),
                partyId = partyId != null ? partyId.ToString() : string.Empty,
            };

            IdentityOptions identityOptions = new IdentityOptions();
            string groupId = Guid.NewGuid().ToString();
            DateTime expirationDate = event_.EndDate.AddDays(1);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = expirationDate - origin;
            double doublec = Math.Floor(diff.TotalSeconds);
            Context contxt = new Context()
            {
                user = userInfo,
                group = groupId
            };
            var payload = new Dictionary<string, object>
            {
                { "iss", _IConfiguration["Meeting:iss"] },
                { "aud", _IConfiguration["Meeting:aud"] },
                { "sub", _IConfiguration["Meeting:sub"] },
                { "room", event_.MeetingId},
                { "autoRec", eventMeeting.RecordingReq!=null?eventMeeting.RecordingReq:false},
                { "singleAccess", eventMeeting.SingleAccess !=null ? eventMeeting.SingleAccess : false},
                { "autoLobby", eventMeeting.AutoLobby !=null ? eventMeeting.AutoLobby : false},
                { "moderator", participant.IsModerator },
                { "context", contxt },
                { "exp", doublec }
            };
            //var attendeesList = new List<string>();
            if (attendees != null && attendees.Any())
            {
                string name = string.Empty;
                //foreach (var a in attendees)
                //{
                //    name = !string.IsNullOrEmpty(a.FullName) ? a.FullName : !string.IsNullOrEmpty(a.Email) ? a.Email : "UnKnown";
                //}
                payload.Add("attendees", attendees);
            }


            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, _IConfiguration["Meeting:secret"]);
            // Uncomment for meeting log
            await LogMeeting(participant.IsModerator, event_.MeetingId, participant.UserId, eventMeeting.Id);
            return result.SuccessMe(participantId, "Ok", true, APIResult.RESPONSE_CODE.OK, string.Format("{0}/{1}?jwt={2}", _IConfiguration["Meeting:host"], event_.MeetingId, token));
        }

        public async Task<object> MeetingJWT(string meetingId, int? userId, JoinData userData, string lang)//From web
        {
            var meeting = await _DbContext.Meetings.Where(d => d.MeetingId == meetingId).AsNoTracking().FirstOrDefaultAsync();

            if (meeting == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "wrongParameter") + meetingId);
                throw _exception;
            }

            var event_ = await _DbContext.Events.Where(x => x.MeetingId == meetingId).AsNoTracking().FirstOrDefaultAsync();

            if (event_ == null)
            {
                _exception.AttributeMessages.Add("Event is not existed");
                throw _exception;
            }

            var allRooms = await _DbContext.ConfEvents.ToListAsync();

            var eventStatus = _IGeneralRepository.CheckStatus(event_.StartDate, event_.EndDate, event_.Id, event_.MeetingId, lang, allRooms);


            DateTime now = DateTime.Now;



            

            if (event_.EGroup == null && eventStatus.Status == 2 && event_.Type == 22)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "sessionLifted"));
                throw _exception;
            }

            if (event_.RecStatus == -2)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "eventCanceled"));
                throw _exception;
            }

            //var Date1 = new DateTime(now.Year, now.Month, now.Day);
            //var Date2 = new DateTime(meeting.StartDate.Year, meeting.StartDate.Month, meeting.StartDate.Day);

            var remainedDays = event_.StartDate.Subtract(now);

            var oneDay = TimeSpan.FromHours(24);

            var compResult = TimeSpan.Compare(remainedDays, oneDay);


            if (compResult > 0)
            {
                _exception.AttributeMessages.Add("Meeting has not started. You can only join in the same day");
                throw _exception;
            }



            if (meeting.Status == (int)Constants.MEETING_STATUS.FINISHED)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "meetingFinished") + meetingId);
                _iLogger.LogInformation(string.Format("try an access to finish meeting, {0}", meetingId));
                throw _exception;
            }


            bool isModerator = false;
            // context.user.id and contex.group should be fresh-generated UUID values(Do not put actual user ids here).
            string autoUserId = Guid.NewGuid().ToString();
            string confUrlPrefix = _IConfiguration["Meeting:host"];
            if (confUrlPrefix == null || confUrlPrefix.Length == 0)
            {
                _exception.AttributeMessages.Add("Missing configuration for lilac CONF_URL_PREFIX ");
                throw _exception;
            }

            string sub = confUrlPrefix;
            if (confUrlPrefix.StartsWith("https://"))
            {
                sub = confUrlPrefix.Substring("https://".Length);
            }

            if (confUrlPrefix.StartsWith("http://"))
            {
                sub = confUrlPrefix.Substring("http://".Length);
            }

            UserStruct userInfo = new UserStruct()
            {
                id = userId != null ? userId.ToString() : autoUserId,
                //avatar = "data:image/png;base64,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",
                avatar = "",
                name = userData != null ? userData.Name : "user",
                email = "not available",
                groupId = event_.Id.ToString(),
            };

            if (userId == null)
            {
                if (userData == null)
                {
                    _exception.AttributeMessages.Add(Translation.getMessage(lang, "UserMissingData"));
                    throw _exception;
                }
                isModerator = false;
                userInfo.name = userData.Name;
                userInfo.email = userData.Email;
                if (meeting.PasswordReq)
                {
                    if (meeting.Password != userData.Password)
                    {
                        _exception.AttributeMessages.Add(Translation.getMessage(lang, "errorAuthentication"));
                        throw _exception;
                    }
                }

                //var dictionary = new Dictionary<string, string>
                //    {
                //        { "secret", _IConfiguration["GoogleReCaptchaSecretKey:SecretKey"] },
                //        { "response", userData.ReCaptchaToken }
                //    };

                //var postContent = new FormUrlEncodedContent(dictionary);



                //HttpResponseMessage recaptchaResponse = null;
                //string stringContent = "";

                //using (var http = new HttpClient())
                //{
                //    recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
                //    stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
                //}

                //if (!recaptchaResponse.IsSuccessStatusCode)
                //{
                //    _exception.AttributeMessages.Add("Unable to verify recaptcha token");

                //    return _exception;
                //}

                //if (string.IsNullOrEmpty(stringContent))
                //{
                //    _exception.AttributeMessages.Add("Invalid reCAPTCHA verification response");

                //    return _exception;

                //}

                //var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaGetDto>(stringContent);

                //if (!googleReCaptchaResponse.Success)
                //{
                //    var errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);
                //    _exception.AttributeMessages.Add(errors);

                //    return _exception;

                //}

                //if (!googleReCaptchaResponse.Action.Equals("signup", StringComparison.OrdinalIgnoreCase))
                //{
                //    _exception.AttributeMessages.Add("Invalid action");
                //    return _exception;

                //}

                //if (googleReCaptchaResponse.Score < 0.5)
                //{
                //    _exception.AttributeMessages.Add("This is a potential bot. Signup request rejected");
                //    return _exception;
                //}

            }
            else
            {
                var user = _DbContext.Users.Where(x => x.Id == userId).AsNoTracking().SingleOrDefault();

                var userRoles = await _IRoleRepository.GetRolesIdByUserId(userId.GetValueOrDefault());

                if (user == null)
                {
                    _exception.AttributeMessages.Add("This user is not authorized");
                    throw _exception;
                }

                var isParticipant = await _DbContext.Participants.Where(x => x.UserId == userId && x.EventId == event_.Id).AsNoTracking().FirstOrDefaultAsync();
                if (isParticipant == null && meeting.CreatedBy != userId && !userRoles.Contains(Int32.Parse(Constants.ADMIN)))
                {
                    _exception.AttributeMessages.Add(Translation.getMessage(lang, "NotParticipant"));
                    throw _exception;
                }
                userInfo.name = userData == null || string.IsNullOrEmpty(userData.Name) ? user.FullName : userData.Name;
                userInfo.email = userData == null || string.IsNullOrEmpty(userData.Email) ? user.Email : userData.Email;
                if (meeting.CreatedBy == userId || (isParticipant != null && isParticipant.IsModerator) ||
                    userRoles.Contains(Int32.Parse(Constants.ADMIN)))
                {
                    isModerator = true;
                }
                byte[] bytes = new byte[16];
                BitConverter.GetBytes(user.Id).CopyTo(bytes, 0);
                //EncryptGUID encryptGUID = new EncryptGUID(bytes);
                //byte[] guid = new byte[16];
                //var encruptedGuid = new Guid(encryptGUID.encryptUID(guid));
                //userInfo.id = encruptedGuid.ToString();
            }

            try
            {
                // get user image.
                string getUserImageURL = _IConfiguration["GetUserImage"];
                if (userId != null && _IFilesUploaderRepository.FileExist("User_images", userId.ToString() + ".jpg"))
                {
                    userInfo.avatar = getUserImageURL + userId.ToString() + ".jpg";
                }
                else
                {
                    if (_IFilesUploaderRepository.FileExist("User_images", "default.jpg"))
                    {
                        userInfo.avatar = getUserImageURL + "default.jpg";
                    }
                }
            }
            catch
            {

            }


            // generate JWT.
            IdentityOptions identityOptions = new IdentityOptions();
            string groupId = Guid.NewGuid().ToString();
            DateTime expirationDate = event_.EndDate.AddDays(1);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = expirationDate - origin;
            double doublec = Math.Floor(diff.TotalSeconds);
            Context contxt = new Context()
            {
                user = userInfo,
                group = groupId
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", _IConfiguration["Meeting:iss"] },
                { "aud", _IConfiguration["Meeting:aud"] },
                { "sub", _IConfiguration["Meeting:sub"] },
                { "room", meetingId },
                { "autoRec", meeting.RecordingReq!=null ? meeting.RecordingReq:false },
                { "singleAccess", meeting.SingleAccess !=null ? meeting.SingleAccess : false },
                { "autoLobby", meeting.AutoLobby !=null ? meeting.AutoLobby : false},
                { "moderator", isModerator },
                { "context", contxt },
                { "exp", doublec }
            };


            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, _IConfiguration["Meeting:secret"]);

            // Console.WriteLine(token);

            // TODO check if correct only one day.
            //var hours = (createdMeeting.EndDate - DateTime.Now).TotalHours;
            //var jwtToken = new JwtSecurityToken(token, expires: DateTime.Now.AddHours(hours));

            await LogMeeting(isModerator, meetingId, userId, meeting.Id);
            return new { meetingLink = string.Format("{0}/{1}?jwt={2}", confUrlPrefix, meetingId, token) };
        }

        private async Task<bool> LogMeeting(bool isModerator, string meetingNo, int? userId, int meetingId)
        {
            if (isModerator)
            {
                await SetMeetingStatus(meetingNo, Constants.MEETING_STATUS.STARTED, true, "en");
                // reset the timer for participants logging.
                try
                {
                    var allUserlogging = await _DbContext.MeetingLoggings.Where(x => x.MeetingId == meetingId).ToListAsync();
                    if (allUserlogging != null)
                    {
                        foreach (var userlogging in allUserlogging)
                        {
                            if (!userlogging.IsModerator)
                            {
                                UserLog myDeserializedObj = new UserLog();
                                if (userlogging.PreviousLoginList != null)
                                {
                                    myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(userlogging.PreviousLoginList);
                                }

                                LogEntry entry = new LogEntry
                                {
                                    Start = userlogging.FirstLogin,
                                    End = userlogging.LoginDate
                                };
                                myDeserializedObj.UserLogs.Add(entry);

                                userlogging.PreviousLoginList = JsonConvert.SerializeObject(myDeserializedObj, Formatting.Indented);
                                userlogging.FirstLogin = DateTime.Now;
                                userlogging.LoginDate = DateTime.Now;

                                _DbContext.MeetingLoggings.Update(userlogging);
                                await _DbContext.SaveChangesAsync();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
                    {
                        msg += " inner error is" + ex.InnerException.Message;
                    }
                    _iLogger.LogInformation(string.Format("Failed in reseting the timer for participants logging to the meeting {0}", meetingId + msg));
                }

            }

            if (userId != null)
            {
                try
                {
                    var loggingUser = await _DbContext.MeetingLoggings.Where(x => x.MeetingId == meetingId && x.UserId == userId).FirstOrDefaultAsync();
                    if (loggingUser != null)
                    {
                        if (loggingUser.LoginDate.AddMinutes(5) < DateTime.Now)
                        {
                            // interruption attending.
                            UserLog myDeserializedObj = new UserLog();
                            if (loggingUser.PreviousLoginList != null)
                            {
                                myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(loggingUser.PreviousLoginList);
                            }

                            LogEntry entry = new LogEntry
                            {
                                Start = loggingUser.FirstLogin,
                                End = loggingUser.LoginDate
                            };
                            myDeserializedObj.UserLogs.Add(entry);

                            loggingUser.PreviousLoginList = JsonConvert.SerializeObject(myDeserializedObj, Formatting.Indented);
                            loggingUser.FirstLogin = DateTime.Now;
                        }

                        loggingUser.LoginDate = DateTime.Now;
                        _DbContext.MeetingLoggings.Update(loggingUser);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        MeetingLogging meetLog = new MeetingLogging
                        {
                            IsModerator = isModerator,
                            LoginDate = DateTime.Now,
                            MeetingId = meetingId,
                            FirstLogin = DateTime.Now,
                            UserId = userId
                        };

                        await _DbContext.MeetingLoggings.AddAsync(meetLog);
                        await _DbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    var error = " error is " + ex.Message;
                    if (ex.InnerException != null && ex.InnerException.Message != null)
                    {
                        error += ex.InnerException.Message;
                    }
                    _iLogger.LogInformation(string.Format("Failed in writing login record to the meeting {0}", meetingId) + error);
                }
            }
            return true;
        }
        public async Task GetMeetingLogger()
        {
            try
            {
                var allMeet = await _DbContext.Meetings.Where(x => x.Status == (int)Constants.MEETING_STATUS.FINISHED && x.MeetingLog == null).ToListAsync();

                if (allMeet == null || allMeet.Count == 0)
                {
                    return;
                }

                HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };

                string confUrlPrefix = _IConfiguration["Meeting:host"];
                if (confUrlPrefix == null || confUrlPrefix.Length == 0)
                {
                    throw _exception;
                }

                string confUrlPrefixLog = _IConfiguration["CONF_URL_PREFIX_LOG"];
                if (confUrlPrefixLog == null || confUrlPrefixLog.Length == 0)
                {
                    throw _exception;
                }

                string confUrlAPIGetLog = _IConfiguration["CONF_URL_API_GET_LOG"];
                if (confUrlAPIGetLog == null || confUrlAPIGetLog.Length == 0)
                {
                    throw _exception;
                }

                HttpClient client = new HttpClient(clientHandler);
                client.DefaultRequestHeaders.Host = confUrlPrefixLog;

                int failed = 0;
                int success = 0;
                foreach (var meet in allMeet)
                {
                    var response = await client.GetAsync(String.Format(confUrlAPIGetLog, meet));

                    // ensure the request was a success
                    if (!response.IsSuccessStatusCode)
                    {
                        meet.MeetingLog = "failed in getting log";
                        failed++;
                    }
                    else
                    {
                        meet.MeetingLog = await response.Content.ReadAsStringAsync();
                        success++;
                    }
                    _DbContext.Meetings.Update(meet);
                    await _DbContext.SaveChangesAsync();
                }

                _iLogger.LogInformation(" GetMeetingLogger success is : " + success.ToString() + " failed records count is : " + failed.ToString());
            }

            catch (Exception ex)
            {
                _iLogger.LogInformation("error in GetMeetingLogger : " + ex.Message.ToString());
                return;
            }

            /* TODO load the string logger to JSON Object to trace users.
             var stream2 = await response.Content.ReadAsStringAsync();
             var content = JsonConvert.DeserializeObject<ConferenceLogger>(stream2);
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(@"c:\movie.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, content);
            }
            */
        }
        private APIResult ValidateMeeting(MeetingPostDto dto, string lang)
        {
            APIResult result = new APIResult();
            if (dto.EndDate < dto.StartDate)
            {
                result.Message.Add(Translation.getMessage(lang, "wrongParameter"));
                return result;
            }

            int status = dto.Status;
            if (!Enum.IsDefined(typeof(Constants.MEETING_STATUS), status))
            {
                result.Message.Add(Translation.getMessage(lang, "wrongParameter"));
                return result;
            }
            return result.SuccessMe(1);
        }
        private APIResult ValidateMeeting(EventPostDto dto, string lang)
        {
            APIResult result = new APIResult();
            if (dto.EndDate < dto.StartDate)
            {
                result.Message.Add(Translation.getMessage(lang, "ErrorEventDate"));
                return result;
            }

            //byte? status = dto.Status;
            //if (!Enum.IsDefined(typeof(Constants.MEETING_STATUS), status))
            //{
            //    result.Message.Add(Translation.getMessage(lang, "wrongParameter"));
            //    return result;
            //}
            return result.SuccessMe(1);
        }

        private int GetNewValueByMeetingSec()
        {
            int sequenceNum = 0;
            var connection = _DbContext.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT nextval('dual');";
                var  intRes = (Int64)cmd.ExecuteScalar();
                sequenceNum = Convert.ToInt32(intRes);
            }
            connection.Close();
            return sequenceNum;
        }



        public class ConferenceDetailsLogger
        {
            public string id { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string confid { get; set; }
            public string userid { get; set; }
            public string info { get; set; }
        }

        public class ConferenceLogger
        {
            public string status { get; set; }
            public List<ConferenceDetailsLogger> data { get; set; }
        }
    }
}



public class Context
{
    public Context()
    {

    }
    public Context(UserStruct _user, string _group)
    {
        user = _user;
        group = _group;
    }

    public UserStruct user { get; set; }     // user restrict to be in small letter user accord to conference meeting.
    public string group { get; set; }        // group restrict to be in small letter group accord to conference meeting.
}

public struct UserStruct
{
    public string id { get; set; }    // id restrict to be in small letter id accord to conference meeting.
    public string avatar { get; set; }   // avatar restrict to be in small letter name accord to conference meeting.
    public string name { get; set; }   // name restrict to be in small letter name accord to conference meeting.
    public string email { get; set; }   // email restrict to be in small letter name accord to conference meeting.

    public string groupId { get; set; }
    public string partyId { get; set; }

}
