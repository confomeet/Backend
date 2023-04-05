using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.INotificationRepository;

namespace VideoProjectCore6.Services.NotificationService
{
    public class InternalNotification : INotificationObserver
    {
        private readonly List<NotificationLogPostDto> _notificationsLogPostDto;
        private readonly OraDbContext _DBContext;

        public InternalNotification(List<NotificationLogPostDto> notificationsLogPostDto, OraDbContext DBContext)
        {
            _notificationsLogPostDto = new List<NotificationLogPostDto>();
            _notificationsLogPostDto = notificationsLogPostDto;
            _DBContext = DBContext;
        }

        public async Task<List<NotificationLogPostDto>> Notify(bool notUsed, string key)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            foreach (var notify in _notificationsLogPostDto)
            {
                var userDetails = await _DBContext.Users.Where(x => x.Id == notify.UserId).FirstOrDefaultAsync();
                if (userDetails != null)
                {
                    notify.HostSetting = "Internal Notification";
                    notify.SentCount = 1;
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                    res.Add(notify);
                }
            }
            return res;
        }
    }
}
