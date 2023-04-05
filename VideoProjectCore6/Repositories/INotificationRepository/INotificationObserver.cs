using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Repositories.INotificationRepository
{
    public interface INotificationObserver
    {
        Task<List<NotificationLogPostDto>> Notify(bool sendImmediately, string key);
    }
}
