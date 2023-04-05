
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserGroupPostDto
    {
        public string GroupName { get; set; }

        public string Description { get; set; }


        public List<int> ACLs { get; set; }
    }
}
