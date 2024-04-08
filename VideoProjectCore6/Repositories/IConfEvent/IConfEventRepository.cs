

using Microsoft.AspNetCore.SignalR;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.Hubs;

namespace VideoProjectCore6.Repositories.IConfEventRepository
{
    public interface IConfEventRepository
    {
        
        Task<APIResult> getConfEvents(string lang);
        Task<APIResult> getConfEventById(int id, string lang);
        Task<APIResult> addConfEvent(ConfEventPostDto confEventPostDto,string lang);
        Task<APIResult> addProsodyEvent(ProsodyEventPostDto prosodyEventPostDto, IHubContext<EventHub> HubContext);

        Task<APIResult> handleGetRoom(DateTimeRange range, string pId, string meetingID);

        Task<APIResult> handleListRoom(string lang);

        Task<APIResult> handleRoomsUsersList(List<string> names, string lang);
    }
}
