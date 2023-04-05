using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Authentication;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IChannelRepository;
#nullable disable
namespace VideoProjectCore6.Services.Channels
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private IConfiguration _configuration;

        public ChannelRepository(OraDbContext dbContext, IConfiguration configuration, IGeneralRepository iGeneralRepository)
        {
            _DbContext = dbContext;
            _iGeneralRepository = iGeneralRepository;
            _configuration = configuration;

        }

        public List<ChannelGetDto> GetChannelsName(string lang)
        {
            var query = from srv in _DbContext.SysLookupTypes
                        join s in _DbContext.SysLookupValues on srv.Id equals s.LookupTypeId
                        join stg in _DbContext.SysTranslations on s.Shortcut equals stg.Shortcut
                        where srv.Value == Constants.NOTIFICATION_CHANNEL && stg.Lang == lang
                        select new ChannelGetDto { ChannelName = stg.Value, ChannelNameShortcut = stg.Shortcut, Id = s.Id };

            List<ChannelGetDto> ChannelName = query.ToList<ChannelGetDto>();

            return ChannelName;
        }

        public void AddChannelMailFirstSetting(ChannelMailFirstSetting config)
        {
            _configuration["ChannelMailFirstSetting:Host"] = config.Host;
            _configuration["ChannelMailFirstSetting:Port"] = config.Port.ToString();
            _configuration["ChannelMailFirstSetting:Mail"] = config.Mail;
            _configuration["ChannelMailFirstSetting:Password"] = config.Password;
        }

        public ChannelMailFirstSetting GetChannelMailFirstConfig()
        {
            return _configuration.GetSection("ChannelMailFirstSetting").Get<ChannelMailFirstSetting>();
        }


        public void TestEmailFirstSettingConnection(ChannelMailFirstSetting config, string lang)
        {
            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();

                client.Connect(config.Host, config.Port, false);
                client.Authenticate(config.Mail, config.Password);
                client.Disconnect(true);

            }
            catch (AuthenticationException)
            {
                throw new InvalidOperationException(Translation.getMessage(lang, "errorAuthentication"));
            }
            catch (SocketException)
            {
                throw new InvalidOperationException(Translation.getMessage(lang, "errorConfig"));
            }
        }
    }

}
