using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories;
using Microsoft.AspNetCore.SignalR;
using VideoProjectCore6.Hubs;
using VideoProjectCore6.Utility;
#nullable disable
namespace VideoProjectCore6.Services.NotificationService
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly OraDbContext _DbContext;
        ValidatorException _exception;
        private readonly IServiceProvider _services;


        public NotificationLogRepository(OraDbContext DBContext, IServiceProvider serviceProvider)
        {
            _DbContext = DBContext;
            _exception = new ValidatorException();
            _services = serviceProvider;
        }

        public async Task<int> AddNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto)
        {
            // TODO validation for notification.
            List<NotificationLog> notifyLog = new List<NotificationLog>();
            foreach (var notify in notificationsLogPostDto)
            {
                notifyLog.Add(notify.GetEntity());
            }
            await _DbContext.NotificationLogs.AddRangeAsync(notifyLog);

            await _DbContext.SaveChangesAsync();

            await UserNotifySignalR(notificationsLogPostDto.Select(p => p.UserId).ToHashSet(), "en");

            return 1;
        }

        public async Task<int> UpdateNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto)
        {

            var notificationsIds = notificationsLogPostDto.Select(x => x.Id).ToList();
            var originalNotify = await _DbContext.NotificationLogs.Where(x => notificationsIds.Contains(x.Id)).ToListAsync();

            List<NotificationLog> notifyLog = new ();
            foreach (var notify in notificationsLogPostDto)
            {
                var updatedNotify = originalNotify.Where(x => x.Id == notify.Id).FirstOrDefault();
                if (updatedNotify != null)
                {
                    updatedNotify.IsSent = notify.IsSent;
                    updatedNotify.SendReportId = notify.SendReportId;
                    updatedNotify.Hostsetting = notify.HostSetting;
                    updatedNotify.SentCount = notify.SentCount;
                    updatedNotify.LastUpdatedDate = DateTime.Now;
                    updatedNotify.ToAddress = notify.ToAddress;
                  //  updatedNotify. =  notify.ApplicationId;
                  // updatedNotify.PnsApplicationId = notify.PNSApplicationId;
                    updatedNotify.Lang = notify.Lang;
                    updatedNotify.NotificationBody = notify.NotificationBody;
                    updatedNotify.NotificationTitle = notify.NotificationTitle;
                }
                notifyLog.Add(updatedNotify);
            }

            _DbContext.NotificationLogs.UpdateRange(notifyLog);
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<List<NotificationLogGetDto>> GetInternalNotificationsLog(int userId, string lang)
        {
            List<NotificationLogGetDto> res = new List<NotificationLogGetDto>();

            int internalChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            return await (from noty in _DbContext.NotificationLogs.Where(x => x.ToAddress.Trim() == userId.ToString() && x.NotificationChannelId == internalChannel)
                          select new NotificationLogGetDto
                          {
                              Id = noty.Id,
                              //ApplicationId = noty.ApplicationId ?? noty.PnsApplicationId,
                              PNSApplication = noty.ApplicationId != null,
                              NotificationTitle = noty.NotificationTitle,
                              IsSent = noty.IsSent
                          }).ToListAsync();
        }

        public async Task<bool> ReadInternalNotificationsLog(int userId, int notifyId)
        {
            var internalNotifications = await _DbContext.NotificationLogs.Where(x => x.Id == notifyId && x.ToAddress == userId.ToString()).FirstOrDefaultAsync();
            if (internalNotifications != null)
            {
                internalNotifications.IsSent = 1;
                internalNotifications.LastUpdatedDate = DateTime.Now;
                _DbContext.NotificationLogs.Update(internalNotifications);
                await _DbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<int> UpdateInternalNotificationsLogState(int notificationID)
        {
            // TODO Validate if Internal notification.
            var internalNotifications = await _DbContext.NotificationLogs.Where(x => x.Id == notificationID).FirstOrDefaultAsync();

            if (internalNotifications != null)
            {
                internalNotifications.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                internalNotifications.LastUpdatedDate = DateTime.Now;
            }

            _DbContext.NotificationLogs.Update(internalNotifications);
            return await _DbContext.SaveChangesAsync();
        }


        



        //    return new ListCount
        //    {
        //        Count = total,
        //        Items = query.Skip((userFilterDto.pageIndex - 1) * userFilterDto.pageSize).Take(userFilterDto.pageSize)
        //    };


    public async Task<ListCount> GetNotificationsLog(NotificationFilterDto notificationFilterDto, int? userId = null, string lang="ar", int pageIndex = 1, int pageSize = 25)
        {

            bool twoArePassed = notificationFilterDto != null ? 
                notificationFilterDto.name != null 
                && notificationFilterDto.email != null 
                && notificationFilterDto.PhoneNumber != null
                : false;

            int internalChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int mailChannel = await _DbContext.SysLookupValues.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();


            APIResult result = new APIResult();

            var query = await _DbContext.NotificationLogs.OrderByDescending(w => w.CreatedDate).Where(x => x.Lang.Equals(lang)&&(userId == null || x.UserId == userId) && (x.NotificationChannelId == internalChannel || x.NotificationChannelId == mailChannel)).Select(noty => new NotificationLogGetDto
                        {
                            Id = noty.Id,
                            NotificationTitle = noty.NotificationTitle,
                            NotificationBody = noty.NotificationBody,
                            IsSent = noty.IsSent,
                            NotificationLink = noty.NotificationLink,
                            Template = noty.Template,
                            LinkCaption = noty.LinkCaption,
                            CreatedAt = noty.CreatedDate,
                            UserId = noty.UserId,
                            ToAddress = noty.ToAddress,
                            RecipientName = noty.UserId != null ? _DbContext.Users.Where(n => n.Id == noty.UserId).FirstOrDefault().FullName : null,
                            RecipientEmail = noty.UserId != null ? _DbContext.Users.Where(n => n.Id == noty.UserId).FirstOrDefault().Email : null,
            }).Where(u =>

            (notificationFilterDto.text == null

            && notificationFilterDto.email == null

            && notificationFilterDto.name == null 
            
            && notificationFilterDto.PhoneNumber == null) ? u.NotificationTitle.Contains("") :


            (twoArePassed ?

            (u.RecipientEmail.ToLower().Contains(notificationFilterDto.email.ToLower())

            && u.RecipientName.ToLower().Contains(notificationFilterDto.name.ToLower())
            
            && u.ToAddress.ToLower().Contains(notificationFilterDto.PhoneNumber.ToLower()))

            :

           notificationFilterDto.email != null ? u.RecipientEmail.ToLower().Contains(notificationFilterDto.email.ToLower())

            :

            notificationFilterDto.name != null ? u.RecipientName.ToLower().Contains(notificationFilterDto.name.ToLower())

            :

            notificationFilterDto.PhoneNumber != null ? u.ToAddress.ToLower().Contains(notificationFilterDto.PhoneNumber.ToLower())

            :

            notificationFilterDto.text != null && notificationFilterDto.email == null ?

            (u.RecipientEmail.ToLower().Contains(notificationFilterDto.text.ToLower()) ||

            u.RecipientName.ToLower().Contains(notificationFilterDto.text.ToLower()) ||

            u.NotificationTitle.ToLower().Contains(notificationFilterDto.text.ToLower()) ||


            u.NotificationBody.ToLower().Contains(notificationFilterDto.text.ToLower()))
            :
            u.NotificationTitle.Contains(""))).AsNoTracking().ToListAsync();

            return new ListCount
            {
                Count = query.Count(),
                Items = query.Skip((notificationFilterDto.pageIndex - 1) * notificationFilterDto.pageSize).Take(notificationFilterDto.pageSize)
            };
        }

        public async Task<APIResult> SendSignalRNotification(NotificationDirectCallGetDto notificationLogPostDto, int v, byte? status)
        {
            // TODO validation for notification.

            APIResult result = new APIResult();

            try
            {

                List<NotificationLogPostDto> notificationLogs = new List<NotificationLogPostDto>();


                Dictionary<string, List<string>> keyVal = new Dictionary<string, List<string>>();

                keyVal.Add("en", new List<string> { notificationLogPostDto.SenderName, "Call recieved"});
                keyVal.Add("ar", new List<string> { notificationLogPostDto.SenderName, "استلام اتصال جديد" });




                for (int i = 0; i < keyVal.Count(); i++)
                {
                    NotificationLogPostDto notificationLogPost = new NotificationLogPostDto
                    {

                        NotificationTitle = keyVal.ElementAt(i).Value[0],
                        NotificationBody = keyVal.ElementAt(i).Value[1],
                        NotificationChannelId = 42,
                        UserId = v,
                        ToAddress = notificationLogPostDto.RecieverId,
                        Lang = keyVal.ElementAt(i).Key,
                        CreatedDate = DateTime.Now,
                        NotificationLink = notificationLogPostDto.NotificationBody != null ? notificationLogPostDto.NotificationBody : "No Link",
                        RecStatus = status,
                        CreatedBy = notificationLogPostDto.SenderId
                    };


                    notificationLogs.Add(notificationLogPost);
                }
                

               await AddNotificationsLog(notificationLogs);

                //await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Created", true, APIResult.RESPONSE_CODE.OK);

            } 
            
            catch
            {

                return result.FailMe(-1, "Failed to add");
            }
            
        }


        public async Task<ListCount> GetNotificationsLogSignalR(string recieverId, string lang = "ar", int pageIndex = 1, int pageSize = 25)
        {
            var allUserNotifications = await _DbContext.NotificationLogs.Where(x => x.ToAddress.Equals(recieverId)).
                Skip((pageIndex - 1) * pageSize).
                Take(pageSize).ToListAsync();


            return new ListCount
            {
                Count = allUserNotifications.Count(),
                Items = allUserNotifications
            };

        }

        // For participants
        // Otp notification
        // Registeration
        public async Task UserNotifySignalR(HashSet<int?> userIds, string lang)
        {
            var scope = _services.CreateScope();

            var notificationHub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            NotificationFilterDto notificationFilterDto = new NotificationFilterDto();

            notificationFilterDto.pageIndex = 1;
            notificationFilterDto.pageSize = 1;

            if (UserHandler.ConnectedIds.Count() > 0)
            {
                foreach (var connectedId in UserHandler.ConnectedIds)
                {
                    if (userIds.Contains(Int32.Parse(connectedId)))
                    {
                        var notifications = await GetNotificationsLog(notificationFilterDto, Int32.Parse(connectedId), lang, 1, 1);

                        await notificationHub.Clients.User(connectedId).SendAsync("notify", notifications);
                    }
                }
            }
        }
    }
}
