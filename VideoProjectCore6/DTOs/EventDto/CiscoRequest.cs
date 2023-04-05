using Flurl;
namespace VideoProjectCore6.DTOs.EventDto
#nullable disable

{
    public class CiscoRequest
    {
        public string Subject { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string SenderEmail { get; set; }

        public string BuildPath(string baseUrl)
        {
            return baseUrl.AppendPathSegment(Subject)
                .AppendPathSegment(StartDateTime.ToString("yyyyMMddTHHmmss"))
                .AppendPathSegment(EndDateTime.ToString("yyyyMMddTHHmmss"))
                .AppendPathSegment(SenderEmail)
                .ToString();
        }
    }
}
