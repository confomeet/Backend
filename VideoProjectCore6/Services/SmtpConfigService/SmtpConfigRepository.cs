using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.SmtpConfigDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.ISmtpConfigRepository;

namespace VideoProjectCore6.Services.SmtpConfigService
{
    public class SmtpConfigRepository : ISmtpConfigRepository
    {

        //private readonly IFilesUploaderRepository _iFilesUploaderRepositiory;
        private readonly OraDbContext _context;

        public SmtpConfigRepository(OraDbContext context/*, IFilesUploaderRepository iFilesUploaderRepositiory*/)
        {
            _context = context;
            //_iFilesUploaderRepositiory = iFilesUploaderRepositiory;
        }

        public async Task<APIResult> CreateUpdate(int currentUserId, SmtpConfigPostDto smtpConfigPostDto, string lang)
        {
            APIResult result = new APIResult();

            DateTime currentTime = DateTime.Now;

            try
            {
                var existingConfig = await _context.SmtpConfigs.FirstOrDefaultAsync();

                if (existingConfig != null)
                {
                    existingConfig.DisplayName = smtpConfigPostDto.DisplayName;
                    existingConfig.Email = smtpConfigPostDto.Email;
                    existingConfig.Password = string.IsNullOrEmpty(smtpConfigPostDto.Password) ? existingConfig.Password : smtpConfigPostDto.Password;
                    existingConfig.Port = smtpConfigPostDto.Port;
                    existingConfig.Host = smtpConfigPostDto.Host;
                    existingConfig.UpdatedById = currentUserId;
                    existingConfig.UpdatedAt = currentTime;

                    _context.SmtpConfigs.Update(existingConfig);
                    await _context.SaveChangesAsync();

                    return result.SuccessMe(1, "Success", false, APIResult.RESPONSE_CODE.CREATED, new SmtpConfigGetDto
                    {
                        Id = existingConfig.Id,
                        DisplayName = smtpConfigPostDto.DisplayName,
                        Email = smtpConfigPostDto.Email,
                        Port = smtpConfigPostDto.Port,
                        Host = smtpConfigPostDto.Host
                    });
                }

                SmtpConfig newSmtpConfig = new SmtpConfig
                {
                    DisplayName = smtpConfigPostDto.DisplayName,
                    Email = smtpConfigPostDto.Email,
                    Port = smtpConfigPostDto.Port,
                    Password = smtpConfigPostDto.Password,
                    Host = smtpConfigPostDto.Host,
                    CreatedById = currentUserId,
                    CreatedAt = currentTime,
                    UpdatedAt = currentTime
                };

                _context.SmtpConfigs.Add(newSmtpConfig);

                await _context.SaveChangesAsync();

                return result.SuccessMe(1, "Success", false, APIResult.RESPONSE_CODE.CREATED, new SmtpConfigGetDto
                {
                    Id = newSmtpConfig.Id,
                    DisplayName = smtpConfigPostDto.DisplayName,
                    Email = smtpConfigPostDto.Email,
                    Port = smtpConfigPostDto.Port,
                    Host = smtpConfigPostDto.Host
                });
            } 
            
            catch
            {
                return result.FailMe(-1, "Error creating the SMTP CONFIGURATION");
            }
        }

        public async Task<APIResult> DisplaySmtpConfig(string lang)
        {
            APIResult result = new APIResult();


            try
            {
                var existingConfig = await _context.SmtpConfigs.FirstOrDefaultAsync();

                if(existingConfig == null)
                {
                    return result.FailMe(-1, "Could not find any smtp configuration");
                }

                SmtpConfigGetDto smtpConfigGetDto = new SmtpConfigGetDto
                {
                    Id = existingConfig.Id,
                    DisplayName = existingConfig.DisplayName,
                    Email = existingConfig.Email,
                    Port = existingConfig.Port,
                    Host = existingConfig.Host
                };

                return result.SuccessMe(1, "Success", false, APIResult.RESPONSE_CODE.CREATED, smtpConfigGetDto);

            }
            catch
            {
                return result.FailMe(-1, "Error gettting configuration");
            }

        }
    }
}
