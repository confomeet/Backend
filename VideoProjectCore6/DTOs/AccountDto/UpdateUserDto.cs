namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Enable2FA { get; set; }
        public string? Address { get; set; }
        public int? Country { get; set; }
        public List<int>? Roles { get; set; }
        public List<int>? Groups { get; set; }
    }
}
