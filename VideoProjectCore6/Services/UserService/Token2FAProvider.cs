using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using OtpNet;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Services.UserService
{
    public class Token2FAProvider(OraDbContext dbContext, ILogger<Token2FAProvider> logger, IConfiguration configuration) : IUserTwoFactorTokenProvider<User>
    {
        private readonly OraDbContext _dbContext = dbContext;
        private readonly ILogger<Token2FAProvider> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
        {
            return Task.FromResult(true);
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
        {
            var totp = new Totp(Base32Encoding.ToBytes(Constants.otpBase32Secret));
            var code = totp.ComputeTotp();

            OtpLog? otp = null;
            try
            {
                otp = await _dbContext.OtpLogs.Where(otp => otp.UserId == user.Id).FirstOrDefaultAsync();
                if (otp != null)
                {
                    otp.OtpCode = code;
                    otp.GeneratedDate = DateTime.UtcNow;
                    _logger.LogDebug("Updating TOTP token for user {}", user.Id);
                }
                else
                {
                    otp = new()
                    {
                        GeneratedDate = DateTime.UtcNow,
                        UserId = user.Id,
                        OtpCode = code
                    };
                    await _dbContext.OtpLogs.AddAsync(otp);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError("Failed to save TOTP code of user {} to db: {}", user.Id, e.Message);
                return "";
            }
            _logger.LogTrace("Created new TOTP id={} code={} for user_id={}", otp.Id, code, user.Id);
            return code;
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
        {
            try
            {
                var otp = await _dbContext.OtpLogs.Where(row => row.UserId == user.Id).FirstOrDefaultAsync();
                if (otp == null)
                {
                    _logger.LogTrace("TOTP code validation failed, no matching record in database");
                    return false;
                }
                if (otp.OtpCode == token)
                {
                    _dbContext.OtpLogs.Remove(otp);
                    await _dbContext.SaveChangesAsync();
                }
                else if (otp.GeneratedDate.Add(GetOTPLifetime(otp)) < DateTime.UtcNow)
                {
                    _logger.LogError("TOTP code validation failed: expired. user id is {}", user.Id);
                    return false;
                }
                else
                {
                    _logger.LogTrace("TOTP code validation failed: user.Id={}  token={}  expected={}", user.Id, token, otp.OtpCode);
                    return false;
                }
            }
            catch (DbUpdateException e)
            {
                _logger.LogError("Failed to validate TOTP code of user {} due to DB error: {}", user.Id, e.Message);
                return false;
            }
            _logger.LogTrace("TOTP code validation for user_id={} succeeded", user.Id);
            return true;
        }

        private TimeSpan GetOTPLifetime(OtpLog otp) {
            if (_configuration["CONFOMEET_OTP_PERIOD_IN_MINUTES"] == null)
            {
                _logger.LogWarning("CONFOMEET_OTP_PERIOD_IN_MINUTES is not configured, use {}", Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
                return TimeSpan.FromMinutes(Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
            }

            if (int.TryParse(_configuration["CONFOMEET_OTP_PERIOD_IN_MINUTES"], out int otpPeriodInMinutes)) {
                if (otpPeriodInMinutes < 1) {
                    _logger.LogWarning("CONFOMEET_OTP_PERIOD_IN_MINUTES < 1 is invalid, use {}", Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
                    return TimeSpan.FromMinutes(Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
                }
                return TimeSpan.FromMinutes(otpPeriodInMinutes);
            }

            _logger.LogError("{} is incorrect value for CONFOMEET_OTP_PERIOD_IN_MINUTES setting use {}", _configuration["CONFOMEET_OTP_PERIOD_IN_MINUTES"], Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
            return TimeSpan.FromMinutes(Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING);
        }
    }
}