using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.Models;
using Exception = System.Exception;

namespace VideoProjectCore6.Services.RecordingService
{
    public class YandexDiskRecordingUploader : BackgroundService, IRecordingUploader
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<YandexDiskRecordingUploader> logger;
        private readonly HttpClient httpClient = new();
        private readonly AsyncServiceScope scope;
        private string Token
        {
            get => configuration["YandexDisk:DebugToken"]!;
        }

        public YandexDiskRecordingUploader(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<YandexDiskRecordingUploader> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            scope = scopeFactory.CreateAsyncScope();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (scope)
            {
                if (!IsConfigurationValid(out string reason)) {
                    logger.LogError("Service is misconfigured: {}", reason);
                    return;
                }
                while (!stoppingToken.IsCancellationRequested)
                {
                    await UploadRecordsAsync();
                    await Task.Delay(30 * 1000, stoppingToken);
                }
            }
        }

        private async Task UploadRecordsAsync()
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OraDbContext>();
            var rl = await PollNextRecordToUploadAsync(dbContext);
            while (rl is not null) {
                bool succeeded = await TryUploadSingleRecord(rl);
                await dbContext.SaveChangesAsync();
                if (!succeeded)  // Backoff to avoid spamming Yandex.Disk API.
                    break;
                rl = await PollNextRecordToUploadAsync(dbContext);
            }
        }

        private async Task<bool> TryUploadSingleRecord(RecordingLog rl)
        {
            try
            {
                var href = await GetUploadUrlAsync(rl.RecordingfileName);
                var fileUploaded = await DoUploadFileAsync(href, rl.FilePath);
                if (!fileUploaded)
                {
                    logger.LogError("Uploading of recording {}:{} failed. See logs above ^^^^^^^^^^^^^^^", rl.Id, rl.RecordingfileName);
                    rl.Status = RecordingStatus.UploadingFailed;
                    return false;
                }
                var public_link = await PublishFile(rl.RecordingfileName);
                if (public_link.Length == 0) {
                    rl.Status = RecordingStatus.UploadingFailed;
                    return false;
                }
                logger.LogInformation("Uploaded file {}:{} successfully published. public_url={}", rl.Id, rl.RecordingfileName, rl.VideoPublicLink);
                rl.Status = RecordingStatus.Uploaded;
                rl.VideoPublicLink = public_link;
                return true;
            }
            catch (Exception e)
            {
                logger.LogError("Uploading of recording {}:{} failed. {}", rl.Id, rl.RecordingfileName, e.Message);
                rl.Status = RecordingStatus.UploadingFailed;
                return false;
            }
            finally
            {
                rl.UploadDate = DateTime.UtcNow;
            }
        }

        private class ObjectMetaPublicUrl
        {
            [Newtonsoft.Json.JsonProperty("public_url")]
            public string PublicUrl { get; set; } = string.Empty;
        }

        private async Task<string> PublishFile(string filename)
        {
            var pathOnDisk = $"app:/Recordings/{filename}";
            var request = BuildPublishRequest(pathOnDisk);
            var httpResponse = await httpClient.SendAsync(request);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                logger.LogError("Failed to publish uploaded file {}  status_code={} error={}", filename, httpResponse.StatusCode, error);
                return string.Empty;
            }

            request = BuildGetPublicUrlRequest(pathOnDisk);
            httpResponse = await httpClient.SendAsync(request);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                logger.LogError("Failed to get public_url of file {} after uploading to Yandex.Disk: status_code={} error={}", filename, httpResponse.StatusCode, error);
                return string.Empty;
            }

            var omPublicUrl = await httpResponse.Content.ReadAsAsync<ObjectMetaPublicUrl>();
            return omPublicUrl.PublicUrl;
        }

        private HttpRequestMessage BuildPublishRequest(string filePathOnDisk)
        {
            const string baseUrl = "https://cloud-api.yandex.net/v1/disk/resources/publish?";
            var urlParams = HttpUtility.ParseQueryString(string.Empty);
            urlParams["path"] = filePathOnDisk;
            var request = new HttpRequestMessage(HttpMethod.Put, baseUrl + urlParams.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", Token);
            return request;
        }

        private HttpRequestMessage BuildGetPublicUrlRequest(string filePathOnDisk)
        {
            const string baseUrl = "https://cloud-api.yandex.net/v1/disk/resources?";
            var urlParams = HttpUtility.ParseQueryString(string.Empty);
            urlParams["path"] = filePathOnDisk;
            urlParams["fields"] = "public_url";
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + urlParams.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", Token);
            return request;
        }

        private class ResponseObjectLink
        {
            public string Href { get; set; } = string.Empty;
            public string Method { get; set; } = string.Empty;
            public bool Templated { get; set; }
        };

        private async Task<string> GetUploadUrlAsync(string filename)
        {
            var request = BuildGetUploadUrlRequest($"app:/Recordings/{filename}");
            var httpResponse = await httpClient.SendAsync(request);

            if (httpResponse.StatusCode == HttpStatusCode.OK) {
                var response = await httpResponse.Content.ReadAsAsync<ResponseObjectLink>();
                if (!IsValid(response)) {
                    throw new ApiError("Response format changed. Please check documentation for uploading file to Yandex.Disk");
                }
                return response.Href;
            }

            var error = await httpResponse.Content.ReadAsStringAsync();
            logger.LogError("Failed to get url for uploading recording {} to Yandex.Disk: status_code={}, error={}", filename, httpResponse.StatusCode, error);
            throw new ApiError($"Getting uploading url for recording {filename} failed, error={error}");
        }

        private HttpRequestMessage BuildGetUploadUrlRequest(string filePathOnDisk)
        {
            const string baseUrl = "https://cloud-api.yandex.net/v1/disk/resources/upload?";
            var urlParams = HttpUtility.ParseQueryString(string.Empty);
            urlParams["path"] = filePathOnDisk;

            // We expect that nobody else perform CRUD operations with disk on destination file,
            // so when file exists it means, that in past we have started recording upload operation
            // but the operation failed. In such case it is safe to restart the operation overriding everything we've uploaded.
            urlParams["overwrite"] = "true";

            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + urlParams.ToString());
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", Token);
            return request;
        }

        private static bool IsValid(ResponseObjectLink response) {
            if (response.Href is null || response.Href.Length == 0)
                return false;
            if (response.Method is null || response.Method.Length == 0)
                return false;
            return true;
        }

        async Task<bool> DoUploadFileAsync(string href, string filePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, href);
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", Token);
            request.Content = new StreamContent(File.OpenRead(filePath));
            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation("Recording {} scuccessfully uploaded to Yandex.Disk", filePath);
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Uploading of recording {} failed, status_code={},  error={}", filePath, response.StatusCode, error);
            return true;
        }

        static async Task<RecordingLog?> PollNextRecordToUploadAsync(OraDbContext dbContext)
        {
            return await dbContext.RecordingLogs.Where(rl =>
                rl.Status == RecordingStatus.Recorded
                || rl.Status == RecordingStatus.UploadingFailed
            ).OrderBy(rl => rl.Status).ThenBy(rl => rl.UploadDate ?? DateTime.UnixEpoch).FirstOrDefaultAsync();
        }

        private bool IsConfigurationValid(out string misconfigDescription)
        {
            if (string.IsNullOrEmpty(configuration["YandexDisk:DebugToken"]))
            {
                misconfigDescription = "YandexDisk:DebugToken for recordings uploading is not specified";
                return false;
            }
            misconfigDescription = string.Empty;
            return true;
        }
    }

    public class ApiError : Exception
    {
        public ApiError(string message)
            : base(message)
        {
        }
    }
}
