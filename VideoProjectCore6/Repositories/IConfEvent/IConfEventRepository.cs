

using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;

namespace VideoProjectCore6.Repositories.IConfEventRepository
{
    public interface IConfEventRepository
    {
        Task<APIResult> AddProsodyEvent(ProsodyEventPostDto prosodyEventPostDto);

        Task<APIResult> HandleGetRoom(DateTimeRange range, string pId, string meetingID, string lang);

        Task<APIResult> HandleListRoom(string lang);
    }
}
