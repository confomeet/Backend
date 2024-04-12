using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.DTOs;
namespace VideoProjectCore6.Repositories.IEventRepository
{
    public interface IEventRepository
    {
        Task<APIResult> AddEvent(EventPostDto EventPostDto, int addBy, string lang);
        Task<APIResult> AddMeetingEvent(EventWParticipant dto, int addBy, bool sendNotification, string lang);
        Task<APIResult> AddConnectedEvents(FullEventPostDto EventPostDto, int addBy, string lang,bool justCreatorLink,bool checkUserTime = false, bool sendNotification=false,  bool checkWorkTime = false,string? appId = null);
        Task<APIResult> AddParticipantsToEvents(List<ParicipantDto> dtos, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll);
        Task<APIResult> AddParticipantsToEventsScoped(List<ParicipantDto> dtos, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll);
        Task<APIResult> AddParticipantsToEventsScoped(ParticipantsAsObj participants, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll);
        Task<APIResult> UpdateEvent(int id, int updatedBy,MeetingEventDto dto, UpdateOption opt, string lang);
        Task<int> DeleteEvent(int id);
        Task<List<EventGetDto>> GetEvent(int userId);
        Task<List<EventGetDto>> GetEventByMeetingId(string MeetingId);
        Task<List<EventFullView>> GetAllOfUser(int userId,EventSearchObject? obj=null,bool withRelatedUserEvents=false,string lang="ar");
        Task<ListCount> GetAll(int CurrentUserId, EventSearchObject? obj = null, int pageIndex = 1, int pageSize = 25, string lang = "ar");
        Task<APIResult> UnlinkSubEvent(int id, int byUserId);
        Task<APIResult> EventById(string eventId);
        Task<EventFullView?> EventDetails(int id,int userId, string timeZoneId);

        Task<APIResult> EventParticipantLinks(int id, int userId);
        Task<APIResult> UpdateEventParticipants(ParticipantsAsObj dtos, int eventId, int addBy, string lang, bool sendNotification, bool sendToAll);
        Task<APIResult> AddRecurrenceEvents(EventWParticipant dto, DateTimeOfRule rDates, int addBy,bool sendNotification, string lang);
        Task<APIResult> ShiftRecurrenceEvents(int eventId, DateTimeRange dto, int updatedBy,bool updateThis=true, string lang="ar" );
        Task<APIResult> Reschedule(int id, MeetingEventDto dto, int updatedBy, string lang);
        Task<APIResult> ReNotifyParticipants(int eventId,int userId, string lang);

        Task<APIResult> Cancel(int eventId, int updatedBy, string lang);
        // ------------------------------------------------------------- //
        // ------- Endpoints for active, finished meetings realtime ---- //
        // ------------------------------------------------------------- //
        Task<APIResult> ActiveMeetings(DateTimeRange range, string lang);
        Task<APIResult> FinishedMeetings(DateTimeRange range, string meetingId, string lang);
        Task<APIResult> MeetingDetails(int? id, string meetingId, string lang);

        
    }
}
