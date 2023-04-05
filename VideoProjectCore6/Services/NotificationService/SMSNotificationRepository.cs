using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.INotificationRepository;
#nullable disable
namespace VideoProjectCore6.Services.NotificationService
{
    class SMSNotification : INotificationObserver
    {
        private readonly List<NotificationLogPostDto> _notifications;
        //private readonly ChannelSMSSetting _sMSSettings;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IConfiguration _IConfiguration;
        private string _connectionError;
        private string _SmsUrl;

        public SMSNotification(List<NotificationLogPostDto> notificationsLogPostDto, IGeneralRepository iGeneralRepository/*, IOptions<ChannelSMSSetting> smsSetting,*/, IConfiguration iConfiguration)
        {
            // _sMSSettings = smsSetting.Value;
            _notifications = new List<NotificationLogPostDto>();
            _notifications = notificationsLogPostDto;
            _IGeneralRepository = iGeneralRepository;
            _IConfiguration = iConfiguration;
            _connectionError = string.Empty;
            _SmsUrl = string.Empty;
        }

        public async Task<List<NotificationLogPostDto>> Notify_sms_by_lang(bool sendImmediately, string key)
        {
            var client = GetConnection();
            if (client == null)
            {
                foreach (var notify in _notifications)
                {
                    notify.HostSetting = string.Empty;
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                    notify.UpdatedDate = DateTime.Now;
                    notify.SentCount += 1;
                    notify.SendReportId = _connectionError;
                }
                return _notifications;
            }

            foreach (var notify in _notifications)
            {
                if (sendImmediately)
                {
                    try
                    {
                        var tempReportId = _IGeneralRepository.GetNewValueBySec().ToString();
                        var link = !string.IsNullOrWhiteSpace(notify.NotificationLink) ? " " + notify.NotificationLink : string.Empty;
                        SMSDto sms = new SMSDto
                        {
                            pLanguage = notify.Lang.ToLower() == "ar" ? "AR1" : "ENG",
                            pMessage_TEXT = notify.NotificationBody + link,
                            pMobileNO = notify.ToAddress
                        };
                        var smsSend = await InvokeSMSService(client, sms);
                        if (smsSend.Id > 0)
                        {
                            notify.ReportValueId = smsSend.Code.ToString();
                            notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.SENT;
                            notify.SentCount += 1;
                            notify.UpdatedDate = DateTime.Now;
                            notify.SendReportId += smsSend.Message[0] + tempReportId;
                        }
                        else
                        {
                            notify.ReportValueId = smsSend.Code.ToString();
                            notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.ERROR;
                            notify.SentCount += 1;
                            notify.UpdatedDate = DateTime.Now;
                            notify.SendReportId += string.Format(" Error at attempt {0}, : {1} ", notify.SentCount, smsSend.Message[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        string mes = "";
                        notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.SentCount += 1;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SendReportId += string.Format(" Error at attempt {0}, : {1} ", notify.SentCount, ex.Message/*, mes*/);
                    }
                }
                else
                {
                    notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.PENDING;
                    notify.SentCount = 0;
                }
            }
            client.Dispose();
            return _notifications;
        }
        public async Task<List<NotificationLogPostDto>> Notify(bool sendImmediately, string key)
        {
            var client = GetConnection();
            if (client == null)
            {
                foreach (var notify in _notifications)
                {
                    notify.HostSetting = string.Empty;
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                    notify.UpdatedDate = DateTime.Now;
                    notify.SentCount += 1;
                    notify.SendReportId = _connectionError;
                }
                return _notifications;
            }
           
            var multiLangNotifications = new List<MultiLangPostNotification>();
            var langCount = _notifications.Select(x => x.Lang).Distinct().Count();
            //string hostSetting = "Host: " + _mailSettings.Host + "Sender: " + _mailSettings.Mail + "Port: " + _mailSettings.Port;
            for (int i = 0; i <= _notifications.Count() - langCount; i += langCount)
            {
                multiLangNotifications.Add(_notifications[i].ToMultiLangue(
                       _notifications[i + 1].NotificationBody,
                       _notifications[i].Template,
                       _notifications[i].NotificationTitle + " | " + _notifications[i + 1].NotificationTitle,
                       _notifications[i + 1].LinkCaption + " | " + _notifications[i].LinkCaption));
            }
            var ptr = 0;

            foreach (var notify in multiLangNotifications)
            {
                if (sendImmediately)
                {
                    try
                    {
                        var tempReportId = _IGeneralRepository.GetNewValueBySec().ToString();
                        var link = !string.IsNullOrWhiteSpace(notify.NotificationLink) ? " " + notify.NotificationLink : string.Empty;
                        SMSDto sms = new SMSDto
                        {
                            pLanguage = "AR1",// notify.Lang.ToLower() == "ar" ? "AR1" : "ENG",
                            pMessage_TEXT = notify.NotificationBody + " | " + notify.NotificationBodyL+ ":" + link,
                            pMobileNO = notify.ToAddress
                        };
                        var smsSend = await InvokeSMSService(client, sms);
                        if (smsSend.Id > 0)
                        {
                           for (int i = 0; i < langCount; i++)
                            {
                                _notifications[ptr + i].ReportValueId = smsSend.Code.ToString();
                                _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                                _notifications[ptr + i].UpdatedDate = DateTime.Now;
                                _notifications[ptr + i].SentCount += 1;
                                _notifications[ptr + i].SendReportId = smsSend.Message[0];                               
                            }
                        }
                        else
                        {
                            for (int i = 0; i < langCount; i++)
                            {
                                _notifications[ptr + i].ReportValueId = smsSend.Code.ToString();
                                _notifications[ptr + i].IsSent = (byte?)Constants.NOTIFICATION_STATUS.ERROR;
                                _notifications[ptr + i].UpdatedDate = DateTime.Now;
                                _notifications[ptr + i].SentCount += 1;
                                _notifications[ptr + i].SendReportId = string.Format("Error at attempt {0}, : {1} ", notify.SentCount, smsSend.Message[0]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.SentCount += 1;
                        notify.SendReportId = string.Format("Error at attempt {0}, : {1} ", notify.SentCount, ex.Message);
                    }
                }
                else
                {
                    notify.IsSent = (byte?)Constants.NOTIFICATION_STATUS.PENDING;
                    notify.SentCount = 0;
                }
                ptr = ptr + langCount;
            }
            client.Dispose();
            return _notifications;
        }
        private HttpClient GetConnection()
        {            
            _SmsUrl = _IConfiguration["SMSEndPoint"];
            if (_SmsUrl == null)
            {
                _connectionError = "SMS endpoint not provided ";
                return null;
            }
            else
            {
                return new HttpClient();
            }
        }
        private async Task<APIResult> InvokeSMSService(HttpClient client, SMSDto sms)
        {
            APIResult res = new APIResult();
            if (_SmsUrl == null)
            {
                return res.FailMe(-1, "SMS endpoint url not provided ");
            }
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(sms);
                if (json == string.Empty)
                {
                    return res.FailMe(-1, "Empty object");
                }
            }
            catch (Exception ex)
            {
                return res.FailMe(-1, ex.Message);
            }
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage ;
            try
            {
                httpResponseMessage = await client.PostAsync(_SmsUrl, data);
                if (httpResponseMessage == null)
                {
                    return res.FailMe(-1, "Connection error");
                }
            }
            catch (HttpRequestException ex)
            {
                return res.FailMe(-1, ex.Message);
            }
            catch (Exception ex)
            {
                return res.FailMe(-1, ex.Message);
            }

            string result = await httpResponseMessage.Content.ReadAsStringAsync();
            SMSResult s = JsonConvert.DeserializeObject<SMSResult>(result);

            res.Id = s.pCode == 200 ? 1 : -1;
            res.Code = s.pCode == 200 ? APIResult.RESPONSE_CODE.OK : APIResult.RESPONSE_CODE.BadRequest;
            res.Message.Add(s.pStatus);
            return res;
        }
    }
}
