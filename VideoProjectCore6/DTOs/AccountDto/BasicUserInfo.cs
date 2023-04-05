namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class BasicUserInfo
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string EmiratesId { get; set; }
        public string UUID { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        //public int? UserId { get; set; }
        public byte? UserType { get; set; }
        public bool HasIdentity()
        {
            return !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(EmiratesId) || !string.IsNullOrWhiteSpace(UUID);
        }
    }
}
