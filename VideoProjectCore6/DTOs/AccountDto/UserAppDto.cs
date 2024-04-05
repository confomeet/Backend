namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserAppDto
    {
        public int UserId { get; set; }
        public string ServiceId { get; set; } = string.Empty;
        public string ApplicationId { get; set; } = string.Empty;
        public bool PNS { get; set; }
    }
}
