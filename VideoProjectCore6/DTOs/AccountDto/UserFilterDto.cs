

#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.AccountDto
{
    public class UserFilterDto
    {
        public List<int> userType { get; set; } = null;

        public List<int> userGroups { get; set; } = null;


        public string text { get; set; } = null;
        public string name { get; set; } = null;

        public bool? isLocked { get; set; }

        public bool? isConfirmed { get; set; }

        //[EmailAddress]
        public string email { get; set; } = null;

        public int pageSize { get; set; } = 25;

        public int pageIndex { get; set; } = 1;
    }
}
