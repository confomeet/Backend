using VideoProjectCore6.DTOs.SmtpConfigDto;
using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.Repositories.ISmtpConfigRepository
{
    public interface ISmtpConfigRepository
    {
        Task<APIResult> CreateUpdate(int currentUser, SmtpConfigPostDto smtpConfigPostDto, string lang);

        Task<APIResult> DisplaySmtpConfig(string lang);
    }
}
