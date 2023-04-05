//using System;
//using System.Threading;
//using System.Threading.Tasks;
////using EngineCoreProject.IRepository.IEmail;
//using EngineCoreProject.IRepository.IPaymentRepository;
//using EngineCoreProject.IRepository.INotificationSettingRepository;
//using EngineCoreProject.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using EngineCoreProject.IRepository.IMeetingRepository;
//using EngineCoreProject.IRepository;
//using EngineCoreProject.IRepository.IFilesUploader;
//using EngineCoreProject.IRepository.IApplicationSetRepository;

//namespace EngineCoreProject.Services.Job
//{
//    public interface ISMSCronService
//    {
//        Task DoWork(CancellationToken cancellationToken);
//    }

//    public class SMSCronService : ISMSCronService
//    {
//        private readonly ILogger<SMSCronService> _logger;
//        private readonly IServiceScopeFactory scopeFactory;
//        private readonly ISendNotificationRepository _ISendNotificationRepository;

//        public SMSCronService(ILogger<SMSCronService> logger, IServiceScopeFactory scopeFactory, ISendNotificationRepository iSendNotificationRepository)
//        {
//            _logger = logger;
//            this.scopeFactory = scopeFactory;
//            _ISendNotificationRepository = iSendNotificationRepository;
//        }

//        public async Task DoWork(CancellationToken cancellationToken)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {

//                var _EngineCoreDBContext = scope.ServiceProvider.GetRequiredService<EngineCoreDBContext>();


//                try
//                {
//                    await _ISendNotificationRepository.ReSend(Constants.NOTIFICATION_SMS_CHANNEL, 150);
//                }
//                catch (Exception e)
//                {
//                    _logger.LogInformation("error in sending fail SMS notifications " + e.Message);
//                }

//            }

//            await Task.Delay(1000 * 20, cancellationToken);
//        }
//    }
//}
