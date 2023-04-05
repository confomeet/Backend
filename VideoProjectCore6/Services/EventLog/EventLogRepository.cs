using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IEventLogRepository;
#nullable disable
namespace VideoProjectCore6.Services.EventLog
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _IGeneralRepository;
        public EventLogRepository(OraDbContext OraDbContext, IGeneralRepository generalRepository)
        {
            _DbContext = OraDbContext;
            _IGeneralRepository = generalRepository;
        }
        public async Task<APIResult> AddEventLog(Models.EventLog log, string lang)
        {
            APIResult result = new();
            try
            {
                _DbContext.Add(log);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(log.Id, Translation.getMessage(lang, "sucsessAdd"));
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "ErrorELog"));
            }
        }

        public async Task<APIResult> AddEventLogs(List<Models.EventLog> logs, string lang)
        {
            APIResult result = new();
            try
            {
                 _DbContext.AddRange(logs); 
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(1, Translation.getMessage(lang, "sucsessAdd"));
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "ErrorELog"));
            }
        }
        public async Task<List<EventLogView>> GetEventLog(int eventId, string lang)
        {
            string s;
            var actions = await _IGeneralRepository.GetActions(lang);
            var logs = await (from el in _DbContext.EventLogs
                              join ur in _DbContext.Users
                              on el.CreatedBy equals ur.Id
                              where el.EventId == eventId
                              orderby el.CreatedDate descending
                              select new EventLogView
                              {
                                  Action = actions.TryGetValue(el.ActionId, out s) ? s : string.Empty,
                                  CreatedDate = el.CreatedDate,
                                  UserName = ur.FullName,
                                  Note = el.Note,
                              }).ToListAsync();
            return logs;
        }
    }
}
