namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class FCMTokenDto
    {
        public int LocalUserId { get; set; }
        public string DeviceInfo { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
    }
}
