using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories.IEventLogRepository
{
    public interface IEventLogRepository
    {
        Task<APIResult> AddEventLog(EventLog log, string lang);
        Task<APIResult> AddEventLogs(List<EventLog> logs, string lang);
        Task<List<EventLogView>> GetEventLog(int eventId, string lang);
    }
}
