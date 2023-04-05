using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Repositories.INotificationRepository
{
    public interface ISendNotificationRepository
    {
        /// <summary>
                /// Send the notifications by the observer.
                /// </summary>
                /// <param name="notifications">list of notifications to send</param>
                /// <param name="addOrUpdateNotificationslog">true to add the notifications to the log which are not send before, false if existed before to update</param>
                /// <returns></returns>
                Task DoSend(List<NotificationLogPostDto> notifications, bool sendImmediately,  bool addOrUpdateNotificationslog = true, string key="key.json");

        /// <summary>
        /// Re send the failed notification (it is status is error),
        /// attempt every notification to resend until MAX_NOTIFY_SEND_ATTEMPTS
        /// </summary>
        /// <returns></returns>
        Task ReSend(string channelName, int size);
        Task<string> GenerateUrlToken(int userId, string meetingId, string lang);
        Task<APIResult> VerifyOTP(int userId, string number, string lang);
        Task<bool> SendOTP(int userId, string mobile, string email, int eventId, string lang);
        Task<UserAppDto> VerifyToken(Guid guid, string lang);
        Task SendMailToAdmin(int userId, string extraText);

        // Task<List<NotificationLogPostDto>> GetScheduleNotifications(List<NotificationLogPostDto> responseNotification, List<Receiver> receivers, string meetingId, string lang);
        Task<List<NotificationLogPostDto>> BuildNotifications(List<NotificationLogPostDto> notifications, List<Receiver> receivers, List<string> notyBody,bool addPublicLink=true);
        Task<List<MeetingUserLink>> FillAndSendNotification(List<NotificationLogPostDto> notifications, List<Receiver> receivers, Dictionary<string, string> Parameters, string meetingId, bool addAppLink, string lang, bool send, bool isDirectInvitation,string cisco=null);
        Task<APIResult> InvokeSMSService(SMSDto sms);

        Task<APIResult> SendOTPCode(int userId, string mobile, string email, string lang);
    }
}
