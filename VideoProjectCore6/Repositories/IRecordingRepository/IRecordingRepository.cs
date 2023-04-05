using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.RecordingDto;

namespace VideoProjectCore6.Repositories.IRecordingRepository
{
    public interface IRecordingRepository
    {
        Task<APIResult> AddRecordingLog(RecordingPostDto recordingPostDto);
    }
}
