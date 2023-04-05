using VideoProjectCore6.Services;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Models;

namespace EngineCoreProject.Services.Job
{
    public interface ICronService
    {
        Task DoWork(CancellationToken cancellationToken);
    }

    public class CronService : ICronService
    {
        private readonly ILogger<CronService> _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ISendNotificationRepository _ISendNotificationRepository;

        public CronService(ILogger<CronService> logger, IServiceScopeFactory scopeFactory,ISendNotificationRepository iSendNotificationRepository)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
            _ISendNotificationRepository = iSendNotificationRepository;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var _EngineCoreDBContext = scope.ServiceProvider.GetRequiredService<OraDbContext>();

                try
                {
                    await _ISendNotificationRepository.ReSend(Constants.NOTIFICATION_MAIL_CHANNEL, 50);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Error in sending fail Mail notifications " + e.Message);
                }
            }
            await Task.Delay(1000 * 20, cancellationToken);
        }
    }
}
