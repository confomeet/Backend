//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using EngineCoreProject.Models;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

//namespace EngineCoreProject.Services.Job
//{
//    public class DailyWorkerJob : CronJobService
//    {
//        private readonly ILogger<DailyWorkerJob> _logger;
//        private readonly IServiceProvider _serviceProvider;


//        //TODO basetime every 24 h  
//        public DailyWorkerJob(IScheduleConfig<DailyWorkerJob> config, ILogger<DailyWorkerJob> logger, IServiceProvider serviceProvider)
//  : base(config.CronExpression, config.TimeZoneInfo)
//        {
//            _logger = logger;
//            _serviceProvider = serviceProvider;
//        }

//        public override Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Daily Worker Job  starts.");
//            return base.StartAsync(cancellationToken);
//        }

//        public override async Task DoWork(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Daily Worker Job is working.");
//            try
//            {
//                using var scope = _serviceProvider.CreateScope();
//                var svc = scope.ServiceProvider.GetRequiredService<IDailyCronService>();
//                await svc.DoWork(cancellationToken);
//            }
//            catch (Exception e)
//            {

//                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ============error : " +e.Message.ToString());
//            }
//        }

//        public override Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Daily Worker Job is stopping.");
//            return base.StopAsync(cancellationToken);
//        }
//    }
//}
