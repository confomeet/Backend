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
//    public interface IDailyCronService
//    {
//        Task DoWork(CancellationToken cancellationToken);
//    }

//    public class DailyCronService : IDailyCronService
//    {
//        private readonly ILogger<DailyCronService> _logger;
//        private readonly IServiceScopeFactory scopeFactory;
//        private readonly IApplicationRepository _IApplicationRepository;
//        private readonly IMeetingRepository _ImeetingRepository;
//        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;

//        public DailyCronService(ILogger<DailyCronService> logger, IServiceScopeFactory scopeFactory, IMeetingRepository imeetingRepository,
//                           IApplicationRepository iApplicationRepository, IFilesUploaderRepositiory iFilesUploaderRepositiory)
//        {
//            _logger = logger;
//            this.scopeFactory = scopeFactory;
//            _IApplicationRepository = iApplicationRepository;
//            _ImeetingRepository = imeetingRepository;
//            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
//        }

//        public async Task DoWork(CancellationToken cancellationToken)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {

//                var _EngineCoreDBContext = scope.ServiceProvider.GetRequiredService<EngineCoreDBContext>();

//                try
//                {
//                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Daily Worker Job is working for RejectApps.");
//                   await _IApplicationRepository.RejectApps(null);
//                }
//                catch (Exception e) 
//                {
//                   _logger.LogInformation("error in Daily Worker Job in RejectApps error is  " + e.Message);
//                }

//                try
//                {
//                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Daily Worker Job is working for DailyNotify.");
//                    await _IApplicationRepository.DailyNotify();
//                }
//                catch (Exception e)
//                {
//                    _logger.LogInformation("error in Daily Worker Job in DailyNotify error is  " + e.Message);
//                }

//                try
//                {
//                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ==== get meeting logger >>");
//                    await _ImeetingRepository.GetMeetingLogger();
//                }
//                catch (Exception e)
//                {
//                    _logger.LogInformation(" error in Daily Worker Job in get meeting logger error is " + e.Message);
//                }

//                try
//                {
//                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ==== DeleteTemporaryFiles >>");
//                    _logger.LogInformation(" the count of the deleted files is : " + _IFilesUploaderRepositiory.DeleteTemporaryFiles(DateTime.Now).ToString());
//                }
//                catch (Exception e)
//                {
//                    _logger.LogInformation(" error in Daily Worker Job in DeleteTemporaryFiles " + e.Message);
//                }
//            }

//            await Task.Delay(1000 * 20, cancellationToken);
//        }
//    }
//}
