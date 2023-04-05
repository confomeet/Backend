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


        public RecordingRepository(OraDbContext dbContext, IConfiguration configuration)
        {
            _DbContext = dbContext;
            _IConfiguation = configuration;
        }


        public async Task<APIResult> AddRecordingLog(RecordingPostDto recordingPostDto)
        {

            APIResult result = new APIResult();

            DateTime now = DateTime.Now;

            RecordingLog newRecordingLog = new RecordingLog()
            {
                //VideoType = recordingPostDto.Type,
                RecordingfileName = recordingPostDto.RecordingfileName,
                //RecordingDate = recordingPostDto.RecordingDate,
                FileSize = recordingPostDto.FileSize,
                CreatedDate = now,
                FilePath = recordingPostDto.FilePath,
                IsSucceeded = recordingPostDto.Status
            };

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
