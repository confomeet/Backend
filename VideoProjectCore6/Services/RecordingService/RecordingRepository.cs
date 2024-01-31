using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.RecordingDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IRecordingRepository;

namespace VideoProjectCore6.Services.RecordingService
{
    public class RecordingRepository : IRecordingRepository
    {

        private readonly OraDbContext _DbContext;

        private readonly IConfiguration _IConfiguation;
        private readonly ILogger _ILogger;


        public RecordingRepository(OraDbContext dbContext, IConfiguration configuration, ILogger<RecordingRepository> logger)
        {
            _DbContext = dbContext;
            _IConfiguation = configuration;
            _ILogger = logger;
        }


        public async Task<APIResult> AddRecordingLog(RecordingPostDto recordingPostDto)
        {

            APIResult result = new APIResult();

            DateTime now = DateTime.Now;

            var videoPublicLink = _IConfiguation["Meeting:host"] + recordingPostDto.FilePath.Replace("/config/recordings", "/recordings");
            RecordingLog newRecordingLog = new RecordingLog()
            {
                //VideoType = recordingPostDto.Type,
                RecordingfileName = recordingPostDto.RecordingfileName,
                //RecordingDate = recordingPostDto.RecordingDate,
                FileSize = recordingPostDto.FileSize,
                CreatedDate = now,
                FilePath = recordingPostDto.FilePath,
                IsSucceeded = recordingPostDto.Status,
                VideoPublicLink = videoPublicLink,
                Status = RecordingStatus.Uploaded
            };

            _ILogger.LogInformation("Adding recording {} at {} in status={}", newRecordingLog.RecordingfileName, newRecordingLog.FilePath, newRecordingLog.Status);

            try
            {

                await _DbContext.RecordingLogs.AddAsync(newRecordingLog);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(newRecordingLog.Id, "Created", true,  APIResult.RESPONSE_CODE.OK);

            }

            catch
            {
                return result.FailMe(-1, "Fail to add");
            }

        }
    }
}
