using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Repositories.INotificationRepository;

namespace VideoProjectCore6.Services.NotificationService
{
    public class ControlNotification
    {
        public List<INotificationObserver> Notifications = new List<INotificationObserver>();

        public List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
        /// <summary>  
        /// Add object of notification System  
        /// </summary>  
        /// <param name="obj">Object is notification class</param>  
        public void AddService(INotificationObserver obj)
        {
            Notifications.Add(obj);
        }

        /// <summary>  
        /// Remove object of notification System  
        /// </summary>  
        /// <param name="obj">Object of notification Class</param>  
        public void RemoveService(INotificationObserver obj)
        {
            Notifications.Remove(obj);
        }
        public List<NotificationLogPostDto> ExecuteNotifier(bool sendImmediately, string key)
        {
            foreach (INotificationObserver O in Notifications)
            {
                res.AddRange(O.Notify(sendImmediately, key).Result);
            }
            return res;
        }
    }
}
