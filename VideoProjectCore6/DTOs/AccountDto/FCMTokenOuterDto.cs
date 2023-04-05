namespace VideoProjectCore6.DTOs.AccountDto
{
    public class FCMTokenOuterDto : FCMTokenDto
    {
        public new int LocalUserId { get; set; } = 0;
        public int UserId { get; set; }
        public byte UserType { get; set; }
    }
}
