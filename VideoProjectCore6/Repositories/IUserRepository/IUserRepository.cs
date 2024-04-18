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
        public int GetUserID();
        public string GetUserName();
        public bool IsAdmin();
        public Task<APIResult> FindUserById(int id, string lang);
        public Task<APIResult> RefreshToken(string lang);
        public Task<List<RoleGetDto>> GetUserRoles(int userId, string lang);
        public Task<APIResult> EditProfile(int id, UserPostDto UserPostDto, bool fromUg, bool updateRoles, string lang);
        public Task<APIResult> EditUser(int id,int editBy, UserView dto, bool fromUg ,string lang);
        public Task<APIResult> ResetPasswordByToken(ResetPasswordDTO resetPasswordObject, string lang);
        public Task<bool> EditPassword(EditUserPasswordDTO editUserPasswordDTO, string lang);
        public Task<APIResult> RegisterAsync(RegisterDTO registerDTO, string lang, bool sendNotification, bool AddByAdmin = false);
        public Task<APIResult> ConfirmEmail(string token, string email, string lang);
        public Task<APIResult> ResendActivation(string email, string lang);
        public Task<APIResult> SendResetPasswordEmail(MultiLangMessage multiLangMessage, string email, string lang);
        public Task<APIResult> CreateUser(UserPostDto UserPostDto, string ImageUrl, bool updateRoles, string lang);
        public Task<APIResult> DeleteUser(int id, string lang);
        public Task<UserPermissionsDTO> GetUserPermissions(int userId);
        public Task<IdentityResult> EditUserRolesAsync(int userId, List<int> userRoles, string lang);
        Task<List<int>> GetUserClaimPermissions(int userId, string claimType);
        public  Task<ListCount> GetUsers(string lang, int pageIndex = 1, int pageSize = 25);
        public  Task<APIResult> DisableAccount(int id,string lang="en");
        public  Task<APIResult> EnableAccount(int id,string lang= "en");
        public Task<APIResult> LockAccount(int id,int? forDays , string lang = "en");
        public Task<APIResult> UnLockAccount(int id, string lang = "en");
        public Task<APIResult> ActivateAccount(int id, string lang = "en");
        public Task<UserView> Search(string email);
        public Task<ListCount> SearchFilterUsers(UserFilterDto userFilterDto, int currentUser, string lang = "en");
        public Task<APIResult> CheckEMailAddress(string email,string lang);
        public Task<ListCount> SearchFilterUsers(string? text = null, string? name = null, string? email = null,
                                                       int pageIndex = 1, int pageSize = 25, string lang = "en");
        Task<APIResult> VerifyOTP(OtpLogInDto otpLogInDto, string lang);
        Task<APIResult> LogIn(LogInDto logInDto, string lang);
        Task<APIResult> LogInWithToken(string token);

        Task<APIResult> ViewMyProfile(int currentUserId, string pathToTokenLogin, string lang);

        Task<APIResult> EditProfilePhoto(int id, FilePostDto filePostDto, bool fromUg, bool updateRoles, string lang);

        public Task<LogInResultDto> LogInExternal(string externalId, string providerName, string email, string fullName);
    }
}
