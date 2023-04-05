using Microsoft.AspNetCore.SignalR;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services.UserService;


#nullable disable
namespace VideoProjectCore6.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
            public string GetUserId(HubConnectionContext connection)
        {
            //var userId = UserRepository.FindUser(Int32.Parse(connection.User.Identity.Name), "en");
            return connection.User.Identity.Name;
        }
    }
}
