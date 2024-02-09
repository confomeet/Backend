using VideoProjectCore6.DTOs.CommonDto;
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class LogInResultDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public int? GenderId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string EmirateId { get; set; }
        public List<int> RolesId { get; set; }
        public IList<string> RolesName { get; set; }
        // public bool ShowAadel { get; set; }
        public string Address { get; set; }
        public int? CountryId { get; set; }
        public int? AreaId { get; set; }
        public DateTime? LastLogin { get; set; }
        public byte? UserType { get; set; }
        public uint? RemoteUserId{ get; set; }
        public uint? RemoteEntityId { get; set; }
        public int? EntityId { get; set; }


        public LogInResultDto()
        {
            RolesId = new();
        }
    }
}
