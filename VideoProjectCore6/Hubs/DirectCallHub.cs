using System.Net;
using iText.StyledXmlParser.Jsoup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRSwaggerGen.Attributes;
using VideoProjectCore6.Utilities.ErrorHandling.Exceptions;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Utility;

namespace VideoProjectCore6.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SignalRHub("/directCall")]
    public class DirectCallHub : Hub
    {
        //private readonly IChatRepository _chatRepository;
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly IUserRepository _usersService;
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _generalRepository;
        private readonly IConfiguration _IConfiguration;

        public DirectCallHub(IGeneralRepository generalRepository, IConfiguration iConfiguration, IUserRepository usersService, INotificationLogRepository _notificationRepository, OraDbContext dbContext)
        {
            //_chatRepository = chatRepository;
            _usersService = usersService;
            _notificationLogRepository = _notificationRepository;
            _DbContext = dbContext;
            _IConfiguration = iConfiguration;
            _generalRepository = generalRepository;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.UserIdentifier == null)
                return;
            UserHandler.ConnectedIds.Add(Context.UserIdentifier);

            await Clients.All.SendAsync("UserConnected", Context.UserIdentifier);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.UserIdentifier == null)
                return;

            UserHandler.ConnectedIds.Remove(Context.UserIdentifier);

            await Clients.All.SendAsync("UserDisconnected", Context.UserIdentifier);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartCall(NotificationDirectCallPostDto notificationDirectCallPostDto)
        {
            var currentUserId = Convert.ToInt32(Context.UserIdentifier);

            var sender = await _DbContext.Users.Where(e => e.Id == currentUserId).FirstOrDefaultAsync();

            var receiver = await _DbContext.Users.Where(e => e.Id.ToString().Equals(notificationDirectCallPostDto.RecieverId)).FirstOrDefaultAsync();


            if (sender == null)
                throw new HttpStatusException("Sender not found", "SENDER_NOT_FOUND_ERR", HttpStatusCode.NotFound);


            if (receiver == null)
                throw new HttpStatusException("receiver not found", "RECEIVER_NOT_FOUND_ERR", HttpStatusCode.NotFound);

            if (sender.Id == receiver.Id)
                throw new HttpStatusException("You can't call yourself", "CAN'T_CALL_YOURSELF", HttpStatusCode.Conflict);


            string meetingId = sender.meetingId;

            var receiverToken = _generalRepository.generateMeetingToken(receiver, meetingId, true);

            var receiverLink = string.Format("{0}/{1}?jwt={2}", _IConfiguration["PUBLIC_URL"], meetingId, receiverToken);


            NotificationDirectCallGetDto notificationDirectCallGetDto = new NotificationDirectCallGetDto
            {

                NotificationTitle = "Meeting Request",
                NotificationBody = receiverLink,
                SenderId = currentUserId,
                SenderName = sender.FullName,
                RecieverId = notificationDirectCallPostDto.RecieverId,
                Status = notificationDirectCallPostDto.Status

            };


            await _notificationLogRepository.SendSignalRNotification(notificationDirectCallGetDto, receiver.Id, notificationDirectCallPostDto.Status);

            if (sender.Id != receiver.Id)
            {
                await Clients.User(notificationDirectCallPostDto.RecieverId)
                    .SendAsync("Notify", notificationDirectCallGetDto);
            }

        }




        public async Task RespondCall(NotificationDirectCallPostDto notificationDirectCallPostDto)
        {
            var currentUserId = Convert.ToInt32(Context.UserIdentifier);

            var sender = await _DbContext.Users.Where(e => e.Id == currentUserId).FirstOrDefaultAsync();

            var receiver = await _DbContext.Users.Where(e => e.Id.ToString().Equals(notificationDirectCallPostDto.RecieverId)).FirstOrDefaultAsync();


            if (sender == null)
                throw new HttpStatusException("Sender not found", "SENDER_NOT_FOUND_ERR", HttpStatusCode.NotFound);


            if (receiver == null)
                throw new HttpStatusException("receiver not found", "RECEIVER_NOT_FOUND_ERR", HttpStatusCode.NotFound);

            if (sender.Id == receiver.Id)
                throw new HttpStatusException("You can't call yourself", "CAN'T_CALL_YOURSELF", HttpStatusCode.Conflict);

            string meetingId = receiver.meetingId;

            var receiverToken = _generalRepository.generateMeetingToken(receiver, meetingId, true);

            var receiverLink = string.Format("{0}/{1}?jwt={2}", _IConfiguration["PUBLIC_URL"], meetingId, receiverToken);


            NotificationDirectCallGetDto notificationDirectCallGetDto = new NotificationDirectCallGetDto
            {

                NotificationTitle = "User Responded",
                SenderId = currentUserId,
                SenderName = sender.FullName,
                RecieverId = receiver.Id.ToString(),
                Status = notificationDirectCallPostDto.Status

            };


            if (notificationDirectCallPostDto.Status == 1)
            {
                notificationDirectCallGetDto.NotificationBody = receiverLink;
            }
            else
            {
                notificationDirectCallGetDto.NotificationBody = null;
            }

            await _notificationLogRepository.SendSignalRNotification(notificationDirectCallGetDto, receiver.Id, notificationDirectCallPostDto.Status);

            if (sender.Id != receiver.Id)
            {
                await Clients.User(notificationDirectCallPostDto.RecieverId)
                    .SendAsync("RespondNotify", notificationDirectCallGetDto);
            }

        }
    }
}