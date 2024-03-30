using Flurl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.DTOs.JWTDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.RoleDto;
using VideoProjectCore6.DTOs.UserDTO;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IFileRepository;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IRoleRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.NotificationService;
using static VideoProjectCore6.Services.Constants;
#nullable disable
namespace VideoProjectCore6.Services.UserService
{
    public class UserRepository : IUserRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IRoleRepository _IRoleRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _IConfiguration;
        private readonly IOptions<ChannelMailFirstSetting> _mailSetting;
        private readonly IOptions<ChannelSMSSetting> _smsSetting;
        //private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly INotificationLogRepository _iNotificationLogRepository;
        private readonly ILogger<UserRepository> _logger;
        private readonly INotificationSettingRepository _INotificationSettingRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly IFileRepository _IFileRepository;

        ValidatorException _exception;
        private readonly jwt _jwt;
        public UserRepository(IRoleRepository iRoleRepository, IOptions<jwt> jwt, SignInManager<User> signInManager,
                             OraDbContext OraDbContext, IHttpContextAccessor httpContext,
                             IGeneralRepository iGeneralRepository,
                             UserManager<User> userManager,
                             IOptions<ChannelMailFirstSetting> mailSetting,
                             IOptions<ChannelSMSSetting> smsSetting,
                             RoleManager<Role> roleManager, IConfiguration iConfiguration,
                             INotificationLogRepository iNotificationLogRepository,
                             /*IFilesUploaderRepositiory iFilesUploaderRepository,*/ IWebHostEnvironment iWebHostEnvironment,
                             ILogger<UserRepository> logger,
                             INotificationSettingRepository iNotificationSettingRepository,
                             ISysValueRepository iSysValueRepository, IFileRepository fileRepository)
        {
            _DbContext = OraDbContext;
            _httpContext = httpContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _iGeneralRepository = iGeneralRepository;
            _signInManager = signInManager;
            _IRoleRepository = iRoleRepository;
            _IConfiguration = iConfiguration;
            _iNotificationLogRepository = iNotificationLogRepository;
            // _IFilesUploaderRepository = iFilesUploaderRepository;
            _IWebHostEnvironment = iWebHostEnvironment;
            _logger = logger;
            _mailSetting = mailSetting;
            _smsSetting = smsSetting;
            _jwt = jwt.Value;
            _exception = new ValidatorException();
            _INotificationSettingRepository = iNotificationSettingRepository;
            _ISysValueRepository = iSysValueRepository;
            _IFileRepository = fileRepository;
        }

        public int GetUserID()
        {
            int userID = 0;
            if (_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                userID = int.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            return userID;
        }
        public string GetUserEid()
        {
            string EmirateId = _httpContext.HttpContext.User.Claims.Where(x => x.Type == "EmirateId").Select(x => x.Value).FirstOrDefault();// FindFirst("EmirateId").Value ;
            return EmirateId;
        }
        public string GetUserEmail()
        {
            string Email = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            return Email;
        }
        public string GetUserName()
        {
            string userName = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            return userName;
        }
        public bool IsAdmin()
        {
            return _httpContext.HttpContext.User.IsInRole(AdminPolicy);
        }

        public bool IsInspector()
        {
            return false;//_httpContext.HttpContext.User.IsInRole(Constants.InspectorPolicy);
        }

        public bool IsEmployee()
        {
            return _httpContext.HttpContext.User.IsInRole(EmployeePolicy);
        }

        //public bool IsPNSEmployee()
        //{
        //    return _httpContext.HttpContext.User.IsInRole(Constants.PNSEmployeePolicy);
        //}

        public async Task<bool> IsEmployee(int userId)
        {
            bool isEmployee = false;
            //TODO join user with user role in oracle.
            //var user = await _userManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync(u => u.Id == userId);
            //var roleEmployee = await _roleManager.FindByNameAsync(Constants.EmployeePolicy);
            //if (user != null && roleEmployee != null)
            //{
            //    isEmployee = user.UserRole.Any(x => x.RoleId == roleEmployee.Id);
            //}
            return isEmployee;
        }

        public async Task<bool> HasRole(int userId, string policy)
        {
            bool hasRole = false;
            //TODO join user with user role in oracle.
            //var user = await _userManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync(u => u.Id == userId);
            //var roleEmployee = await _roleManager.FindByNameAsync(policy);
            //if (user != null && roleEmployee != null)
            //{
            //    hasRole = user.UserRole.Any(x => x.RoleId == roleEmployee.Id);
            //}
            return hasRole;
        }

        public async Task<List<RoleGetDto>> GetUserRoles(int userId, string lang)
        {
            var user = await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(u => u.Id == userId);
            List<RoleGetDto> res = new List<RoleGetDto>();
            if (user != null && user.UserRoles != null)
            {
                foreach (var role in user.UserRoles)
                {
                    RoleGetDto roleGetDto = new RoleGetDto()
                    {
                        Id = role.RoleId,
                        RoleName = await _iGeneralRepository.GetTranslateByShortCut(lang, role.Role.Name)
                    };
                    res.Add(roleGetDto);
                }
            }
            return res;
        }

        public async Task<List<UserDto>> GetUsersRoles(string blindSearch, string lang)
        {
            List<UserDto> res = new List<UserDto>();

            if (blindSearch == null || blindSearch.Length < 6)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "blindSearchText"));
                throw _exception;
            }
            var value = blindSearch.Trim().ToLower();
            var users = await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Where(x =>
                                                         x.Email.Trim().ToLower().Contains(value) ||
                                                         x.UserName.Trim().ToLower().Contains(value) ||
                                                         x.PhoneNumber.Trim().ToLower().Contains(value) ||
                                                         x.FullName.Trim().ToLower().Contains(value) ||
                                                         x.Address.Trim().ToLower().Contains(value) ||
                                                         x.EmiratesId.Trim().ToLower().Contains(value)
                                                         ).ToListAsync();
            foreach (var user in users)
            {
                List<RoleGetDto> userRoles = new List<RoleGetDto>();
                foreach (var userRole in user.UserRoles)
                {
                    RoleGetDto roleDto = new RoleGetDto()
                    {
                        Id = userRole.RoleId,
                        RoleName = await _iGeneralRepository.GetTranslateByShortCut(lang, userRole.Role.Name)
                    };
                    userRoles.Add(roleDto);
                }

                UserDto userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    EmiratesId = user.EmiratesId,
                    Address = user.Address,
                    AreaId = user.AreaId,
                    BirthdayDate = user.BirthdayDate,
                    EmailLang = user.EmailLang,
                    Emarit = user.EmiratesId,
                    Gender = user.Gender,
                    NatId = user.NatId,
                    PhoneNumber = user.PhoneNumber,
                    TelNo = user.TelNo,
                    SmsLang = user.SmsLang,
                    RolesName = userRoles,
                    ProfileStatus = (int)user.ProfileStatus,
                };
                res.Add(userDto);
            }

            return res;
        }
        public async Task<UserPermissionsDTO> GetUserPermissions(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            UserPermissionsDTO permissions = new UserPermissionsDTO();
            if (user == null)
            {
                return permissions;
            }

            permissions.UserID = user.Id;
            var userRoles = await _roleManager.Roles.Include(e => e.RoleClaims).ToListAsync();

            List<RoleClaim> userclaims = new List<RoleClaim>();
            foreach (var role in userRoles)
            {
                if (_userManager.IsInRoleAsync(user, role.Name).Result)
                {
                    userclaims.AddRange(role.RoleClaims.ToList());
                }
            }

            permissions.Permissions = userclaims.Distinct().ToList();
            return permissions;
        }

        public async Task<List<int>> GetUserClaimPermissions(int userId, string claimType)
        {
            List<int> res = new List<int>();
            var userPermissionsDTO = await GetUserPermissions(userId);

            if (userPermissionsDTO == null)
            {
                return res;
            }
            res = userPermissionsDTO.Permissions.Where(x => x.ClaimType == claimType).Select(x => Int32.Parse(x.ClaimValue)).ToList();
            return res;
        }

        public async Task<IdentityResult> EditUserRolesAsync(int userId, List<int> userRoles, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException(Translation.getMessage(lang, "UserNotExistedBefore"));
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            List<string> roles = new List<string>();
            roles = await _roleManager.Roles.Where(x => userRoles.Contains(x.Id)).Select(x => x.Name).ToListAsync();

            IdentityResult res = IdentityResult.Success;
            if (roles != null)
            {
                res = await _userManager.AddToRolesAsync(user, roles);
            }

            scope.Complete();
            return res;
        }

        public async Task<IdentityResult> EditUserGroupsAsync(int userId, List<int> userGroups, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _DbContext.Users.Include(p => p.UserGroups).Where(q => q.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new InvalidOperationException(Translation.getMessage(lang, "UserNotExistedBefore"));
            }

            var oldgGroups = user.UserGroups;
            await RemoveFromGroupsAsync(oldgGroups);

            List<int> groups = new List<int>();
            groups = await _DbContext.Groups.Where(x => userGroups.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            IdentityResult res = IdentityResult.Success;
            if (groups != null)
            {
                foreach(var group in groups)
                {
                    user.UserGroups.Add(new UserGroup { GroupId = group });
                }
            }

            await _DbContext.SaveChangesAsync();

            scope.Complete();
            return res;
        }

        private async Task RemoveFromGroupsAsync(ICollection<UserGroup> oldGroups)
        {
            foreach(var group in oldGroups)
            {
                _DbContext.UserGroups.Remove(group);
            }

            await _DbContext.SaveChangesAsync();
        }


        public async Task<APIResult> FindUserById(int id, string lang)
        {
            List<RoleGetDto> roles = await GetUserRoles(id, lang);
            var userInfo = await _DbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            APIResult result = new APIResult();



            try
            {

                if (userInfo != null)
                {

                    UserDto userDto = new UserDto
                    {
                        Address = userInfo.Address,
                        BirthdayDate = userInfo.BirthdayDate,
                        Email = userInfo.Email,
                        EmailLang = userInfo.EmailLang,
                        EmiratesId = userInfo.EmiratesId,
                        FullName = userInfo.FullName,
                        Gender = userInfo.Gender,
                        Id = userInfo.Id,
                        Image = userInfo.Image,
                        PhoneNumber = userInfo.PhoneNumber,
                        SmsLang = userInfo.SmsLang,
                        TelNo = userInfo.TelNo,
                        ProfileStatus = (int)userInfo.ProfileStatus,
                        UserName = userInfo.UserName,
                        AreaId = userInfo.AreaId,
                        NatId = userInfo.NatId,
                        RolesName = roles
                        //    Location = userInfo.LocationId,

                    };

                    if (userInfo.NatId != null)
                    {
                        // TODO 
                        //  var nat = await _DbContext.Country.Where(x => x.UgId == userInfo.NatId).FirstOrDefaultAsync();
                        //if (nat != null)
                        //{
                        //    userDto.NationalityName = (lang == "en") ? nat.CntCountryEn : nat.CntCountryAr;
                        //}
                    }


                    // get user sign.
                    if (userInfo.Sign != null && File.Exists(Path.Combine(_IWebHostEnvironment.WebRootPath, userInfo.Sign)))
                    {
                        userDto.Sign = userInfo.Sign;
                    }


                    return result.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, userDto);
                }
                else
                {
                    return result.FailMe(-1, "User not found");
                }


            }
            catch
            {
                return result.FailMe(-1, "An error occured");
            }

        }

        private string GetPhoneNumberWithCode(string phoneNumber)
        {
            string phoneNumberWithCode = phoneNumber;
            if (phoneNumberWithCode != null && phoneNumber != "" && phoneNumber.Length >= 5 && phoneNumber.IndexOf("00971", 0, 5) == -1)
            {
                if (phoneNumber.IndexOf("0", 0, 1) == 0)
                {
                    phoneNumberWithCode = phoneNumber[1..];
                }
                phoneNumberWithCode = "00971" + phoneNumberWithCode;
            } else
            {
                phoneNumberWithCode = phoneNumber;
            }

            return phoneNumberWithCode;
        }


        private uint GetNewValueByMeetingSec()
        {
            uint sequenceNum = 0;
            var connection = _DbContext.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT nextval('loginseq');";
                var intRes = (Int64)cmd.ExecuteScalar();
                sequenceNum = Convert.ToUInt32(intRes);
            }
            connection.Close();
            return sequenceNum;
        }

        public async Task<APIResult> CreateUser(UserPostDto dto, string ImageUrl, bool updateRoles, string lang, bool outerReq)
        {
            APIResult result = new();
            if (outerReq)
            {
                if (dto.UserId == null && dto.UserType == null)
                {
                    return result.FailMe(-1, Translation.getMessage(lang, "MissingTypeOrUserId"));
                }
                int id = await _DbContext.Users.Where(x => (x.UserId != null && x.UserId == dto.UserId && x.UserType == dto.UserType) /*|| (x.Email != null && x.Email == dto.Email)*/).Select(x => x.Id).FirstOrDefaultAsync();
                if (id > 0)
                {
                    //Return Success result even if user already exist
                    return result.SuccessMe(id, Translation.getMessage(lang, "UserExisted"), true, APIResult.RESPONSE_CODE.ERROR);
                }
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var checkUser=await _userManager.FindByEmailAsync(dto.Email);
                    if(checkUser != null)
                    {
                        try
                        {
                            checkUser.UserId = dto.UserId;
                            checkUser.UserType = dto.UserType;
                            var update = await _userManager.UpdateAsync(checkUser);
                        }
                        catch
                        {

                        }
                        return result.SuccessMe(checkUser.Id, "مستخدم موجود مع بيانات تعريف مختلفة");
                    }

                }
                if (string.IsNullOrEmpty(dto.Email))
                {
                    string Email = INVALID_EMAIL_PREFIX + /*_iGeneralRepository.*/GetNewValueBySec() + INVALID_EMAIL_SUFFIX;
                    dto.Email = Email;
                    int index = Email.IndexOf("@");
                    dto.UserName = Email.Substring(0, index);
                }
                else
                {
                    try
                    {
                        var addr = new MailAddress(dto.Email);
                    }
                    catch
                    {
                        result.FailMe(-1, Translation.getMessage(lang, "InvalidEmailFormat"));
                        return result;
                    }
                }

            }

            if (dto.EmiratesId != null && await _DbContext.Users.AnyAsync(x => x.EmiratesId.Trim() == dto.EmiratesId.Trim()))
            {
                result.FailMe(-1, "EmiratesID Existed Before");
                return result;
            }

            if (dto.PhoneNumber != null && dto.PhoneNumber.Length > 0)
            {
                dto.PhoneNumber = GetPhoneNumberWithCode(dto.PhoneNumber);
                if (dto.PhoneNumber.Length > 25)
                {
                    result.FailMe(-1, Translation.getMessage(lang, "InvalidPhoneNumber"));
                    return result;
                }
            }


            /*  if (await _DbContext.Users.AnyAsync(x => x.Email.Trim() == dto.Email.Trim()))
              {
                  if (outerReq)
                  {
                      User user = await _DbContext.Users.Where(x => x.Email.Trim() == dto.Email.Trim()).FirstOrDefaultAsync();
                      await GenerateInvalidEmailAsync(user);
                  }
                  else
                  {
                      throw new System.InvalidOperationException(Translation.getMessage(lang, "EmailExistedBefore"));
                  }
              }*/

            /*  if (await _DbContext.Users.AnyAsync(x => x.UserName.Trim() == dto.UserName.Trim()))
              {
                  if (outerReq)
                  {
                      User user = await _DbContext.Users.Where(x => x.Email.Trim() == dto.Email.Trim()).FirstOrDefaultAsync();
                      await GenerateInvalidEmailAsync(user);
                  }
                  else
                  {
                      throw new System.InvalidOperationException(Translation.getMessage(lang, "UserNameExistedBefore"));
                  }
              }*/


            /* try
             {
                 var addr = new System.Net.Mail.MailAddress(dto.Email);
             }
             catch
             {
                 throw new System.InvalidOperationException(Translation.getMessage(lang, "InvalidEmailFormat"));
             }*/

            User newUser = new User()
            {
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.PasswordHash,
                EmailConfirmed = true, // ---------- Temporary
                NormalizedEmail = dto.Email.ToUpper(),
                Email = dto.Email,
                NormalizedUserName = dto.UserName.ToLower(),
                UserName = !string.IsNullOrWhiteSpace(dto.UserName) ? dto.UserName : dto.Email,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                FullName = dto.FullName,
                BirthdayDate = dto.BirthdayDate,
                Gender = dto.Gender,
                TelNo = dto.TelNo,
                Address = dto.Address,
                EmiratesId = dto.EmiratesId,
                Image = ImageUrl,//UserPostDto.Image,
                CreatedDate = DateTime.Now,
                ProfileStatus = Convert.ToInt32(PROFILE_STATUS.SUSPENDED),
                UserId = dto.UserId != null ? dto.UserId : GetNewValueByMeetingSec(),
                UserType = dto.UserType,
                EntityId = dto.EntityId,
                RecStatus = dto.RecStatus
            };


            IdentityResult createUserRes = new IdentityResult();
            try
            {
                createUserRes = await _userManager.CreateAsync(newUser, dto.PasswordHash);
            }

            catch (Exception ex)
            {
                string obj = " fullName is " + dto.FullName + " emaritId is " + dto.EmiratesId;
                // _logger.LogInformation("Error in create new user the error message is" + ex.Message + " for the user " + obj);
                result.FailMe(-1, "UserCreateError" + ex.Message);
                return result;
                //throw new System.InvalidOperationException(Translation.getMessage(lang, "UserCreateError") + ex.Message);
            }

            if (!createUserRes.Succeeded)
            {
                var errors = createUserRes.Errors.Select(x => x.Description).ToList();
                string errorResult = "";
                foreach (var x in errors)
                {
                    errorResult = errorResult + " , " + x;
                }
                result.FailMe(-1, "UserCreateError" + errorResult);
                return result;
                //throw new System.InvalidOperationException(Translation.getMessage(lang, "UserCreateError") + errorResult);
            }

            if (newUser.Id == 0)
            {
                result.FailMe(-1, "UserCreateError" + "identity is deleted.");
                return result;
                //throw new System.InvalidOperationException(Translation.getMessage(lang, "UserCreateError") + "identity is deleted.");
            }
            var assignRole = await _userManager.AddToRoleAsync(newUser, DefaultUserPolicy);
            if (!assignRole.Succeeded)
            {
                var errors = assignRole.Errors.Select(x => x.Description).ToList();
                foreach (var x in errors)
                {
                    throw new System.InvalidOperationException(Translation.getMessage(lang, "UserRolesError"));
                }
                result.Code = APIResult.RESPONSE_CODE.UnavailableForLegalReasons;
                return result;
            }


            //if (updateRoles)
            //{
            //    var res = await EditUserRolesAsync(newUser.Id, dto.UserRoles, lang);
            //    if (!res.Succeeded)
            //    {
            //        throw new System.InvalidOperationException(Translation.getMessage(lang, "UserRolesError"));
            //    }
            //}
            //---******---By yhab--------Create JWT for new outer user-------------------
            //if (outerReq)
            //{
            //    var claims = new List<Claim>{new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString())};
            //    outerUserToken = GenerateToken(claims.ToArray(), _jwt.Key, true);
            //}

            //---******--------------------------------------------------------------------
            return result.SuccessMe(newUser.Id, Translation.getMessage(lang, "UserCreated"), true, APIResult.RESPONSE_CODE.CREATED);

        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        // register and assign a role to a user 
        public async Task<APIResult> RegisterAsync(RegisterDTO registerDTO, string lang, bool sendNotification, bool AddByAdmin = false)
        {
            APIResult res = new();
            bool successEnable = bool.TryParse(_IConfiguration["EnableRegistration"], out bool enableRegistration);


            bool disableActivate = bool.TryParse(_IConfiguration["DisableActivate"], out bool disableActiveAccount);


            if (successEnable && !enableRegistration)
            {
                return res.FailMe(-1, Translation.getMessage(lang, "NoRegisteration"));
            }

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);


            if (!AddByAdmin)
            {

                if (registerDTO.ReCaptchaToken == null)
                {
                    return res.FailMe(-1, "RecaptchaToken is required");
                }
                var dictionary = new Dictionary<string, string>
                    {
                        { "secret", _IConfiguration["GoogleReCaptchaSecretKey:SecretKey"] },
                        { "response", registerDTO.ReCaptchaToken }
                    };

                var postContent = new FormUrlEncodedContent(dictionary);



                HttpResponseMessage recaptchaResponse = null;
                string stringContent = "";

                using (var http = new HttpClient())
                {
                    recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
                    stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
                }

                if (!recaptchaResponse.IsSuccessStatusCode)
                {
                    return res.FailMe(-1, "Unable to verify recaptcha token");
                }

                if (string.IsNullOrEmpty(stringContent))
                {
                    return res.FailMe(-1, "Invalid reCAPTCHA verification response");
                }

                var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaGetDto>(stringContent);

                if (!googleReCaptchaResponse.Success)
                {
                    var errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);
                    return res.FailMe(-1, errors);
                }

                if (!googleReCaptchaResponse.Action.Equals("signup", StringComparison.OrdinalIgnoreCase))
                {
                    return res.FailMe(-1, "Invalid action");
                }

                if (googleReCaptchaResponse.Score < 0.5)
                {
                    return res.FailMe(-1, "This is a potential bot. Signup request rejected");
                }

            }



            int userMeetingId = _iGeneralRepository.GetNewValueBySec();
            int sumDigits = 0;
            int temp = userMeetingId;
            while (temp > 0)
            {
                sumDigits += temp % 10;
                temp /= 10;
            }
            sumDigits %= 100;

            if (!AddByAdmin && string.IsNullOrEmpty(registerDTO.Password))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " password");
                return res;
            }

            if (!AddByAdmin && registerDTO.Password != registerDTO.ConfirmPassword)
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "NotIdenticalPassword"));
                return res;
            }

            if (string.IsNullOrEmpty(registerDTO.Email))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " email");
                return res;
            }

            /*if (string.IsNullOrEmpty(registerDTO.PhoneNumber))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " Phone Number");
                return res;
            }*/

            if (string.IsNullOrEmpty(registerDTO.FullName))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " Full name");
                return res;
            }


            if (_IConfiguration["CurrentHostName"] == null || _IConfiguration["CurrentHostName"].Length < 5)
            {
                res.Code = APIResult.RESPONSE_CODE.NotImplemented;
                res.Message.Add(" Missed Current Host Name configuration.. ask the admin to fix.");
                return res;
            }

            if (!IsValidEmail(registerDTO.Email))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "InvalidEmailFormat"));
                return res;
            }

            var oldUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (oldUser != null)
            {
                if (oldUser.EmailConfirmed)
                {
                    res.Code = APIResult.RESPONSE_CODE.BadRequest;
                    res.Id = -1; // agree with front.
                    res.Message.Add(Translation.getMessage(lang, "EmailExistedBefore"));
                    return res;
                }
                else
                {
                    if (oldUser.ProfileStatus != null && oldUser.ProfileStatus == Convert.ToInt32(PROFILE_STATUS.SUSPENDED))// User Is created without his knowing 
                    {
                        if (!await SendActivation(oldUser))
                        {
                            res.Code = APIResult.RESPONSE_CODE.NoResponse;
                            res.Message.Add(Translation.getMessage(lang, "InActiveEmail"));
                        }
                        else
                        {
                            res.Code = APIResult.RESPONSE_CODE.OK;
                            res.Message.Add(Translation.getMessage(lang, "ActivateAccount"));
                        }
                        return res;
                    }

                    res.Id = -1; // agree with front.
                    res.Code = APIResult.RESPONSE_CODE.Locked;
                    res.Message.Add(Translation.getMessage(lang, "accountRegistered"));
                    return res;
                }
            }

            // TODO  validate for  phone
            var user = registerDTO.GetEntity();
            user.UserName = registerDTO.Email.ToUpper();
            user.CreatedDate = DateTime.UtcNow;
            user.EmailConfirmed = false;
            user.ProfileStatus = Convert.ToInt32(PROFILE_STATUS.ENABLED);
            user.meetingId = sumDigits < 10 ? userMeetingId.ToString() + "0" + sumDigits.ToString() + "0" + sumDigits.ToString() + "0" + sumDigits.ToString() : userMeetingId.ToString() + "0" + sumDigits.ToString() + sumDigits.ToString();
            if (AddByAdmin && sendNotification)
                user.EmailConfirmed = true;

            IdentityResult createUser = null;
            if (AddByAdmin) {
                createUser = await _userManager.CreateAsync(user);
            } else if (!string.IsNullOrEmpty(registerDTO.Password)) {
                createUser = await _userManager.CreateAsync(user, registerDTO.Password);
            } else {
                _logger.LogError("Failed to register new user because password is required but registerDTO.Password is empty");
                res.Id = -1;
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "PasswordIsRequired"));
                return res;
            }
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(x => x.Description).ToList();
                foreach (var x in errors)
                {
                    res.Message.Add(x);
                }
                res.Code = APIResult.RESPONSE_CODE.NotAcceptable;
                return res;
            }

            IdentityResult assignRole = IdentityResult.Failed();

            if (AddByAdmin)
            {
                if (registerDTO.Roles == null)
                {
                    return res.FailMe(-1, "Roles is required");
                }

                List<string> roles = new List<string>();
                roles = await _roleManager.Roles.Where(x => registerDTO.Roles.Contains(x.Id)).Select(x => x.Name).ToListAsync();
                assignRole = await _userManager.AddToRolesAsync(user, roles);
            } else
            {
                assignRole = await _userManager.AddToRoleAsync(user, DefaultUserPolicy);
            }

            if (!assignRole.Succeeded)
            {
                var errors = assignRole.Errors.Select(x => x.Description).ToList();
                foreach (var x in errors)
                {
                    res.Message.Add(x);
                }
                res.Code = APIResult.RESPONSE_CODE.UnavailableForLegalReasons;
                return res;
            }
            if (sendNotification && !AddByAdmin)
            {
                if (disableActivate && disableActiveAccount)
                {
                    res.Code = APIResult.RESPONSE_CODE.NoResponse;
                    res.Message.Add("Your account requires activation from the admin now");
                    return res;
                }
                else
                {
                    if (!await SendActivation(user))
                    {
                        res.Code = APIResult.RESPONSE_CODE.NoResponse;
                        res.Message.Add(Translation.getMessage(lang, "InActiveEmail"));
                        return res;
                    }
                }
            }
            else if (sendNotification && AddByAdmin)
            {
                MultiLangMessage multiLangMessage = new MultiLangMessage
                {
                    En = "You have been added to the system, please reset your password",
                    Ar = "تم اضافتك الى النظام, رجاء اعادة تعيين كلمة المرور"
                };
                var sendNotify = await SendResetPasswordEmail(multiLangMessage, user.Email, "en");

                if (!((bool)sendNotify.Result))
                {
                    res.Code = APIResult.RESPONSE_CODE.NoResponse;
                    res.Message.Add(Translation.getMessage(lang, "InActiveEmail"));
                    return res;
                }
            }

            scope.Complete();
            res.Id = user.Id;
            res.Code = APIResult.RESPONSE_CODE.OK;
            res.Message.Add(sendNotification ? Translation.getMessage(lang, "ActivateAccount") : Translation.getMessage(lang, "RegOk"));
            res.Result = true;
            return res;
        }

        public async Task<APIResult> ResendActivation(string email, string lang)
        {
            APIResult res = new();
            if (string.IsNullOrEmpty(email))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " email");
                return res;
            }

            var oldUser = await _userManager.FindByEmailAsync(email);
            if (oldUser == null)
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "UserEmailError"));
                return res;
            }
            else
            {
                if (oldUser.EmailConfirmed)
                {
                    res.Code = APIResult.RESPONSE_CODE.BadRequest;
                    res.Message.Add(Translation.getMessage(lang, "IsActiveEmail"));
                    return res;
                }
                else
                {
                    await SendActivation(oldUser);
                }
            }

            res.Id = 1;
            res.Code = APIResult.RESPONSE_CODE.OK;
            res.Message.Add("check your mail please");
            res.Result = true;
            return res;

        }

        public async Task<bool> SendActivation(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = Url.Combine(_IConfiguration["CurrentHostName"], "ActiveAccount") + "?token=" + token + "&email=" + user.Email;
            int activationActionId = await _DbContext.Actions.Where(x => x.Shortcut == "ACTIVATION").Select(x => x.Id).FirstOrDefaultAsync();

            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(activationActionId);

            foreach (var n in toSendNoti)
            {
                n.LinkCaption = Translation.getMessage(n.Lang.ToLower(), "AccountActivation");
                n.Template = ACTIVATION_TEMPLATE;
                n.UserId = user.Id;
                n.CreatedDate = DateTime.Now;
                n.ToAddress = user.Email;
                n.NotificationLink = url;
            }
            SendNotificationRepository sendNotificationRepository = new(_DbContext, _mailSetting, _smsSetting, _iGeneralRepository, _iNotificationLogRepository);
            await sendNotificationRepository.DoSend(toSendNoti, true, true, null);
            return true;
        }


        public async Task<bool> SendInvitation(User user, string password)
        {
            string url = Url.Combine(_IConfiguration["CurrentHostName"], "ForgetPassword");
            int activationActionId = await _DbContext.Actions.Where(x => x.Shortcut == "SEND_REGISTER_INVITATION").Select(x => x.Id).FirstOrDefaultAsync();

            var toSendNoti = await _INotificationSettingRepository.GetNotificationsForAction(activationActionId);

            foreach (var n in toSendNoti)
            {
                n.LinkCaption = Translation.getMessage(n.Lang.ToLower(), "AccountActivation");
                n.Template = ACTIVATION_TEMPLATE;
                n.UserId = user.Id;
                n.CreatedDate = DateTime.Now;
                n.ToAddress = user.Email;
                n.NotificationLink = url;
            }
            SendNotificationRepository sendNotificationRepository = new(_DbContext, _mailSetting, _smsSetting, _iGeneralRepository, _iNotificationLogRepository);
            await sendNotificationRepository.DoSend(toSendNoti, true, true, null);
            return true;
        }


        public async Task<APIResult> ConfirmEmail(string token, string email, string lang)
        {
            var user = await _userManager.FindByEmailAsync(email);

            APIResult res = new();
            if (user == null)
            {
                res.Message.Add(Translation.getMessage(lang, "UserNotExistedBefore"));
                return res;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                foreach (var x in errors)
                {
                    res.Message.Add(x);
                }
                return res;
            }

            res.Id = 1;
            res.Code = APIResult.RESPONSE_CODE.OK;
            res.Message.Add(Translation.getMessage(lang, "EmailVFD"));
            res.Result = true;

            var contacts = await _DbContext.Contacts.Where(x => x.Email.Equals(email)).ToListAsync();

            if (contacts.Count() > 0)
            {
                foreach (var contact in contacts)
                {
                    contact.ContactId = user.Id;
                }
            }


            _DbContext.Contacts.UpdateRange(contacts);
            await _DbContext.SaveChangesAsync();




            return res;
        }

        private async Task GenerateInvalidEmailAsync(User user)
        {
            string Email = INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + INVALID_EMAIL_SUFFIX;

            user.Email = Email;
            user.NormalizedEmail = Email.ToUpper();
            int index = Email.IndexOf("@");
            user.UserName = Email.Substring(0, index);
            await _userManager.UpdateAsync(user);
        }

        public async Task<APIResult> ResetPasswordByToken(ResetPasswordDTO resetPasswordObject, string lang)
        {
            APIResult res = new();
            if (string.IsNullOrEmpty(resetPasswordObject.Email))
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "missedParameter") + " email");
                return res;
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordObject.Email);
            if (user == null)
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "UserEmailError"));
                return res;
            }

            var reset = await _userManager.ResetPasswordAsync(user, resetPasswordObject.Token, resetPasswordObject.Password);
            if (reset.Succeeded)
            {
                res.Id = 1;
                res.Code = APIResult.RESPONSE_CODE.OK;
                res.Message.Add("Reset Password success");
                res.Result = true;
                return res;
            }

            foreach (var errorMsg in reset.Errors)
            {
                res.Message.Add(errorMsg.Description);
            }

            res.Code = APIResult.RESPONSE_CODE.BadRequest;

            return res;
        }

        public async Task<APIResult> EditProfile(int id, UserPostDto userPostDto, bool fromUg, bool updateRoles, string lang)
        {
            APIResult res = new APIResult();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return res.FailMe(-1, "User not found");
            }

            if (await _DbContext.Users.AnyAsync(x => x.Email.Trim() == userPostDto.Email.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _DbContext.Users.Where(x => x.Email.Trim() == userPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    throw new System.InvalidOperationException("EmailExistedBefore");
                }
            }

            if (await _DbContext.Users.AnyAsync(x => !string.IsNullOrEmpty(userPostDto.UserName) && x.UserName.Trim() == userPostDto.UserName.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _DbContext.Users.Where(x => x.Email.Trim() == userPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    throw new System.InvalidOperationException("UserNameExistedBefore");
                }
            }

            user.PhoneNumber = userPostDto.PhoneNumber;//GetPhoneNumberWithCode(userPostDto.PhoneNumber);
            user.NormalizedEmail = userPostDto.Email.ToUpper();
            user.Email = userPostDto.Email;
            user.NormalizedUserName = userPostDto.UserName.ToLower();
            user.UserName = userPostDto.UserName;
            user.FullName = userPostDto.FullName;
            user.BirthdayDate = userPostDto.BirthdayDate;
            user.Gender = userPostDto.Gender;
            user.TelNo = userPostDto.TelNo;
            user.Address = userPostDto.Address;
            user.EmiratesId = userPostDto.EmiratesId;
            user.SecurityQuestionId = userPostDto.SecurityQuestionId;
            user.NatId = userPostDto.NatId;
            user.SecurityQuestionAnswer = userPostDto.SecurityQuestionAnswer;
            user.Status = userPostDto.Status;
            user.EmailLang = userPostDto.EmailLang;
            user.SmsLang = userPostDto.SmsLang;
            user.AreaId = userPostDto.AreaId;
            user.NotificationType = userPostDto.NotificationType;
            user.ProfileStatus = userPostDto.ProfileStatus;

            //if (userPostDto.ImageFile != null)
            //{
            //     var fileUploaded = await _IFilesUploaderRepository.UploadFile(userPostDto.ImageFile, "UserImageFolder");

            //     if (!fileUploaded.SuccessUpload)
            //     {
            //         return new UserResultDto { Message = fileUploaded.Message, User = null };
            //     }

            //     user.Image = Path.Combine(_IConfiguration["UserImageFolder"], fileUploaded.FileName);
            //}

            var result = await _userManager.UpdateAsync(user);
            var resUpdateRole = IdentityResult.Success;
            if (updateRoles)
            {
                resUpdateRole = await EditUserRolesAsync(user.Id, userPostDto.UserRoles, lang);
            }

            if (userPostDto.PasswordHash != null)
            {
                var validators = _userManager.PasswordValidators;

                foreach (var validator in validators)
                {
                    var vres = await validator.ValidateAsync(_userManager, null, userPostDto.PasswordHash);

                    if (!vres.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _exception.AttributeMessages.Add(error.Description);
                        }
                        throw _exception;
                    }
                }

                await _userManager.RemovePasswordAsync(user);
                var updatePassword = await _userManager.AddPasswordAsync(user, userPostDto.PasswordHash);

                if (!updatePassword.Succeeded)
                {
                    foreach (var x in updatePassword.Errors)
                    {
                        _exception.AttributeMessages.Add(x.Description);
                    }
                    throw _exception;
                }
            }

            scope.Complete();

            if (result.Succeeded && resUpdateRole.Succeeded)
            {
                //return new UserResultDto { Message = Translation.getMessage(lang, "sucsessUpdate"), User = user };
                return res.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
            }
            else
            {
                return res.FailMe(-1, result.Errors.FirstOrDefault().Description + resUpdateRole.Errors.FirstOrDefault().Description);
            }
        }

        public async Task<APIResult> EditUser(int id, int editBy, UserView dto, bool fromUg, string lang)
        {
            APIResult res = new();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return res.FailMe(-1, "مستخدم غير موجود");
            }

            if(dto.UserName == null)
            {
                dto.UserName = dto.Email;
            }

            if (await _DbContext.Users.AnyAsync(x => x.Email.Trim() == dto.Email.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _DbContext.Users.Where(x => x.Email.Trim() == dto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    return res.FailMe(-1, "الايميل مستخدم مسبقا");
                }
            }

            if (await _DbContext.Users.AnyAsync(x => x.UserName.Trim() == dto.UserName.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _DbContext.Users.Where(x => x.Email.Trim() == dto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    return res.FailMe(-1, "اسم المستخدم موجود مسبقا");
                }
            }

            user.PhoneNumber = dto.PhoneNumber;//GetPhoneNumberWithCode(userPostDto.PhoneNumber);
            user.NormalizedEmail = dto.Email.ToUpper();
            user.Email = dto.Email;
            user.NormalizedUserName = dto.UserName.ToLower();
            user.UserName = dto.UserName;
            user.FullName = dto.FullName;
            user.LastUpdatedDate = DateTime.Now;
            if (dto.Enable2FA != null)
                await _userManager.SetTwoFactorEnabledAsync(user, (bool)dto.Enable2FA);

            var result = await _userManager.UpdateAsync(user);
            var resUpdateRole = IdentityResult.Success;
            resUpdateRole = await EditUserRolesAsync(user.Id, dto.Roles, lang);

            await EditUserGroupsAsync(user.Id, dto.Groups, lang);

            scope.Complete();

            if (result.Succeeded && resUpdateRole.Succeeded)
            {
                return res.SuccessMe(user.Id, Translation.getMessage(lang, "sucsessUpdate"));
            }
            else
            {
                return res.FailMe(-1, result.Errors.FirstOrDefault().Description + resUpdateRole.Errors.FirstOrDefault().Description);
            }
        }
        public async Task<string> AddEditSignature64(string signatureBase64, string lang)
        {
            string pathSign = "";
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            string target = "Signature";
            // pathSign = _IFilesUploaderRepository.FromBase64ToImage(signatureBase64, target);
            try
            {

                if (pathSign != "")
                {
                    user.Sign = pathSign;
                    await _userManager.UpdateAsync(user);
                    return pathSign;
                }
            }
            catch
            {
                return pathSign;
            }
            return pathSign;
        }

        public async Task<bool> AddEditSignature(SignaturePostDto signaturePostDto, string lang)
        {
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            string imageName;
            if (signaturePostDto.SignatureFile != null)
            {
                imageName = new String(Path.GetFileNameWithoutExtension(signaturePostDto.SignatureFile.FileName).Take(10).ToArray()).Replace(" ", "_");
                imageName = imageName + DateTime.Now.ToString("yyyymmdd") + Path.GetExtension(signaturePostDto.SignatureFile.FileName);

                var imagePath = Path.Combine(_IWebHostEnvironment.ContentRootPath, "wwwroot/Signature", imageName);
                using var fileStream = new FileStream(imagePath, FileMode.Create);

                await signaturePostDto.SignatureFile.CopyToAsync(fileStream);
                user.Sign = Path.Combine("Signature", imageName);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return true;
        }

        public async Task<APIResult> SendResetPasswordEmail(MultiLangMessage message, string email, string lang)
        {
            APIResult res = new();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                res.Code = APIResult.RESPONSE_CODE.BadRequest;
                res.Message.Add(Translation.getMessage(lang, "UserEmailError"));
                return res;
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (_IConfiguration["CurrentHostName"] == null || _IConfiguration["CurrentHostName"].Length < 5)
            {
                _exception.AttributeMessages.Add(" Missed Current Host Name configuration.. ask the admin to fix.");
                throw _exception;
            }

            string url = Url.Combine(_IConfiguration["CurrentHostName"], "ResetPassword") + "?token=" + token + "&email=" + email;
            //string body = " please follow the link to change your password :\n" + "<a href =\'" + url + "' > reset password link </a> ";
            var channel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            NotificationLogPostDto notificationPost = new()
            {
                LinkCaption = "Link to reset your password was sent",
                Template = ACTIVATION_TEMPLATE,
                NotificationLink = url,
                Lang = "en",
                UserId = user.Id,
                CreatedDate = DateTime.Now,
                NotificationBody = message.En,
                ToAddress = email,
                NotificationTitle = "Link to reset your password was sent",
                MeetingId = 0.ToString(),
                NotificationChannelId = channel
            };
            NotificationLogPostDto notificationPost2 = new()
            {
                LinkCaption = "تم ارسال رابط اعادة تعيين كلمة السر",
                Template = ACTIVATION_TEMPLATE,
                NotificationLink = url,
                Lang = "ar",
                UserId = user.Id,
                CreatedDate = DateTime.Now,
                NotificationBody = message.Ar,
                ToAddress = email,
                NotificationTitle = "تم ارسال رابط اعادة تعيين كلمة السر",
                MeetingId = 0.ToString(),
                NotificationChannelId = channel
            };

            List<NotificationLogPostDto> notifications = new() { notificationPost, notificationPost2 };

            SendNotificationRepository sendNotificationRepository = new(_DbContext, _mailSetting, _smsSetting, _iGeneralRepository, _iNotificationLogRepository);
            await sendNotificationRepository.DoSend(notifications, true, true, null);
            res.Id = 1;
            res.Code = APIResult.RESPONSE_CODE.OK;
            res.Message.Add("Link to reset your password was sent");
            res.Result = true;
            return res;
        }

        public async Task<bool> EditPassword(EditUserPasswordDTO editUserPasswordDTO, string lang)
        {
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            if (editUserPasswordDTO.NewPassword != editUserPasswordDTO.ConfirmPassword)
            {
                _exception.AttributeMessages.Add(Translation.getMessage(lang, "NotIdenticalPassword"));
                throw _exception;
            }

            var updatePassword = await _userManager.ChangePasswordAsync(user, editUserPasswordDTO.CurrentPassword, editUserPasswordDTO.NewPassword);

            if (!updatePassword.Succeeded)
            {
                foreach (var x in updatePassword.Errors)
                {
                    _exception.AttributeMessages.Add(x.Description);
                }
                throw _exception;
            }
            return true;
        }

        public async Task<APIResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new APIResult { Id = 1, Code = APIResult.RESPONSE_CODE.PageNotFound, Message = new List<string> { "USER NOT FOUND" } };
            }
            var delResult = await _userManager.DeleteAsync(user);
            if (!delResult.Succeeded)
            {
                foreach (var error in delResult.Errors)
                {
                    _logger.LogError("Cannot delete user {}, error_code={}, error_text={}", id, error.Code, error.Description);
                }
                return new APIResult { Id = 1, Code = APIResult.RESPONSE_CODE.ERROR, Message = new List<string> { "USER NOT DELETE DUE TO SERVICE INTERNAL ERROR" } };
            }

            return new APIResult { Id = 1, Code = APIResult.RESPONSE_CODE.OK, Message = new List<string> { "SUCCESS" } };
        }

        public async Task<UserResultDto> EnableUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return new UserResultDto { Message = "USER NOT FOUND", User = null };
            user.ProfileStatus = Convert.ToInt32(PROFILE_STATUS.ENABLED);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return new UserResultDto { Message = "USER WAS ENABLED ", User = user };
            else return new UserResultDto { Message = result.Errors.FirstOrDefault().Description, User = null };
        }

        public async Task<LogInResultDto> LocalSignIn(LogInDto logInDto, string lang)
        {
            return null;
            // return await SignIn(new LogInDto { Email = logInDto.Email, PassWord = logInDto.PassWord, UA = logInDto.UA }, lang);
        }

        public async Task<APIResult> RefreshToken()
        {
            APIResult result = new APIResult();

            try
            {
                string refreshToken = "";
                var user = await _DbContext.Users.Where(x => x.Id == GetUserID()).FirstOrDefaultAsync();
                if (user != null)
                {
                    Claim[] claims = GenerateClaims(user);
                    refreshToken = GenerateToken(claims, _jwt.Key, true);
                }

                if (IsEmployee())
                {
                    try
                    {
                        //TODO
                        //  await RefreshEmpLogin();
                    }
                    catch (Exception ex)
                    {
                        var msg = " error is : " + ex.Message;
                        if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
                        {
                            msg += "  " + ex.InnerException.Message;
                        }
                        _logger.LogInformation("Error in Refresh employee Login  " + msg);
                    }
                }

                return result.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, refreshToken);
            }
            catch
            {
                return result.FailMe(-1, "Failed to generate");
            }
        }

        public async Task<int> VisitorsCount()
        {

            // TODO
            return 1;
            //var users = await _DbContext.UserLogin.Select(x => x.UserId).Distinct().ToListAsync();
            //return users.Count;
        }
        // private async Task<LogInResultDto> SignIn(LogInDto logInDto, string lang)
        // {
        //     // using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
        //     var user = await _userManager.FindByEmailAsync(logInDto.Email);

        //     var userRoles = user != null ? await _IRoleRepository.GetRolesIdByUserId(user.Id) : null;

        //     var onlyAdminLogin = bool.Parse(_IConfiguration["EnableOnlyAdminLogin"]);


        //     LogInResultDto result = new();

        //     if(onlyAdminLogin && (userRoles != null && !userRoles.Contains(Int32.Parse(Constants.ADMIN)) 
        //         && !userRoles.Contains(Int32.Parse(Constants.EXUSER))))
        //     {
        //         result.StatusCode.Code = APIResult.RESPONSE_CODE.BadRequest;
        //         result.StatusCode.Message.Add(Translation.getMessage(lang, "InActiveEmail"));
        //         return result;
        //     }


        //     if (user == null)
        //     {
        //         result.StatusCode.Code = APIResult.RESPONSE_CODE.BadRequest;
        //         result.StatusCode.Message.Add(Translation.getMessage(lang, "UserEmailError"));
        //         return result;
        //     }
        //     var resSignin = await _signInManager.PasswordSignInAsync(user, logInDto.PassWord, false, lockoutOnFailure: true);
        //     if (resSignin.IsNotAllowed)
        //     {
        //         if (user.ProfileStatus != null && user.ProfileStatus == Convert.ToInt32(PROFILE_STATUS.SUSPENDED))// User Is created without his knowing 
        //         {
        //             if (!await SendActivation(user))
        //             {
        //                 result.StatusCode.Code = APIResult.RESPONSE_CODE.NoResponse;
        //                 result.StatusCode.Message.Add(Translation.getMessage(lang, "InActiveEmail"));
        //             }
        //             else
        //             {
        //                 result.StatusCode.Code = APIResult.RESPONSE_CODE.OK;
        //                 result.StatusCode.Message.Add("Check your mail please");
        //             }
        //             return result;
        //         }
        //         result.StatusCode.Code = APIResult.RESPONSE_CODE.BadRequest;
        //         result.StatusCode.Message.Add(Translation.getMessage(lang, "UserAccountInactive"));
        //         return result;
        //     }
        //     if (resSignin.IsLockedOut)
        //     {
        //         result.StatusCode.Code = APIResult.RESPONSE_CODE.BadRequest;
        //         result.StatusCode.Message.Add(Translation.getMessage(lang, "UserAccountLocked"));
        //         return result;
        //     }
        //     if (!resSignin.Succeeded)
        //     {
        //         //await _userManager.SetLockoutEnabledAsync(user, true);
        //         result.StatusCode.Code = APIResult.RESPONSE_CODE.BadRequest;
        //         result.StatusCode.Message.Add(Translation.getMessage(lang, "wrongPassword"));
        //         return result;
        //     }

        //     DateTime lastLog = DateTime.Now;
        //     var info = await _DbContext.UserLogins.Where(x => x.UserId == user.Id).ToListAsync();
        //     if (info.Count > 0)
        //     {
        //         lastLog = (DateTime)info.Last().LoginDate;
        //     }

        //     await LogUserSignInEvent(user, logInDto.UA);
        //     return await PrepareUserLoggedInResponse(user);
        // }


        //private async Task<int> GetGenderIdAsync(string gender)
        //{
        //    int res = 1;
        //    var lookupValue = await _DbContext.SysLookupValue.Where(x => x.Shortcut == "MALE").FirstOrDefaultAsync();
        //    if (lookupValue != null)
        //    {
        //        res = lookupValue.Id;
        //    }

        //    if (gender == null) return res;

        //    if (gender.Contains('F') || gender.Contains('f') || gender.Contains('ث'))
        //    {
        //        var lookupValueF = await _DbContext.SysLookupValue.Where(x => x.Shortcut == "FEMALE").FirstOrDefaultAsync();
        //        if (lookupValueF != null)
        //        {
        //            return lookupValueF.Id;
        //        }
        //    }
        //    return res;
        //}
        private string GenerateToken(Claim[] claims, string key, bool longLifeTime = false)
        {
            int tokenPeriodInMinutes = 200;

            int tokenPeriodOutDays = 30;

            if (_IConfiguration["jwt:TokenInMinutes"] == null)
            {
                _logger.LogInformation("Warning!!! jwt:TokenInMinutes is missing");
            }
            else
            {
                bool success = int.TryParse(_IConfiguration["jwt:TokenInMinutes"], out int settingPeriod);
                if (!success || settingPeriod < 1)
                {
                    _logger.LogInformation("jwt:TokenInMinutes is invalid number or < 1 minute");
                }
                else
                {
                    tokenPeriodInMinutes = settingPeriod;
                }
            }
            var expDate = longLifeTime ? DateTime.Now.AddDays(tokenPeriodOutDays) : DateTime.Now.AddMinutes(tokenPeriodInMinutes);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
            var jwt = new JwtSecurityToken(signingCredentials: signingCredentials,
                claims: claims, expires: expDate);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        private Claim[] GenerateClaims(User user)
        {
            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
             //   new Claim("EmirateId", user.EmiratesId),
                new Claim("iss","MOJ_NOTARY"),
            //    new Claim(ClaimTypes.Expiration, new DateTimeOffset().ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            //if (!string.IsNullOrWhiteSpace(user.UuId))
            //{
            //    claims.Add(new Claim("uuid", user.UuId));
            //}
            foreach (var role in _roleManager.Roles.ToList())
            {
                if (_userManager.IsInRoleAsync(user, role.Name).Result)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }
            return claims.ToArray();
        }
        public async Task<bool> IsExist(string EmiratId)
        {
            return await _DbContext.Users.AnyAsync(m => m.EmiratesId == EmiratId);
        }
        public async Task<List<UserDto>> GetUsers()
        {
            List<UserDto> Users = await _DbContext.Users.Select(x => new UserDto
            {
                Address = x.Address,
                BirthdayDate = x.BirthdayDate,
                Email = x.Email,
                EmailLang = x.EmailLang,
                EmiratesId = x.EmiratesId,
                FullName = x.FullName,
                Gender = x.Gender,
                Id = x.Id,
                Image = x.Image,
                PhoneNumber = x.PhoneNumber,
                SmsLang = x.SmsLang,
                TelNo = x.TelNo,
                ProfileStatus = (int)x.ProfileStatus,
                UserName = x.UserName
            }).ToListAsync();

            return Users;
        }
        public async Task<bool> DisabledAccount(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return true;
            if (user.ProfileStatus == Convert.ToInt32(PROFILE_STATUS.DISABLED)) return true;
            else return false;
        }
        private async Task<bool> EnabledAccount(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            if (user.ProfileStatus == Convert.ToInt32(PROFILE_STATUS.ENABLED))
            {
                return true;
            }

            return false;
        }
        public async Task<APIResult> DisableAccount(int id, string lang = "ar")
        {
            APIResult result = new APIResult();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
            }
            user.RecStatus = (byte?)CABIN_STATUS.DISABLED;
            user.LastUpdatedDate = DateTime.Now;
            try
            {
                await _userManager.UpdateAsync(user);
                return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
            }
            catch { return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate")); }

        }
        public async Task<APIResult> EnableAccount(int id, string lang = "ar")
        {
            APIResult result = new APIResult();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
            }
            user.RecStatus = (byte?)CABIN_STATUS.ENABLED;
            user.EmailConfirmed = true;
            user.LastUpdatedDate = DateTime.Now;
            try
            {
                await _userManager.UpdateAsync(user);
                return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
            }
            catch { return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate")); }
        }
        public async Task<APIResult> LockAccount(int id, int? forDays, string lang = "ar")
        {
            APIResult result = new APIResult();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
            }
            user.LastUpdatedDate = DateTime.Now;
            try
            {
                var lockIt = await _userManager.SetLockoutEnabledAsync(user, true);
                if (lockIt.Succeeded)
                {
                    if (forDays.HasValue)
                    {
                        lockIt = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(forDays.Value));
                    }
                    else
                    {
                        lockIt = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    }
                    return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));
                }
                else
                {
                    return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
                }
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
            }
        }
        public async Task<APIResult> UnLockAccount(int id, string lang = "ar")
        {
            APIResult result = new APIResult();
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
            }
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                user.LastUpdatedDate = DateTime.Now;
                //var lockIt = await _userManager.SetLockoutEnabledAsync(user, false);
                //if (lockIt.Succeeded)
                //{
                var lockIt = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromDays(10));
                await _userManager.ResetAccessFailedCountAsync(user);
                scope.Complete();
                return result.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));

                //}
                //else
                //{
                //    return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
                //}
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "faildUpdate"));
            }
        }
        public async Task<APIResult> ActivateAccount(int id, string lang = "ar")
        {
            APIResult result = new();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "NoMatchingRecord"));
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            try
            {
                await _userManager.ConfirmEmailAsync(user, token);
                return result.SuccessMe(id, Translation.getMessage(lang, "AccountACT"));
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "AccountActEr"));
            }
        }
        public void SignOut()
        {
            _signInManager.SignOutAsync();

            //var uSERNAME = _httpContext.HttpContext.User.Identity;

            bool isAuthenticated = _httpContext.HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                _signInManager.SignOutAsync();
            }
        }
        public async Task<UserDto> GetOne(int Id)
        {
            var query = _DbContext.Users.Where(x => x.Id == Id)
                                         .Select(x => new UserDto
                                         {
                                             Address = x.Address,
                                             BirthdayDate = x.BirthdayDate,
                                             Email = x.Email,
                                             EmailLang = x.EmailLang,
                                             EmiratesId = x.EmiratesId,
                                             FullName = x.FullName,
                                             Id = x.Id,
                                             SmsLang = x.SmsLang,
                                             TelNo = x.TelNo,
                                             PhoneNumber = x.PhoneNumber,
                                             ProfileStatus = (int)x.ProfileStatus,
                                             UserName = x.UserName,
                                             AreaId = (int)x.AreaId,
                                             NatId = (int)x.NatId,
                                             Sign = x.Sign
                                         });

            return await query.FirstOrDefaultAsync();
        }
        public async Task<Dictionary<int, string>> GetEmployees()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            var allEmployeeUsers = await _userManager.GetUsersInRoleAsync(EmployeePolicy);
            dic = allEmployeeUsers.ToDictionary(x => x.Id, x => x.UserName);
            return dic;
        }

        //public async Task<List<OnLineEmployee>> GetOnlineEmployees(DateTime date)
        //{
        //    List<OnLineEmployee> result = new List<OnLineEmployee>();

        //    var logger = await (
        //        from employeeLog in _DbContext.EmpLogger
        //        join useInfo in _DbContext.User on employeeLog.UserId equals useInfo.Id
        //        where employeeLog.LoggingDate.Date == date.Date
        //        select new { UserID = useInfo.Id, UserName = useInfo.FullName, fLogIn = employeeLog.FirstLogin, logIn = employeeLog.LoggingDate, LogRec = employeeLog.PreviousLoginList }).ToListAsync();

        //    foreach (var rec in logger)
        //    {
        //        OnLineEmployee onLineEmployee = new OnLineEmployee
        //        {
        //            UserId = rec.UserID,
        //            UserName = rec.UserName,
        //            LastStartWork = rec.logIn,
        //            StartWork = (DateTime)rec.fLogIn,
        //            Minutes = (int)((DateTime)rec.logIn - (DateTime)rec.fLogIn).TotalMinutes,
        //            LogToday = rec.LogRec
        //        };

        //        if (rec.LogRec != null)
        //        {
        //            UserLog myDeserializedObj = new UserLog();
        //            myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(rec.LogRec);
        //            foreach (var log in myDeserializedObj.UserLogs)
        //            {
        //                var min = (int)(log.End - log.Start).TotalMinutes;
        //                if (min > 0)
        //                {
        //                    onLineEmployee.Minutes += min;
        //                }
        //            }
        //        }

        //        result.Add(onLineEmployee);
        //    }


        //    return result;
        //}

        //public async Task<bool> RefreshEmpLogin()
        //{
        //    var userId = GetUserID();
        //    var logDay = await _DbContext.EmpLogger.Where(x => x.UserId == userId && x.LoggingDate.Date == DateTime.Now.Date).FirstOrDefaultAsync();
        //    if (logDay == null)
        //    {

        //        first login today for the employee.

        //       await _DbContext.EmpLogger.AddAsync(new EmpLogger { UserId = userId, LoggingDate = DateTime.Now, FirstLogin = DateTime.Now });
        //       await _DbContext.SaveChangesAsync();
        //        return true;
        //    }
        //    else
        //    {
        //        if (logDay.FirstLogin == null || logDay.LoggingDate == null)
        //        {

        //            logDay.FirstLogin ??= DateTime.Now;
        //            logDay.LoggingDate = DateTime.Now;
        //            _DbContext.EmpLogger.Update(logDay);
        //            await _DbContext.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            if ((int)(DateTime.Now - logDay.LoggingDate).TotalMinutes > 21)
        //            {
        //                UserLog myDeserializedObj = new UserLog();
        //                if (logDay.PreviousLoginList != null)
        //                {
        //                    myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(logDay.PreviousLoginList);
        //                }

        //                LogEntry entry = new LogEntry
        //                {
        //                    Start = (DateTime)logDay.FirstLogin,
        //                    End = (DateTime)logDay.LoggingDate
        //                };
        //                myDeserializedObj.UserLogs.Add(entry);

        //                logDay.PreviousLoginList = JsonConvert.SerializeObject(myDeserializedObj, Formatting.Indented);
        //                logDay.FirstLogin = DateTime.Now;
        //                logDay.LoggingDate = DateTime.Now;

        //                _DbContext.EmpLogger.Update(logDay);
        //                await _DbContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                logDay.LoggingDate = DateTime.Now;
        //                _DbContext.EmpLogger.Update(logDay);
        //                await _DbContext.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    return true;
        //}

        public async Task<CreateUserOldResultDto> CreateUserForOldAppParties(OldUserPostDto UserPostDto)
        {

            CreateUserOldResultDto res = new CreateUserOldResultDto();
            var emailIdOldUsers = UserPostDto.Email;
            var emaritIdOldUsers = UserPostDto.EmiratesId;

            if (UserPostDto.EmiratesId != null && UserPostDto.EmiratesId != "0" && UserPostDto.EmiratesId != "" && UserPostDto.EmiratesId.Trim() != null)
            {
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim();
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"_", "");
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"-", "");
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"/", "");

                if (!UserPostDto.EmiratesId.StartsWith("784") || UserPostDto.EmiratesId.Length != 15)
                {
                    UserPostDto.EmiratesId = "Invalid_Old_" + UserPostDto.EmiratesId;
                }
            }
            else
            {

                var existUserName = await _DbContext.Users.Where(x => x.FullName.Trim() == UserPostDto.FullName.Trim()).FirstOrDefaultAsync();
                if (existUserName != null)
                {
                    res.UserId = existUserName.Id;
                    res.User = UserPostDto;
                    res.Message = "Existed before same full Name";
                    return res;
                }

                UserPostDto.EmiratesId = "Invalid_Old_" + _iGeneralRepository.GetNewValueBySec();
            }

            var existUser = await _DbContext.Users.Where(x => x.EmiratesId.Trim() == UserPostDto.EmiratesId.Trim()).FirstOrDefaultAsync();
            if (existUser != null)
            {
                res.UserId = existUser.Id;
                res.User = UserPostDto;
                res.Message = "Existed before same Emarit id";
                return res;
            }

            if (UserPostDto.Email == null || UserPostDto.Email == "0" || UserPostDto.Email == "")
            {
                string Email = INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + INVALID_EMAIL_SUFFIX;
                UserPostDto.Email = Email;
            }
            else
            {
                if (await _DbContext.Users.AnyAsync(x => x.Email.Trim() == UserPostDto.Email.Trim()))
                {
                    string Email = INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + INVALID_EMAIL_SUFFIX;
                    UserPostDto.Email = Email;
                }
            }

            int index = UserPostDto.Email.IndexOf("@");
            UserPostDto.UserName = UserPostDto.Email.Substring(0, index);


            existUser = await _DbContext.Users.Where(x => x.UserName.Trim() == UserPostDto.UserName.Trim()).FirstOrDefaultAsync();
            if (existUser != null)
            {
                res.UserId = existUser.Id;
                res.User = UserPostDto;
                res.Message = "Existed before same userName id";
                return res;
            }

            if (UserPostDto.PhoneNumber != null)
            {
                UserPostDto.PhoneNumber = GetPhoneNumberWithCode(UserPostDto.PhoneNumber);
                if (UserPostDto.PhoneNumber.Length > 25)
                {
                    throw new System.InvalidOperationException("invalid phone number");
                }
            }

            try
            {

                User newUser = new User()
                {
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    PhoneNumber = UserPostDto.PhoneNumber,
                    PasswordHash = UserPostDto.PasswordHash,
                    EmailConfirmed = false,
                    NormalizedEmail = UserPostDto.Email.ToUpper(),
                    Email = UserPostDto.Email,
                    NormalizedUserName = UserPostDto.UserName.ToLower(),
                    UserName = UserPostDto.UserName,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FullName = UserPostDto.FullName,
                    BirthdayDate = UserPostDto.BirthdayDate,
                    Gender = UserPostDto.Gender,
                    TelNo = UserPostDto.TelNo,
                    Address = UserPostDto.Address,
                    EmiratesId = UserPostDto.EmiratesId,
                    CreatedDate = DateTime.Now,
                    ProfileStatus = Convert.ToInt32(PROFILE_STATUS.ENABLED),
                };


                var result = await _userManager.CreateAsync(newUser, UserPostDto.PasswordHash);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description).ToList();
                    string errorResult = "";
                    foreach (var x in errors)
                    {
                        errorResult = errorResult + " , " + x;
                    }
                    res.UserId = existUser.Id;
                    res.User = UserPostDto;
                    res.Message = errorResult;
                    return res;
                }


                var resRole = await EditUserRolesAsync(newUser.Id, UserPostDto.UserRoles, "en");
                if (!resRole.Succeeded)
                {
                    res.UserId = newUser.Id;
                    res.User = UserPostDto;
                    res.Message = "Success in adding but failed in adding role";
                    return res;
                }

                res.UserId = newUser.Id;
                res.User = UserPostDto;
                res.Message = "Success";
                return res;

            }

            catch (Exception ex)
            {
                res.UserId = 0;
                res.User = UserPostDto;
                res.Message = "Failed," + ex.Message;
                return res;
            }
        }

        // TODO
        private int GetNewValueBySec()
        {
            int sequenceNum = 0;
            var connection = _DbContext.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "select SEQUENCE_LOGIN.NEXTVAL from dual";
                decimal obj = (decimal)cmd.ExecuteScalar();
                sequenceNum = decimal.ToInt32(obj);
            }
            connection.Close();
            return sequenceNum;
        }

        public async Task<APIResult> UpdateFCMToken(int id, FCMTokenDto dto)
        {
            APIResult res = new APIResult();
            FcmToken fcmToken = await _DbContext.FcmTokens.FirstOrDefaultAsync(x => x.Id == id);
            if (fcmToken == null)
            {
                return res.FailMe(-1, "No matching record");
            }
            try
            {
                fcmToken.DeviceInfo = dto.DeviceInfo;
                fcmToken.Token = dto.Token;
                fcmToken.LastUpdatedDate = DateTime.Now;
                _DbContext.FcmTokens.Update(fcmToken);
                await _DbContext.SaveChangesAsync();
                return res.SuccessMe(fcmToken.Id, "Token updated successfully");
            }
            catch
            {
                return res.FailMe(-1, "Fail addeding token");
            }
        }

        public async Task<APIResult> GetLocalUserId(int userId, int userType)
        {
            APIResult result = new APIResult();

            int localUserId = await _DbContext.Users.Where(x => x.UserId == userId && x.UserType == userType).Select(x => x.Id).FirstOrDefaultAsync();

            if (localUserId > 0)

            {
                return result.SuccessMe(localUserId);
            }

            else

            {
                return result.FailMe(-1, "No matching user");
            }
        }

        //public async Task<APIResult> GetLocalUserId(int userId)
        //{
        //    APIResult result = new APIResult();

        //    int localUserId = await _DbContext.Users.Where(x => x.Id == userId).Select(x => x.Id).FirstOrDefaultAsync();

        //    if (localUserId > 0)

        //    {
        //        return result.SuccessMe(localUserId);
        //    }

        //    else

        //    {
        //        return result.FailMe(-1, "No matching user");
        //    }
        //}

        public async Task<ListCount> GetUsers(string lang, int pageIndex = 1, int pageSize = 25)
        {
            var query = await _DbContext.Users.AsNoTracking().Include(u => u.UserRoles).

                        Select(u => new UserView
                        {
                            Email = u.Email,
                            FullName = u.FullName,
                            Id = u.Id,
                            PhoneNumber = u.PhoneNumber,
                            UserName = u.UserName,
                            Locked = CheckLockState(u.LockoutEnabled, u.LockoutEnd),
                            Confirmed = u.EmailConfirmed,
                            Roles = u.UserRoles.Select(r => r.RoleId).ToList(),
                            Enable2FA = u.TwoFactorEnabled,

                        }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            int total = await _DbContext.Users.CountAsync();
            //var filteredResult = await PaginatedList<UserView>.CreateAsync(query.AsQueryable(), pageIndex > 0 ? pageIndex : 1, pageSize > 0 ? pageSize : 20, total);
            return new ListCount
            {
                Count = total,
                Items = query
            };
        }
        public async Task<ListCount> Search(string email, int pageIndex = 1, int pageSize = 25, string lang = "ar")
        {
            var query = await _DbContext.Users.Where(u => u.NormalizedEmail.Contains(email.ToUpper())).AsNoTracking().

                        Select(u => new UserView
                        {
                            Email = u.Email,
                            FullName = u.FullName,
                            Id = u.Id,
                            PhoneNumber = u.PhoneNumber,
                            UserName = u.UserName,
                            Locked = CheckLockState(u.LockoutEnabled, u.LockoutEnd),
                            Confirmed = u.EmailConfirmed,
                            Enable2FA = u.TwoFactorEnabled,

                        }).ToListAsync();

            int total = query.Count();

            return new ListCount
            {
                Count = total,
                Items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };
        }

        // 
        public async Task<ListCount> SearchFilterUsers(UserFilterDto userFilterDto, int currentUser, string lang = "ar")
        {


            
            
            bool twoArePassed = userFilterDto != null ? userFilterDto.name != null && userFilterDto.email != null && userFilterDto.userType.Count() > 0 : false;


            List<ValueId> defaultGroup = new List<ValueId>();
            defaultGroup.Add(new ValueId { Id = 0, Value = "Default Group" });

            APIResult result = new APIResult();

            //try
            //{

                var query = await _DbContext.Users.Where(u =>

            (userFilterDto.text == null

            && userFilterDto.email == null && userFilterDto.userType == null



            && userFilterDto.name == null) ? u.FullName.Contains("") :


            (twoArePassed ?

            (u.NormalizedEmail.Contains(userFilterDto.email.ToUpper())
            && u.FullName.ToLower().Contains(userFilterDto.name.ToLower()))

            :

            userFilterDto.email != null ? u.NormalizedEmail.Contains(userFilterDto.email.ToUpper())

            :

            userFilterDto.name != null ? u.FullName.ToLower().Contains(userFilterDto.name.ToLower())

            :

            userFilterDto.text != null && userFilterDto.email == null ?

            (u.NormalizedEmail.Contains(userFilterDto.text.ToUpper()) || u.FullName.ToLower().Contains(userFilterDto.text.ToLower()))

            :

            u.FullName.Contains(""))).AsNoTracking().

                        Select(u => new UserGroupDtoView
                        {
                            Email = u.Email,
                            FullName = u.FullName,
                            Id = u.Id,
                            PhoneNumber = u.PhoneNumber,
                            UserName = u.UserName,
                            Locked = CheckLockState(u.LockoutEnabled, u.LockoutEnd),
                            Confirmed = u.EmailConfirmed,
                            Roles = u.UserRoles.Select(r => r.RoleId).ToList(),
                            UserGroups = u.UserGroups.Select(o => o.Group.GroupName).ToList().Count > 0 ? u.UserGroups.Select(o => new ValueId { Id = o.GroupId, Value = o.Group.GroupName }).ToList() : defaultGroup,
                            Enable2FA = u.TwoFactorEnabled
                        }).ToListAsync();

            if(userFilterDto.userType?.Count() > 0 || userFilterDto.userGroups?.Count() > 0 || userFilterDto.isLocked != null || userFilterDto.isConfirmed != null)
            {

                query = query.Where(q => 
                ((userFilterDto.userType == null || userFilterDto.userType?.Count < 1) || (userFilterDto.userType != null && q.Roles.Any(w => userFilterDto.userType.Contains(w)))) 
                && (!userFilterDto.isLocked.HasValue || q.Locked == userFilterDto.isLocked) 
                && (!userFilterDto.isConfirmed.HasValue || q.Confirmed == userFilterDto.isConfirmed)
                && ((userFilterDto.userGroups == null || userFilterDto.userGroups?.Count < 1) || (userFilterDto.userGroups!= null && q.UserGroups.Select(e => e.Id).Any(w => userFilterDto.userGroups.Contains(w))))).ToList();
            }

            int total = query.Count();

            return new ListCount
            {
                Count = total,
                Items = query.Skip((userFilterDto.pageIndex - 1) * userFilterDto.pageSize).Take(userFilterDto.pageSize)
            };
        }

        public async Task<ListCount> SearchFilterUsers(string text = null, string name = null, string email = null,
                                                       int pageIndex = 1, int pageSize = 25, string lang = "ar")
        {

            bool twoArePassed = name != null && email != null;


            APIResult result = new APIResult();



            var query = await _DbContext.Users.Where(u => twoArePassed ?

            (u.NormalizedEmail.Contains(email.ToUpper()) && u.FullName.ToLower().Contains(name.ToLower())) :


            email != null ? u.NormalizedEmail.Contains(email.ToUpper()) : name != null ? u.FullName.ToLower().Contains(name.ToLower()) :

            text != null ? (u.NormalizedEmail.Contains(text.ToUpper()) || u.FullName.ToLower().Contains(text.ToLower())) : u.FullName.Contains("")).AsNoTracking().

                        Select(u => new UserView
                        {
                            Email = u.Email,
                            FullName = u.FullName,
                            Id = u.Id,
                            PhoneNumber = u.PhoneNumber,
                            UserName = u.UserName,
                            Locked = CheckLockState(u.LockoutEnabled, u.LockoutEnd),
                            Confirmed = u.EmailConfirmed,
                            Enable2FA = u.TwoFactorEnabled,

                        }).ToListAsync();

            int total = query.Count();



            return new ListCount
            {
                Count = total,
                Items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };
        }


        public async Task<UserView> Search(string email)
        {
            var query = await _DbContext.Users.Where(u => u.NormalizedEmail == email.ToUpper()).AsNoTracking().
                        Select(u => new UserView
                        {
                            Email = u.Email,
                            FullName = u.FullName,
                            Id = u.Id,
                            PhoneNumber = u.PhoneNumber,
                            UserName = u.UserName,
                            Locked = CheckLockState(u.LockoutEnabled, u.LockoutEnd),
                            Confirmed = u.EmailConfirmed,
                            Enable2FA = u.TwoFactorEnabled,
                        }).FirstOrDefaultAsync();
            int total = await _DbContext.Users.CountAsync();
            return query;
        }
        public async Task<APIResult> GetUserToken(int userId, int userType)
        {
            APIResult result = await GetLocalUserId(userId, userType);
            if (result.Id < 0)
            {
                return result;
            }
            var tokens = await _DbContext.FcmTokens.Where(u => u.UserId == result.Id).Select(t => t.Token).ToListAsync();
            return result.SuccessMe(result.Id, "", true, APIResult.RESPONSE_CODE.OK, tokens);
        }

        public async Task<APIResult> CheckEMailAddress(string email, string lang)
        {
            APIResult result = new();
            try
            {
                MailAddress addr = new MailAddress(email);
                return result.SuccessMe(1, "Ok", false, APIResult.RESPONSE_CODE.OK, addr.User);
            }

            catch (ArgumentNullException)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "EmptyEmail"), true);
            }

            catch (FormatException)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "InvalidEmailFormat") + " : " + email, true);
            }

            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "EmailError") + " : " + email, true);
            }

        }

        public static bool CheckLockState(bool lockoutEnabled, DateTimeOffset? lockoutEnd)
        {
            return (!lockoutEnabled) ? false : lockoutEnd.HasValue && lockoutEnd.Value.ToLocalTime().LocalDateTime > DateTime.Now ? true : false;
        }

        public async Task<List<ValueId>> GetRelatedUsers(int userId, string lang)
        {
            return await _DbContext.Users.Where(u => u.EntityId == userId).Select(u => new ValueId { Id = u.Id, Value = string.IsNullOrWhiteSpace(u.FullName) ? u.UserName : u.FullName }).ToListAsync();
        }
        
        // PP
        public async Task<APIResult> CreateParticipantUser(BasicUserInfo dto, string lang, bool outerReq)
        {
            APIResult result = new();
            if (!dto.HasIdentity()) // So create user directly
            {
                //return result.FailMe(-1, "يرجى ادخال بيان تعريف واحد على الأقل لكل مشترك");
                dto.UserName = INVALID_EMAIL_PREFIX + GetNewValueBySec();
                dto.Email = dto.UserName + INVALID_EMAIL_SUFFIX;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.UUID))
                {
                    var u = await _DbContext.Users.Where(x => x.UuId == dto.UUID.Trim()).FirstOrDefaultAsync();
                    if (u != null)
                    {

                        if (!string.IsNullOrWhiteSpace(dto.EmiratesId) && !string.IsNullOrWhiteSpace(u.EmiratesId) && dto.EmiratesId != u.EmiratesId)
                        {
                            _logger.LogInformation(Translation.getMessage(lang, "SameUserIdentity") + "- UUID: " + dto.UUID);
                            //---*********-------Stop check
                           // return result.FailMe(-1, Translation.getMessage(lang, "SameUserIdentity") + " : " + dto.UUID);
                        }
                        return result.SuccessMe(u.Id, "UUID Existed Before", true, APIResult.RESPONSE_CODE.CREATED, u.Email);
                    }
                }
                //--Now UUID Id not found --------
                if (!string.IsNullOrWhiteSpace(dto.EmiratesId))
                {
                    var u = await _DbContext.Users.Where(x => x.EmiratesId == dto.EmiratesId.Trim()).FirstOrDefaultAsync();
                    if (u != null)
                    {

                        if (!string.IsNullOrWhiteSpace(dto.UUID) && !string.IsNullOrWhiteSpace(u.UuId) && dto.UUID != u.UuId)
                        {
                            _logger.LogInformation(Translation.getMessage(lang, "SameUserIdentity") + "- EmiratesId: " + dto.EmiratesId);
                            //---*********-------Stop check 
                            // return result.FailMe(-1, Translation.getMessage(lang, "SameUserIdentity") + " : " + dto.EmiratesId);
                        }
                        return result.SuccessMe(u.Id, "EmiratesID Existed Before", true, APIResult.RESPONSE_CODE.CREATED, u.Email);
                    }
                }
                bool searchEmail = false;
                if (string.IsNullOrEmpty(dto.Email))
                {
                    dto.UserName = INVALID_EMAIL_PREFIX + GetNewValueBySec();
                    dto.Email = dto.UserName + INVALID_EMAIL_SUFFIX;
                }
                else
                {
                    searchEmail = true;
                    var addr = await CheckEMailAddress(dto.Email, lang);
                    if (addr.Id < 0)
                    {
                        return addr;
                    }
                    if (string.IsNullOrEmpty(dto.FullName))
                    {
                        dto.FullName = addr.Result;
                    }
                }
                //--Now Emerats Id & UUID not found --------
                if (searchEmail)
                {
                    var u = await _DbContext.Users.Where(x => x.NormalizedEmail == dto.Email.ToUpper()).FirstOrDefaultAsync();
                    if (u != null)
                    {
                        return result.SuccessMe(u.Id, "email Existed Before", true, APIResult.RESPONSE_CODE.CREATED, u.Email);
                    }
                }
            }
            //if (dto.PhoneNumber != null && dto.PhoneNumber.Length > 0)
            //{
            //    dto.PhoneNumber = GetPhoneNumberWithCode(dto.PhoneNumber);
            //    if (dto.PhoneNumber.Length > 25)
            //    {
            //        result.FailMe(-1, "Invalid PhoneNumber");
            //        return result;
            //    }
            //}

            User newUser = new User()
            {
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.PasswordHash,
                EmailConfirmed = false, // ----------Switched 
                NormalizedEmail = dto.Email.ToUpper(),
                Email = dto.Email,
                NormalizedUserName = dto.UserName.ToLower(),
                UserName = dto.UserName,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                FullName = dto.FullName,
                EmiratesId = dto.EmiratesId,
                CreatedDate = DateTime.Now,
                UuId = dto.UUID,
                ProfileStatus = Convert.ToInt32(PROFILE_STATUS.SUSPENDED)
            };

            IdentityResult createUserRes = new IdentityResult();
            try
            {
                createUserRes = await _userManager.CreateAsync(newUser, dto.PasswordHash);
            }
            catch (Exception ex)
            {
                string obj = " fullName is " + dto.FullName + " emaritId is " + dto.EmiratesId;
                _logger.LogInformation("Error in create new user the error message is" + ex.Message + " for the user " + obj);
                return result.FailMe(-1, "UserCreateError" + ex.Message);
            }
            if (!createUserRes.Succeeded)
            {
                var errors = createUserRes.Errors.Select(x => x.Description).ToList();
                string errorResult = string.Empty;
                foreach (var x in errors)
                {
                    errorResult = errorResult + " , " + x;
                }
                return result.FailMe(-1, "User creating fails: " + errorResult);
            }
            if (newUser.Id == 0)
            {
                return result.FailMe(-1, "UserCreateError" + " identity is deleted.");
            }
            var assignRole = await _userManager.AddToRoleAsync(newUser, DefaultUserPolicy);
            if (!assignRole.Succeeded)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "UserRolesError"), true, APIResult.RESPONSE_CODE.UnavailableForLegalReasons);
            }
            return result.SuccessMe(newUser.Id, "User Created", true, APIResult.RESPONSE_CODE.CREATED, dto.Email);
        }

        public async Task Update(UserDto userInfo)
        {
            User userToUpdate = new User
            {
                Address = userInfo.Address,
                BirthdayDate = userInfo.BirthdayDate,
                Email = userInfo.Email,
                EmailLang = userInfo.EmailLang,
                EmiratesId = userInfo.EmiratesId,
                FullName = userInfo.FullName,
                Gender = userInfo.Gender,
                Id = userInfo.Id,
                Image = userInfo.Image,
                PhoneNumber = userInfo.PhoneNumber,
                SmsLang = userInfo.SmsLang,
                TelNo = userInfo.TelNo,
                ProfileStatus = (int)userInfo.ProfileStatus,
                UserName = userInfo.UserName,
                AreaId = userInfo.AreaId,
                NatId = userInfo.NatId
            };

            _DbContext.Users.Update(userToUpdate);
            await _DbContext.SaveChangesAsync();

        }

        public async Task<APIResult> LogIn(LogInDto logInDto, string lang)
        {
            var user = await _userManager.FindByEmailAsync(logInDto.Email);

            if (user == null)
            {
                return new APIResult(-1, null, APIResult.RESPONSE_CODE.PageNotFound, [Translation.getMessage(lang, "UserEmailError")]);
            }
            SignInResult resSignin = await _signInManager.PasswordSignInAsync(user, logInDto.PassWord, false, lockoutOnFailure: true);
            if (resSignin.IsNotAllowed)
            {
                if (user.ProfileStatus != null && user.ProfileStatus == Convert.ToInt32(PROFILE_STATUS.SUSPENDED))// User Is created without his knowing 
                {
                    if (!await SendActivation(user))
                    {
                        return new APIResult(-1, null, APIResult.RESPONSE_CODE.NoResponse, [Translation.getMessage(lang, "InActiveEmail")]);
                    }
                    else
                    {
                        return new APIResult(1, null, APIResult.RESPONSE_CODE.OK, ["Check your mail please"]);
                    }
                }
                return new APIResult(-1, null, APIResult.RESPONSE_CODE.BadRequest, [Translation.getMessage(lang, "UserAccountInactive")]);
            }
            if (resSignin.IsLockedOut)
            {
                return new APIResult(-1, null, APIResult.RESPONSE_CODE.BadRequest, [Translation.getMessage(lang, "OTPUserAccountLocked")]);
            }
            if (resSignin.RequiresTwoFactor)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "mock");
                SendNotificationRepository sender = new (_DbContext, _mailSetting, _smsSetting, _iGeneralRepository, _iNotificationLogRepository);
                var send_otp_result = await sender.SendOTPCode(user.Id, token, user.PhoneNumber, user.Email, lang);
                if (send_otp_result.Code == APIResult.RESPONSE_CODE.OK)
                    return new APIResult(1, new TwoFactorRequiredDto{userId = user.Id}, APIResult.RESPONSE_CODE.SecondFactorRequired, ["Second factor required"]);
                return send_otp_result;
            }

            if (!resSignin.Succeeded)
                return new APIResult(-1, false, APIResult.RESPONSE_CODE.Unauthorized, [Translation.getMessage(lang, "wrongPassword")]);

            await LogUserSignInEvent(user, logInDto.UA);
            return await PrepareUserLoggedInResponse(user);
        }

        public async Task<APIResult> LogInWithToken(string token)
        {
            APIResult result = new();
            JwtSecurityTokenHandler validator = new();
            ClaimsPrincipal claims = null;
            try {
                claims = validator.ValidateToken(token, new TokenValidationParameters {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                }, out _);
            } catch (SecurityTokenException e) {
                _logger.LogInformation("Failed LogInWithToken attempt, token={}, error={}", token, e.Message);
                return result.FailMe(-1);
            }
            var user_id_claim = claims.FindFirst((claim) => claim.Type == "user_id");
            if (user_id_claim == null)
                return result.FailMe(-1);
            _ = int.TryParse(user_id_claim.Value, out int user_id);
            var user = _DbContext.Users.Find(user_id);
            if (user == null)
                return result.FailMe(-1);
            await _signInManager.SignInAsync(user, false);
            return await PrepareUserLoggedInResponse(user);
        }

        public async Task<APIResult> VerifyOTP(OtpLogInDto otpLogInDto, string lang)
        {
            APIResult result = new();
            var signInResult = await _signInManager.TwoFactorSignInAsync("mock", otpLogInDto.Number, true, false);
            if (!signInResult.Succeeded)
            {
                _logger.LogInformation("Log in failed: lockedOut={} allowed={}", signInResult.IsLockedOut, !signInResult.IsNotAllowed);
                var apiResult = new APIResult(-1, false, APIResult.RESPONSE_CODE.Unauthorized, []);
                if (signInResult.IsLockedOut)
                    apiResult.Message.Add("2FA failed. Account is locked out");
                else if (signInResult.IsNotAllowed)
                    apiResult.Message.Add("2FA failed. Authentication is disabled for this account");
                else
                    apiResult.Message.Add("2FA failed. Incorrect OTP");
                _logger.LogInformation("{}", apiResult.Message[0]);
                return result;
            }

            var user = await _userManager.FindByIdAsync(otpLogInDto.userId.ToString());
            if (user == null)
            {
                _logger.LogError("User logged in sucessfully but not found in db");
                return result;
            }

            await LogUserSignInEvent(user, otpLogInDto.UA);
            return await PrepareUserLoggedInResponse(user);
        }

        private string BuildCreateConfUrl(int userId, string pathToTokenLogin) {
            var createConfAuthToken = GenerateToken([new Claim("user_id", userId.ToString())], _jwt.Key, true);
            var redirectUrl = _IConfiguration["Meeting:host"] + "/meet/panel/events";

            // var baseUrl = _IConfiguration["Meeting:host"] + pathToTokenLogin;
            var baseUrl = "http://localhost:5000" + pathToTokenLogin;
            var urlParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            urlParams["token"] = createConfAuthToken;
            urlParams["redirectUrl"] = redirectUrl;
            return baseUrl + "?" + urlParams.ToString();
        }

        public async Task<APIResult> ViewMyProfile(int currentUserId, string pathToTokenLogin, string lang)
        {
            APIResult res = new APIResult();
            try
            {
                List<ValueId> defaultGroup = new List<ValueId>();
                defaultGroup.Add(new ValueId { Id = 0, Value = "Default Group" });


                var currentUser = await _DbContext.Users.Include(u => u.UserPhotos).Include(u => u.UserGroups).ThenInclude(g =>g.Group).Where(u => u.Id == currentUserId)/*.Include(e => e.Attachments)*/.AsNoTracking().FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    return res.FailMe(-1, "User not exist");
                }

                UserProfileGetDto userProfileGetDto = new UserProfileGetDto()
                {
                    FullName = currentUser.FullName,
                    Email = currentUser.Email,
                    PhoneNumber = currentUser.PhoneNumber,

                    Roles  = await _IRoleRepository.GetRolesIdByUserId(currentUser.Id),
                  
                    File = currentUser.UserPhotos.Select(w => new FileGetDto
                    {
                        Id = w.Id,
                        FilePath = w.FilePath,
                        FileName = w.FileName,
                        FileSize = w.FileSize
                    }),
                    UserGroups = currentUser.UserGroups.Select(o => o.Group.GroupName).ToList().Count > 0 ? currentUser.UserGroups.Select(o => new ValueId { Id = o.GroupId, Value = o.Group.GroupName }).ToList() : defaultGroup,
                    CreateConfLink = BuildCreateConfUrl(currentUserId, pathToTokenLogin),
                };

                return res.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, userProfileGetDto);

            }

            catch
            {
                return res.FailMe(-1, "Failed to view profile");
            }
        }


        public async Task<APIResult> EditProfilePhoto(int id, FilePostDto filePostDto, bool fromUg, bool updateRoles, string lang)
        {
            APIResult res = new APIResult();


            try
            {

                var user = await _DbContext.Users.Where(u => u.Id == id).Include(e => e.UserPhotos).AsNoTracking().FirstOrDefaultAsync();

                if (user == null)
                {
                    return res.FailMe(-1, "User not found");
                }


                if (filePostDto.UserPhoto == null)
                {
                    return res.FailMe(-1, "Please add an image");
                }

                if (filePostDto.UserPhoto != null)
                {
                    if (user.UserPhotos.Count() < 1)
                    {
                        //foreach (var attach in userProfilePostDto.ProfilePhoto)
                        //{
                        var attachCreated = await _IFileRepository.Create(filePostDto.UserPhoto);
                        Files resAttachment = (Files)attachCreated.Result;
                        user.UserPhotos.Add(resAttachment);
                        //}
                    }

                    else
                    {

                        var existingAttach = user.UserPhotos.FirstOrDefault();


                        await _IFileRepository.Delete(existingAttach.Id);

                        user.UserPhotos.Clear();

                        //foreach (var attach in userProfilePostDto.ProfilePhoto)
                        //{
                        var attachCreated = await _IFileRepository.Create(filePostDto.UserPhoto);
                        Files resAttachment = (Files)attachCreated.Result;
                        user.UserPhotos.Add(resAttachment);
                        //}

                    }

                }

                _DbContext.Users.Update(user);

                await _DbContext.SaveChangesAsync();

                //scope.Complete();

                return res.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));

            }
            catch
            {

                return res.FailMe(-1, "Could update profile");
            }
        }

        private async Task<APIResult> PrepareUserLoggedInResponse(User user)
        {
            LogInResultDto result = new();

            var lastUserLogIn = await _DbContext.UserLogins.Where(x => x.UserId == user.Id).OrderByDescending(x => x.LoginDate).Take(1).LastOrDefaultAsync();
            DateTime? lastLogInDate = (lastUserLogIn != null) ? lastUserLogIn.LoginDate : DateTime.Now;

            Claim[] claims = GenerateClaims(user);
            string jwt = GenerateToken(claims, _jwt.Key);

            try
            {
                // get user image.
                if (user.Image != null/* && _IFilesUploaderRepository.FileExist(user.Image)*/)
                {
                    result.Image = user.Image;
                }
                else
                {
                    /* if (_IFilesUploaderRepository.FileExist("User_images", "default.jpg"))
                     {
                         result.Image = Path.Combine("User_images", "default.jpg");
                     }*/
                }
            }
            catch
            {

            }
            var remoteEntityId = user.EntityId != null ? await _DbContext.Users.Where(x => x.Id == user.EntityId).Select(x => x.UserId).FirstOrDefaultAsync() : null;
            result.Token = jwt;
            result.UserId = user.Id;
            result.UserName = user.UserName;
            result.FullName = user.FullName;
            result.Email = user.Email;
            result.PhoneNumber = user.PhoneNumber;
            result.EmirateId = user.EmiratesId;
            result.BirthdayDate = user.BirthdayDate;
            result.RolesName = await _userManager.GetRolesAsync(user);
            result.RolesId = await _IRoleRepository.GetRolesIdByUserId(user.Id);
            result.Address = user.Address;
            result.CountryId = user.NatId /*await getUserCountryId(user.NatId)*/;
            result.AreaId = user.AreaId;
            result.LastLogin = lastLogInDate;
            result.UserType = user.UserType;
            result.RemoteUserId = user.UserId;
            result.EntityId = user.EntityId;
            result.RemoteEntityId = remoteEntityId;
            //result.ShowAadel = !await IsEmployee(user.Id);

            return new APIResult(1, result, APIResult.RESPONSE_CODE.OK, ["Welcome"]);
        }

        private static APIResult PrepareUserLogInFailedResponse(APIResult.RESPONSE_CODE code)
        {
            APIResult result = new();
            string msg;
            if (code == APIResult.RESPONSE_CODE.Unauthorized)
                msg = "Invalid credentials";
            else if (code == APIResult.RESPONSE_CODE.Locked)
                msg = "Account is locked";
            else
                msg = "Internal Server Error";
            result.FailMe(-1, msg, false, code);
            return result;
        }

        private static APIResult PrepareTwoFactorRequiredResponse() {
            APIResult result = new();
            ;
            return result;
        }

        private async Task LogUserSignInEvent(User user, UserAgent ua)
        {
            try
            {
                bool UAData = ua != null;
                UserLogin userLogin = new()
                {
                    LoginProvider = "Local",
                    UserId = user.Id,
                    ProviderDisplayName = user.UserName,
                    ProviderKey = Guid.NewGuid().ToString(), //user.Id.ToString(),    // TODO get a key form the provider.
                    LoginDate = DateTime.Now,
                    Ip = UAData ? ua.IP : null,
                    BrowserName = UAData ? ua.BrowserName : null,
                    BrowserVersion = UAData ? ua.BrowserVersion : null,
                    Device = UAData ? ua.Device : null,
                    Os = UAData ? ua.OS : null,
                };

                await _DbContext.UserLogins.AddAsync(userLogin);
                await _DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += " inner exception is " + ex.InnerException.Message;
                }
                _logger.LogWarning("Failed to log user sign in event due to error: {}", message);
            }
        }
    }
}
