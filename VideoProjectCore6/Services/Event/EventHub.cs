using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;


namespace VideoProjectCore6.Controllers.Event
{
    public class EventHub : Hub
    {

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Message", "Connected successfully!");
        }

        //public async Task SubscribeToBoard(Guid Id)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, Id.ToString());
        //    await Clients.Caller.SendAsync("Message", "Added successfully!");
        //}

    }
}
