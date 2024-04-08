using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;

namespace VideoProjectCore6.Repositories.IStatisticsRepository
{
    public interface IStatisticsRepository
    {
        Task<List<ValueIdDesc>> EventsByApp(DateTimeRange range, string lang);

        Task<ListCount> UsersByStatus(DateTimeRange range, string lang);

        Task<List<ValueIdDesc>> ActiveRooms(DateTimeRange range);
    }
}
