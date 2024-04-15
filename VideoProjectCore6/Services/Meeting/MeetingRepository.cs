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
                NotificationLink = Url.Combine(_IConfiguration["CONFOMEET_BASE_URL"], "join", meetingId),
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
        public async Task<APIResult> MeetingJWT(int participantId, Guid guid, int? partyId, string lang)
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
                participantGuid = partyId != null ? partyId.ToString() : string.Empty,
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
                { "iss", _IConfiguration["JWT_APP_ID"] },
                { "aud", _IConfiguration["*"] },
                { "sub", _IConfiguration["XMPP_DOMAIN"] },
                { "room", event_.MeetingId},
                { "autoRec", eventMeeting.RecordingReq!=null?eventMeeting.RecordingReq:false},
                { "singleAccess", eventMeeting.SingleAccess !=null ? eventMeeting.SingleAccess : false},
                { "autoLobby", eventMeeting.AutoLobby !=null ? eventMeeting.AutoLobby : false},
                { "moderator", participant.IsModerator },
                { "context", contxt },
                { "exp", doublec }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, _IConfiguration["JWT_APP_SECRET"]);
            // Uncomment for meeting log
            await LogMeeting(participant.IsModerator, event_.MeetingId, participant.UserId, eventMeeting.Id);
            return result.SuccessMe(participantId, "Ok", true, APIResult.RESPONSE_CODE.OK, string.Format("{0}/{1}?jwt={2}", _IConfiguration["PUBLIC_URL"], event_.MeetingId, token));
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



            

            if (eventStatus.Status == 2 && event_.Type == 22)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "sessionLifted"));
                throw _exception;
            }

            if (event_.RecStatus == -2)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "eventCanceled"));
                throw _exception;
            }

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
            string confUrlPrefix = _IConfiguration["PUBLIC_URL"];
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
                avatar = "",
                name = userData != null ? userData.Name : "user",
                email = "not available",
                groupId = event_.Id.ToString(),
                participantGuid = userId?.ToString() ?? "hardcoded_user",
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
                { "iss", _IConfiguration["JWT_APP_ID"] },
                { "aud", _IConfiguration["*"] },
                { "sub", _IConfiguration["XMPP_DOMAIN"] },
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
            var token = encoder.Encode(payload, _IConfiguration["JWT_APP_SECRET"]);

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
    public string participantGuid { get; set; }

}
