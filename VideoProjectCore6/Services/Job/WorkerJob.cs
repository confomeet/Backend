namespace EngineCoreProject.Services.Job
{
    public class WorkerJob : CronJobService
    {
        private readonly ILogger<WorkerJob> _logger;
        private readonly IServiceProvider _serviceProvider;



        public WorkerJob(IScheduleConfig<WorkerJob> config, ILogger<WorkerJob> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Immediate Job starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            //   _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob 2 is working.");
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<ICronService>();
                await svc.DoWork(cancellationToken);
            }
            catch (Exception e)
            {

                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ============error : " + e.Message.ToString());
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Immediate Job is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
