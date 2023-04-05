using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Repositories.INotificationRepository;
using MailKit.Security;
using System.Text;
using MimeKit.Utils;
using VideoProjectCore6.Models;
using Microsoft.EntityFrameworkCore;
#nullable disable
namespace VideoProjectCore6.Services.NotificationService
{
    public class MailNotification : INotificationObserver
    {
        private readonly ChannelMailFirstSetting _mailSettings;
        private readonly List<NotificationLogPostDto> _notifications;
        private string _connectionError;
        private readonly OraDbContext _context;

        public MailNotification(List<NotificationLogPostDto> notificationsLogPostDto, IOptions<ChannelMailFirstSetting> mailSettings, OraDbContext context)
        {
            _mailSettings = mailSettings.Value;
            _notifications = new List<NotificationLogPostDto>();
            _notifications = notificationsLogPostDto;
            _connectionError = string.Empty;
            _context = context;
        }

        public async Task<List<NotificationLogPostDto>> Notify_o(bool sendImmediately, string key)
        {
            // List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            var multiLangNotifications = new List<MultiLangPostNotification>();
            var langCount = _notifications.Select(x => x.Lang).Distinct().Count();

            for (int i = 0; i <= _notifications/*.OrderBy(x => x.UserId)*/.Count() - langCount; i += langCount)
            {
                multiLangNotifications.Add(_notifications[i].ToMultiLangue(
                       _notifications[i + 1].NotificationBody,
                       _notifications[i].Template,
                       _notifications[i].NotificationTitle + " | " + _notifications[i + 1].NotificationTitle,
                       _notifications[i + 1].LinkCaption + " | " + _notifications[i].LinkCaption));
            }

            var ptr = 0;

            foreach (var notify in multiLangNotifications)/*_notifications*/
            {
                using (var client = new SmtpClient())
                    try
                    {
                        if (sendImmediately)
                        {
                            //notify.HostSetting = "Host: " + _mailSettings.Host + "Sender: " + _mailSettings.Mail + "Port: " + _mailSettings.Port;
                            client.CheckCertificateRevocation = false;
                            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);//SecureSocketOptions.StartTls

                            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication) && _mailSettings.Password.Equals(""))
                            {
                                client.Authenticate(_mailSettings.Mail, null);
                            }

                            else if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                            {
                                client.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                            }

                            var email = new MimeMessage
                            {
                                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                                Subject = notify.NotificationTitle
                            };
                            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                            email.To.Add(MailboxAddress.Parse(notify.ToAddress));
                            var strBuilder = new StringBuilder();
                            var builder = new BodyBuilder();

                            try
                            {
                                // try to send by template HTML.                            

                                using (var reader = File.OpenText(Path.Combine("wwwroot", "templates", notify.Template)))
                                {
                                    strBuilder.Append(reader.ReadToEnd());
                                    string newText = notify.NotificationBody.Replace(",", "<br>");
                                    string newTextL = notify.NotificationBodyL.Replace(",", "<br>");
                                    strBuilder = strBuilder.Replace("{BODY-AR}", newText);
                                    strBuilder = strBuilder.Replace("{BODY-EN}", newTextL);
                                    strBuilder = strBuilder.Replace("{LINKCAPTION}", notify.LinkCaption);
                                    strBuilder = strBuilder.Replace("{LINKURL}", notify.NotificationLink);
                                }
                                //// try to send by template HTML.
                                //using (var reader = File.OpenText(@"wwwroot\templates\Template.html"))
                                //{
                                //    strBuilder.Append(reader.ReadToEnd());
                                //    strBuilder = strBuilder.Replace("{body}", notify.NotificationBody);
                                //}

                                var imageLogo = builder.LinkedResources.Add(@"wwwroot\Templates\images\logo.png");
                                imageLogo.ContentId = MimeUtils.GenerateMessageId();
                                strBuilder = strBuilder.Replace("{logo}", imageLogo.ContentId);

                                /* var footerIcon = builder.LinkedResources.Add(@"wwwroot\Template_Email\logo\footer.png");
                                 footerIcon.ContentId = MimeUtils.GenerateMessageId();
                                 atringBuilder = atringBuilder.Replace("{footer}", footerIcon.ContentId);*/

                            }
                            catch (Exception ex)
                            {
                                strBuilder.Append(notify.NotificationBody + " " + "<a href =\'" + notify.NotificationLink + "' > Activate account</a>");
                                Console.WriteLine(ex.Message);
                            }

                            builder.HtmlBody = strBuilder.ToString();
                            email.Body = builder.ToMessageBody();
                            client.Send(email);
                            client.Disconnect(true);

                            //----------------------------------update Original notification---------------
                            //notify.ReportValueId = email.MessageId;
                            //notify.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                            //notify.UpdatedDate = DateTime.Now;
                            //notify.SentCount += 1;
                            //string autoMessageId = Guid.NewGuid().ToString();
                            //notify.SendReportId = autoMessageId;

                            for (int i = 0; i < langCount; i++)
                            {
                                _notifications[ptr + i].ReportValueId = email.MessageId;
                                _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                                _notifications[ptr + i].UpdatedDate = DateTime.Now;
                                _notifications[ptr + i].SentCount += 1;
                                _notifications[ptr + i].SendReportId = Guid.NewGuid().ToString();
                                _notifications[ptr + i].HostSetting = "Host: " + _mailSettings.Host + "Sender: " + _mailSettings.Mail + "Port: " + _mailSettings.Port;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < langCount; i++)
                            {
                                _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                                _notifications[ptr + i].SentCount = 0;
                            }
                        }
                    }

                    catch (ArgumentNullException ex)
                    {
                        //notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        //notify.UpdatedDate = DateTime.Now;
                        //notify.SentCount += 1;
                        //notify.SendReportId += string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        //client.Disconnect(true);
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message); ;
                        }
                        client.Disconnect(true);
                    }

                    catch (ArgumentOutOfRangeException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" port is not between 0 and 65535 error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (ArgumentException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" The host is a zero-length string error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (ObjectDisposedException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" MailKit.Net.Smtp.SmtpClient has been disposed error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (InvalidOperationException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" The MailKit.Net.Smtp.SmtpClient is already connected or authenticated error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (NotSupportedException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (OperationCanceledException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" The operation was canceled error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (System.Net.Sockets.SocketException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" A socket error occurred trying to connect to the remote host error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (MailKit.Security.SslHandshakeException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format("  An error occurred during the SSL/TLS negotiations. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (System.IO.IOException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format("An I/O error occurred at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (SmtpCommandException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" An SMTP command failed. Error at attempt {0}, trying to connect: {1}", notify.SentCount, ex.Message);

                        }
                        client.Disconnect(true);
                    }

                    catch (SmtpProtocolException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" An SMTP protocol error occurred error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (AuthenticationException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format("  Authentication using the supplied credentials has failed. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                    catch (Exception ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR; ;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" Inner error at attempt {0}, is {1}", notify.SentCount, ex.Message);
                        }
                        client.Disconnect(true);
                    }

                //res.Add(notify);
                ptr = ptr + langCount;
            }
            return _notifications;
        }

        public async Task<List<NotificationLogPostDto>> Notify_(bool sendImmediately)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            var multiLangNotifications = new List<MultiLangPostNotification>();
            var langCount = _notifications.Select(x => x.Lang).Distinct().Count();



            for (int i = 0; i <= _notifications.OrderBy(x => x.UserId).Count() - langCount; i += langCount)
            {
                multiLangNotifications.Add(_notifications[i].ToMultiLangue(
                       _notifications[i + 1].NotificationBody,
                       _notifications[i].Template,
                       _notifications[i].NotificationTitle + " | " + _notifications[i + 1].NotificationTitle,
                       _notifications[i + 1].LinkCaption + " | " + _notifications[i].LinkCaption));
            }

            var ptr = 0;

            string hostSetting = "Host: " + _mailSettings.Host + "Sender: " + _mailSettings.Mail + "Port: " + _mailSettings.Port;

            if (sendImmediately)
            {
                var sender = MailboxAddress.Parse(_mailSettings.Mail);
                var client = await GetConnection();
                if (!client.IsConnected)
                {
                    foreach (var notify in _notifications)
                    {
                        notify.HostSetting = hostSetting;
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId = _connectionError;
                        res.Add(notify);
                    }
                    return res;
                }
                foreach (var notify in _notifications)
                {
                    try
                    {
                        notify.HostSetting = hostSetting;
                        var email = new MimeMessage
                        {
                            Sender = sender,
                            Subject = notify.NotificationTitle
                        };
                        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                        email.To.Add(MailboxAddress.Parse(notify.ToAddress));
                        var strBuilder = new StringBuilder();
                        var builder = new BodyBuilder();
                        try
                        {
                            var template = notify.Template != null && notify.Template.EndsWith(".html") ? notify.Template : "invitation.template";
                            using (var reader = File.OpenText(Path.Combine("wwwroot", "templates", notify.Template)))
                            {
                                strBuilder.Append(reader.ReadToEnd());
                                string newText = notify.NotificationBody.Replace(",", "<br>");
                                strBuilder = strBuilder.Replace("{MAIL}", newText);
                                strBuilder = strBuilder.Replace("{LINKCAPTION}", notify.LinkCaption);
                                strBuilder = strBuilder.Replace("{LINKURL}", notify.NotificationLink);
                            }
                        }
                        catch (Exception ex)
                        {
                            strBuilder.Append(notify.NotificationBody);
                            Console.WriteLine(ex.Message);
                        }
                        builder.HtmlBody = strBuilder.ToString();
                        email.Body = builder.ToMessageBody();
                        await client.SendAsync(email);

                        notify.ReportValueId = email.MessageId;
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        string autoMessageId = Guid.NewGuid().ToString();
                        notify.SendReportId = autoMessageId;
                    }
                    catch (ArgumentNullException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" MailKit.Net.Smtp.SmtpClient has been disposed error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" The MailKit.Net.Smtp.SmtpClient is already connected or authenticated error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (NotSupportedException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (OperationCanceledException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" The operation was canceled error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (System.IO.IOException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format("An I/O error occurred at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (AuthenticationException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format("  Authentication using the supplied credentials has failed. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" Inner error at attempt {0}, is {1}", notify.SentCount, ex.Message);
                    }
                    res.Add(notify);
                }
                client.Disconnect(true);
            }
            else
            {
                foreach (var notify in _notifications)
                {
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                    notify.SentCount = 0;
                    res.Add(notify);
                }
            }
            return res;
        }

        private async Task<SmtpClient> GetConnection()
        {
            var client = new SmtpClient();

            var existingConfig = await _context.SmtpConfigs.FirstOrDefaultAsync();


            try
            {
                client.CheckCertificateRevocation = false;

                if (existingConfig == null)
                {
                    if (_mailSettings.Password.Equals(""))
                    {
                        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.None);
                    }

                    else if (!(_mailSettings.Password.Equals("")))
                    {

                        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port);


                        if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                        {
                            //client.CheckCertificateRevocation = false;
                            //await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port);
                            client.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                        }

                    }
                }
                else
                {

                    if (existingConfig?.Password == null || existingConfig.Password.Equals(""))
                    {
                        await client.ConnectAsync(existingConfig.Host, existingConfig.Port, SecureSocketOptions.None);
                    }

                    else if (!(existingConfig.Password.Equals("")))
                    {

                        await client.ConnectAsync(existingConfig.Host, existingConfig.Port);


                        if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                        {
                            //client.CheckCertificateRevocation = false;
                            //await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port);
                            client.Authenticate(existingConfig.Email, existingConfig.Password);
                        }
                    }
                }

            }
            catch (ArgumentNullException ex)
            {
                _connectionError = string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _connectionError = string.Format(" port is not between 0 and 65535 error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (ArgumentException ex)
            {
                _connectionError = string.Format(" The host is a zero-length string error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (ObjectDisposedException ex)
            {
                _connectionError = string.Format(" MailKit.Net.Smtp.SmtpClient has been disposed error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (InvalidOperationException ex)
            {
                _connectionError = string.Format(" The MailKit.Net.Smtp.SmtpClient is already connected or authenticated error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (NotSupportedException ex)
            {
                _connectionError = string.Format(" options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension. error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (OperationCanceledException ex)
            {
                _connectionError = string.Format(" The operation was canceled error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _connectionError = string.Format(" A socket error occurred trying to connect to the remote host error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (MailKit.Security.SslHandshakeException ex)
            {
                _connectionError += string.Format("  An error occurred during the SSL/TLS negotiations. error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (System.IO.IOException ex)
            {
                _connectionError = string.Format("An I/O error occurred at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (SmtpCommandException ex)
            {
                _connectionError = string.Format(" An SMTP command failed. Error at attempt {0}, trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (SmtpProtocolException ex)
            {
                _connectionError = string.Format(" An SMTP protocol error occurred error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (AuthenticationException ex)
            {
                _connectionError = string.Format("  Authentication using the supplied credentials has failed. error at attempt {0}, while trying to connect: {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                _connectionError += string.Format(" Inner error at attempt {0}, is {1}", 1, ex.Message);
                client.Disconnect(true);
            }
            return client;
        }
        public async Task<List<NotificationLogPostDto>> Notify(bool sendImmediately, string key)
        {
            var multiLangNotifications = new List<MultiLangPostNotification>();
            var langCount = _notifications.Select(x => x.Lang).Distinct().Count();

            var existingConfig = await _context.SmtpConfigs.FirstOrDefaultAsync();

            string hostSetting = "Host: " + existingConfig.Host + "Sender: " + existingConfig.Email + "Port: " + existingConfig.Port;
            for (int i = 0; i <= _notifications.Count() - langCount; i += langCount)
            {
                multiLangNotifications.Add(_notifications[i].ToMultiLangue(
                       _notifications[i + 1].NotificationBody,
                       _notifications[i].Template,
                       _notifications[i].NotificationTitle + " | " + _notifications[i + 1].NotificationTitle,
                       _notifications[i + 1].LinkCaption + " | " + _notifications[i].LinkCaption));
            }

            var ptr = 0;

            if (sendImmediately)
            {
                var sender = MailboxAddress.Parse(existingConfig.Email);
                var client = await GetConnection();
                if (!client.IsConnected)
                {
                    foreach (var notify in _notifications)
                    {
                        notify.HostSetting = hostSetting;
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId = _connectionError;
                        //res.Add(notify);
                    }
                    return _notifications;//res
                }
                foreach (var notify in multiLangNotifications)
                {
                    try
                    {
                        notify.HostSetting = hostSetting;
                        var email = new MimeMessage
                        {
                            Sender = sender,
                            Subject = notify.NotificationTitle
                        };
                        email.From.Add(new MailboxAddress(existingConfig.DisplayName, existingConfig.Email));
                        email.To.Add(MailboxAddress.Parse(notify.ToAddress));
                        var strBuilder = new StringBuilder();
                        var builder = new BodyBuilder();
                        try
                        {
                            var template = notify.Template != null && (notify.Template.EndsWith(".html") || notify.Template.EndsWith(".htm")) ? notify.Template : "en-invitation.html";
                            using (var reader = File.OpenText(Path.Combine("wwwroot", "templates", notify.Template)))
                            {
                                strBuilder.Append(reader.ReadToEnd());
                                string newText = notify.NotificationBody.Replace(",", "<br>");
                                string newTextL = notify.NotificationBodyL.Replace(",", "<br>");
                                strBuilder = strBuilder.Replace("{BODY-AR}", newText);
                                strBuilder = strBuilder.Replace("{BODY-EN}", newTextL);
                                strBuilder = strBuilder.Replace("{BODY}", newText);
                                strBuilder = strBuilder.Replace("{LINKCAPTION}", notify.LinkCaption);
                                strBuilder = strBuilder.Replace("{LINKURL}", notify.NotificationLink);
                            }
                            try
                            {
                                //var imageLogo = builder.LinkedResources.Add(@"wwwroot\Templates\images\logo.png");
                                //imageLogo.ContentId = MimeUtils.GenerateMessageId();
                                //strBuilder = strBuilder.Replace("{logo}", imageLogo.ContentId);
                            }
                            catch
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            strBuilder.Append(notify.NotificationBody);
                            strBuilder.Append(" <br> ");
                            strBuilder.Append(notify.NotificationBodyL);
                            strBuilder.Append(" <br> ");
                            strBuilder.Append(notify.NotificationBodyL);
                            strBuilder.Append(" <br> ");
                            strBuilder.Append($"<a href='{notify.NotificationLink} '>{notify.LinkCaption}</a>");
                        }
                        builder.HtmlBody = strBuilder.ToString();
                        email.Body = builder.ToMessageBody();
                        await client.SendAsync(email);

                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].ReportValueId = email.MessageId;
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId = Guid.NewGuid().ToString();
                            _notifications[ptr + i].HostSetting = "Host: " + existingConfig.Host + "Sender: " + existingConfig.Email + "Port: " + existingConfig.Port;
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message); ;
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" MailKit.Net.Smtp.SmtpClient has been disposed error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {

                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" The MailKit.Net.Smtp.SmtpClient is already connected or authenticated error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (NotSupportedException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" The operation was canceled error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (System.IO.IOException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format("An I/O error occurred at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (AuthenticationException ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format("Authentication using the supplied credentials has failed. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        for (int i = 0; i < langCount; i++)
                        {
                            _notifications[ptr + i].IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                            _notifications[ptr + i].UpdatedDate = DateTime.Now;
                            _notifications[ptr + i].SentCount += 1;
                            _notifications[ptr + i].SendReportId += string.Format(" Inner error at attempt {0}, is {1}", notify.SentCount, ex.Message);
                        }
                    }
                    // res.Add(notify);
                    ptr = ptr + langCount;
                }
                client.Disconnect(true);
            }
            else
            {
                foreach (var notify in _notifications)
                {
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                    notify.SentCount = 0;
                    //res.Add(notify);
                }
            }

            return _notifications;
        }
    }
}
