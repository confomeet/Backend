#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class PrisonReportGetDto
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public string Base64Data { get; set; }

        public string PreviewDate { get; set; }

        public string NumOfPages { get; set; }
    }
}
