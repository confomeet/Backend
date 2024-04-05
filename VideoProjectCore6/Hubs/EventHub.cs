using SignalRSwaggerGen.Attributes;
using Microsoft.AspNetCore.SignalR;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IConfEventRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace VideoProjectCore6.Hubs
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [SignalRHub("/eventsStatus")]
    public class EventHub : Hub
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _usersService;

        private readonly IConfEventRepository _confEvent;

        public static List<ConfEvent> _Notifications = new List<ConfEvent>();

        private readonly OraDbContext _DbContext;


        public static IHubContext<EventHub>? Current { get; set; }

        public EventHub(IEventRepository eventRepository, IUserRepository usersService, IConfEventRepository confEvent, OraDbContext OraDbContext)
        {

            _eventRepository = eventRepository;
            _usersService = usersService;
            _DbContext = OraDbContext;
            _confEvent = confEvent;

        }

        public override async Task OnConnectedAsync()
        {

            await Clients.All.SendAsync("UserConnected", Context.UserIdentifier);
            //UserHandler.ConnectedIds.Add(Context.UserIdentifier);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.UserIdentifier);
            //UserHandler.ConnectedIds.Remove(Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task StatusUpdated(ConfEvent confEvent)
        {
            if (Context.UserIdentifier == null)
                return;
            await Clients.User(Context.UserIdentifier)
                   .SendAsync("NotifyEventStatus", confEvent);

        }

    }

}
