using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;

namespace VideoProjectCore6.Repositories.IStatisticsRepository
{
    public interface IStatisticsRepository
    {
        Task<List<ValueIdDesc>> ByApp(DateTimeRange range);

        Task<ListCount> ByOnlineUsers(DateTimeRange range);

        Task<List<ValueIdDesc>> ByMeetingStatus(DateTimeRange range);
    }
}
