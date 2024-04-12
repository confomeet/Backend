

using Microsoft.AspNetCore.SignalR;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.Hubs;

namespace VideoProjectCore6.Repositories.IConfEventRepository
{
    public interface IConfEventRepository
    {
        Task<APIResult> AddProsodyEvent(ProsodyEventPostDto prosodyEventPostDto, IHubContext<EventHub> HubContext);

        Task<APIResult> HandleGetRoom(DateTimeRange range, string pId, string meetingID);

        Task<APIResult> HandleListRoom(string lang);
    }
}
