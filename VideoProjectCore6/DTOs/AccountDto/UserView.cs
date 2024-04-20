using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserView
    {
        public string? PhoneNumber { get; set; }

        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public bool Locked { get; set; }
        public bool Confirmed { get; set; }
        public string? Address { get; set; }
        public int? Country { get; set; }
        public string? CountryName { get; set; }
        public bool? Enable2FA { get; set; }
        public List<int> Roles { get; set; } = [];
        public List<ValueId> UserGroups { get; set; } = [];
    }
}
