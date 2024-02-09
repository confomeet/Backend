using Microsoft.AspNetCore.Identity;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.DTOs.RoleDto;
using VideoProjectCore6.DTOs.UserDTO;



namespace VideoProjectCore6.Repositories.IUserRepository
{
    public interface IUserRepository
    {
        Task<bool> IsExist(string EmiratId);
        Task<UserDto> GetOne(int Id);
        public int GetUserID();
        public string GetUserName();
        public bool IsAdmin();
        public bool IsInspector();
        public bool IsEmployee();
        public Task<bool> IsEmployee(int userId);
        public Task<bool> HasRole(int userId, string policy);
        public string GetUserEid();
        public string GetUserEmail();
        public Task<APIResult> FindUserById(int id, string lang);
        public Task<int> VisitorsCount();
        public Task<APIResult> RefreshToken();
        public Task<LogInResultDto> LocalSignIn(LogInDto logInDto, string lang);
        public Task<List<RoleGetDto>> GetUserRoles(int userId, string lang);
        public Task<List<UserDto>> GetUsersRoles(string blindSearch, string lang);
        public Task<APIResult> EditProfile(int id, UserPostDto UserPostDto, bool fromUg, bool updateRoles, string lang);
        public Task<APIResult> EditUser(int id,int editBy, UserView dto, bool fromUg ,string lang);
        public Task<APIResult> ResetPasswordByToken(ResetPasswordDTO resetPasswordObject, string lang);
        public Task<bool> AddEditSignature(SignaturePostDto signaturePostDto, string lang);
        public Task<string> AddEditSignature64(string signatureBase64, string lang);
        public Task<bool> EditPassword(EditUserPasswordDTO editUserPasswordDTO, string lang);
        public Task<APIResult> RegisterAsync(RegisterDTO registerDTO, string lang, bool sendNotification, bool AddByAdmin = false);
        public Task<APIResult> ConfirmEmail(string token, string email, string lang);
        public Task<APIResult> ResendActivation(string email, string lang);     
        public Task<APIResult> SendResetPasswordEmail(MultiLangMessage multiLangMessage, string email, string lang);
        public Task<APIResult> CreateUser(UserPostDto UserPostDto, string ImageUrl, bool updateRoles, string lang, bool FromUg);
        public Task<APIResult> DeleteUser(int id);
        public Task<List<UserDto>> GetUsers();
        public Task<UserPermissionsDTO> GetUserPermissions(int userId);
        public Task<IdentityResult> EditUserRolesAsync(int userId, List<int> userRoles, string lang);
        Task<List<int>> GetUserClaimPermissions(int userId, string claimType);
        public void SignOut();
        public Task<Dictionary<int, string>> GetEmployees();
        public Task<CreateUserOldResultDto> CreateUserForOldAppParties(OldUserPostDto UserPostDto);
        public Task<APIResult> GetUserToken(int userId,int userType);
        public Task<APIResult> GetLocalUserId(int userId,int userType);
        public  Task<ListCount> GetUsers(string lang, int pageIndex = 1, int pageSize = 25);
        public  Task<APIResult> DisableAccount(int id,string lang="ar");
        public  Task<APIResult> EnableAccount(int id,string lang= "ar");
        public Task<APIResult> LockAccount(int id,int? forDays , string lang = "ar");
        public Task<APIResult> UnLockAccount(int id, string lang = "ar");
        public Task<APIResult> ActivateAccount(int id, string lang = "ar");
        public Task<ListCount> Search(string email,int pageIndex = 1, int pageSize = 25,string lang = "ar");
        public Task<UserView> Search(string email);
        public Task<ListCount> SearchFilterUsers(UserFilterDto userFilterDto, int currentUser, string lang = "ar");
        public Task<APIResult> CheckEMailAddress(string email,string lang);
        public Task<List<ValueId>> GetRelatedUsers(int userId, string lang);
        public Task<APIResult> CreateParticipantUser(BasicUserInfo dto, string lang, bool outerReq);

        public Task Update(UserDto userDto);
        public Task<ListCount> SearchFilterUsers(string text = null, string name = null, string email = null,
                                                       int pageIndex = 1, int pageSize = 25, string lang = "ar");
        //Task<APIResult> AuthenticateUserExternal(int v);
        // public Task<APIResult> GetLocalUserId(int userId);
        Task<APIResult> VerifyOTP(OtpLogInDto otpLogInDto, string lang);
        Task<APIResult> LogIn(LogInDto logInDto, string lang);

        Task<APIResult> ViewMyProfile(int currentUserId, string lang);

        Task<APIResult> EditProfilePhoto(int id, FilePostDto filePostDto, bool fromUg, bool updateRoles, string lang);
    }
}
