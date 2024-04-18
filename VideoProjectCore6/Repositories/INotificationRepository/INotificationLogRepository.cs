using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories.INotificationRepository
{
    public interface INotificationLogRepository
    {
                Task<int> AddNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto);
                Task<int> UpdateNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto);
                Task<List<NotificationLogGetDto>> GetInternalNotificationsLog(int userId, string lang);
                Task<bool> ReadInternalNotificationsLog(int userId, int notifyId);
                Task<int> UpdateInternalNotificationsLogState(int notificationID);
                Task<ListCount> GetNotificationsLog(NotificationFilterDto notificationFilterDto, int? userId = null, string lang = "en", int pageIndex = 1, int pageSize = 25);
    }
}
