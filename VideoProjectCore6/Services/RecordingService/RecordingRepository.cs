using Amazon.S3;
using Amazon.S3.Model;
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
        private readonly AmazonS3Client? s3Client;


        public RecordingRepository(OraDbContext dbContext, IConfiguration configuration, ILogger<RecordingRepository> logger)
        {
            _DbContext = dbContext;
            _IConfiguation = configuration;
            _ILogger = logger;
            s3Client = TryCreateS3Client();
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

        public async Task<APIResult> AddS3Recording(S3RecordingPostDto recordingDto)
        {
            if (s3Client == null)
            {
                _ILogger.LogWarning("Tried to save S3 recording while S3 integration is not configured");
                return new APIResult().FailMe(-1, "S3 uploader is not configured", false, APIResult.RESPONSE_CODE.ERROR);
            }

            var guid = Guid.NewGuid();
            string VideoPublicLink = _IConfiguation["CurrentHostName"] + $"/api/v1/Recording/DownloadS3Record/{guid}";
            var recordingLog = new RecordingLog
            {
                FileSize = recordingDto.FileSize.ToString(),
                RecordingfileName = recordingDto.FileName,
                CreatedDate = DateTime.UtcNow,  // FIXME: this does not respect the time spent on uploading video to cloud
                IsSucceeded = 1,
                VideoPublicLink = VideoPublicLink,
                Status = RecordingStatus.Uploaded,
                UploadDate = DateTime.UtcNow,
            };
            await _DbContext.AddAsync(recordingLog);
            await _DbContext.SaveChangesAsync();
            var recording = new S3Recording
            {
                Uuid = guid,
                RecordingLog = recordingLog.Id,
                FileName = recordingDto.FileName,
                FileSize = recordingDto.FileSize,
                Bucket = recordingDto.Bucket,
                Key = recordingDto.Key,
            };
            await _DbContext.AddAsync(recording);
            await _DbContext.SaveChangesAsync();

            _ILogger.LogInformation("Saved new meeting#{} recording to s3://{}/{}", recordingDto.MeetingId, recordingDto.Bucket, recordingDto.Key);
            return new APIResult().SuccessMe(recordingLog.Id, "Saved", false, APIResult.RESPONSE_CODE.CREATED);
        }

        public async Task<string> GetS3RedirectUrl(Guid recordingId)
        {
            if (s3Client == null) {
                return string.Empty;
            }

            var s3Recording = await _DbContext.S3Recordings.FindAsync(recordingId);
            if (s3Recording == null) {
                _ILogger.LogDebug("Recording {} saved to S3 not found, cannot provide download url", recordingId);
                return string.Empty;
            }

            GetPreSignedUrlRequest request = new()
            {
                BucketName = s3Recording.Bucket,
                Key = s3Recording.Key,
                Expires = DateTime.UtcNow.AddHours(24),
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.GET,
            };
            var downloadUrl = s3Client.GetPreSignedURL(request);
            _ILogger.LogTrace("Resulting S3 download url for recording {} is {}", recordingId, downloadUrl);
            return downloadUrl;
        }

        private AmazonS3Client? TryCreateS3Client()
        {
            var serviceUrl = _IConfiguation["CONFOMEET_S3_URL_OVERRIDE_FOR_AWS_SDK"] ?? _IConfiguation["CONFOMEET_S3_URL"];
            if (serviceUrl == null) {
                _ILogger.LogInformation("No S3 service url specified. Recording upload to S3 is disabled");
                return null;
            }
            _ILogger.LogInformation("S3 service url in use is {}", serviceUrl);
            var s3Config = new AmazonS3Config { ServiceURL = serviceUrl };
            return new AmazonS3Client(s3Config);
        }
    }
}
