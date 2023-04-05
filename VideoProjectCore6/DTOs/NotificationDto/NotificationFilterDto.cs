using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class NotificationFilterDto
    {

        public string text { get; set; } = null;
        public string name { get; set; } = null;


        //[EmailAddress]
        public string email { get; set; } = null;

        public string PhoneNumber { get; set; }

        public int pageSize { get; set; } = 25;

        public int pageIndex { get; set; } = 1;


    }
}
