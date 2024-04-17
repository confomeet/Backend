using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ParticipantDto;

namespace VideoProjectCore6.Repositories.IEventRepository
{
    public interface IEventRepository
    {
        Task<APIResult> AddMeetingEvent(EventWParticipant dto, int addBy, bool sendNotification, string lang);
        Task<APIResult> AddParticipantsToEvents(List<ParicipantDto> dtos, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll);
        Task<APIResult> UpdateEvent(int id, int updatedBy,MeetingEventDto dto, UpdateOption? opt, string lang);
        Task<List<EventFullView>> GetAllOfUser(int userId,EventSearchObject? obj=null, string lang="ar");
        Task<ListCount> GetAll(int CurrentUserId, EventSearchObject? obj = null, int pageIndex = 1, int pageSize = 25, string lang = "ar");
        Task<EventFullView?> EventDetails(int id,int userId, string timeZoneId);
        Task<APIResult> AddRecurrenceEvents(EventWParticipant dto, DateTimeOfRule rDates, int addBy,bool sendNotification, string lang);
        Task<APIResult> Cancel(int eventId, int updatedBy, string lang);
    }
}
