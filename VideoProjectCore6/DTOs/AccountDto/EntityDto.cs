namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class EntityDto
    {
       
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public uint? UserId { get; set; }
        public byte? UserType { get; set; }

    }
}
