using SignalRSwaggerGen.Attributes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using VideoProjectCore6.Utility;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Models;
using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Hubs
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SignalRHub("/notifications")]
    public class NotificationHub : Hub
    {
        private readonly INotificationLogRepository _INotificationLogRepository;
        private readonly OraDbContext _DbContext;

        public NotificationHub(INotificationLogRepository iNotificationLogRepository, OraDbContext dbContext)
        {
            _INotificationLogRepository = iNotificationLogRepository;
            _DbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.UserIdentifier == null)
                return;
            UserHandler.ConnectedIds.Add(Context.UserIdentifier);
            await Clients.All.SendAsync("UserConnected", UserHandler.ConnectedIds);
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier == null)
                return;
            UserHandler.ConnectedIds.Remove(Context.UserIdentifier);

            await Clients.All.SendAsync("UserDisconnected", UserHandler.ConnectedIds);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateNotification(NotificationFilterDto notificationFilterDto, int pageIndex = 1, int pageSize = 25, string lang = "ar")
        {
            var currentUserId = Context.UserIdentifier;
            if (currentUserId == null)
                return;

            //var entityId = await _DbContext.EmployeeSetting.Include(l => l.User).Include(q => q.InterEntity)
            //        .Where(o => o.UserId == Int32.Parse(currentUserId)).Select(q => q.InterEntity).FirstOrDefaultAsync();

            var myNotifications = await _INotificationLogRepository.GetNotificationsLog(notificationFilterDto, Int32.Parse(currentUserId), lang, pageIndex, pageSize);


            await Clients.User(currentUserId).SendAsync("notify", myNotifications);
        }
    }

}
