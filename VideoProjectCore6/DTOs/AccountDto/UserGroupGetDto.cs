
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserGroupGetDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<AclsGetDto> acls { get; set; }
    }
}
