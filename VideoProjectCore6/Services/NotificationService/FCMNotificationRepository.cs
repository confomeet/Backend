using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Services.Meeting;
#nullable disable
namespace VideoProjectCore6.Services.NotificationService
{
    public class FCMNotification : INotificationObserver
    {
        private readonly List<NotificationLogPostDto> _notificationsLogPostDto;
        private string _connectionError;
        private readonly IGeneralRepository _IGeneralRepository;

        public FCMNotification(List<NotificationLogPostDto> notificationsLogPostDto,IGeneralRepository iGeneralRepository)
        {
            _notificationsLogPostDto = notificationsLogPostDto;
            _connectionError = string.Empty;
            _IGeneralRepository = iGeneralRepository;
        }


        public async Task<List<NotificationLogPostDto>> Notify(bool sendImmediately, string key)
        {



            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();

            if (sendImmediately)
            {
                var fbm = getConnection(key);


                if (fbm == null)
                {
                    foreach (var notify in _notificationsLogPostDto)
                    {
                        notify.HostSetting = "FCM";
                        notify.IsSent = (int) Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId = _connectionError;
                        res.Add(notify);
                    }

                    return res;
                }

                foreach (var notify in _notificationsLogPostDto)
                {
                    try
                    {
                       // string json = @"{""title"":""Meeting request"",""body"":""Please check your email. You have recieved an invitation to join a personal meeting!""}";

                        var messageBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(notify.NotificationBody);
                         messageBody.Add("title", notify.NotificationTitle);



                        //int? sender = Int32.Parse(messageBody["SenderId"]);

                        //var tokens = await _IGeneralRepository.GetContactToken((int) sender);



                        //if (sender != null && notify.UserId != sender && !tokens.Result.Contains(notify.ToAddress))
                        //{
                            var message = new Message()
                            {
                                Data = messageBody,
                                Token = notify.ToAddress
                            };

                            string response = await fbm.SendAsync(message);
                            notify.ReportValueId = response;
                            notify.IsSent = (int) Constants.NOTIFICATION_STATUS.SENT;
                            notify.UpdatedDate = DateTime.Now;
                            notify.SentCount += 1;
                            notify.SendReportId = Guid.NewGuid().ToString();
                        //} 

                        //else if (sender == null)
                        //{
                        //    var message = new Message()
                        //    {
                        //        Data = messageBody,
                        //        Token = notify.ToAddress
                        //    };

                        //    string response = await fbm.SendAsync(message);
                        //    notify.ReportValueId = response;
                        //    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                        //    notify.UpdatedDate = DateTime.Now;
                        //    notify.SentCount += 1;
                        //    notify.SendReportId = Guid.NewGuid().ToString();

                        //}
                        
                    }

                    catch (Exception ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += ex.Message;

                    }

                    res.Add(notify);
                }
            }
            else
            {
                foreach (var notify in _notificationsLogPostDto)
                {
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                    notify.SentCount = 0;
                    res.Add(notify);
                }
            }
            return res;
        }

        private FirebaseMessaging getConnection(string key)
        {

            FirebaseMessaging firebaseMessagingInstance;
            FirebaseApp defaultApp;
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    defaultApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(Path.Combine("wwwroot", key)),
                    });
                }

                else
                {
                    defaultApp = FirebaseApp.DefaultInstance;
                 
                }
                firebaseMessagingInstance = FirebaseMessaging.GetMessaging(defaultApp);
            }
            catch (Exception e)
            {
                _connectionError = e.Message;
                return null;
            }
            return firebaseMessagingInstance;
        }

    }
}
