//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using EngineCoreProject.Models;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

//namespace EngineCoreProject.Services.Job
//{
//    public class SMSWorkerJob : CronJobService
//    {
//        private readonly ILogger<SMSWorkerJob> _logger;
//        private readonly IServiceProvider _serviceProvider;


//        //TODO basetime 3 minutes. 
//        public SMSWorkerJob(IScheduleConfig<SMSWorkerJob> config, ILogger<SMSWorkerJob> logger, IServiceProvider serviceProvider)
//  : base(config.CronExpression, config.TimeZoneInfo)
//        {
//            _logger = logger;
//            _serviceProvider = serviceProvider;
//        }

//        public override Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("SMS Worker Job  starts.");
//            return base.StartAsync(cancellationToken);
//        }

//        public override async Task DoWork(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} SMS Worker Job is working.");
//            try
//            {
//                using var scope = _serviceProvider.CreateScope();
//                var svc = scope.ServiceProvider.GetRequiredService<ISMSCronService>();
//                await svc.DoWork(cancellationToken);
//            }
//            catch (Exception e)
//            {

//                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ============error : " +e.Message.ToString());
//            }
//        }

//        public override Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("SMS Worker Job is stopping.");
//            return base.StopAsync(cancellationToken);
//        }
//    }
//}
