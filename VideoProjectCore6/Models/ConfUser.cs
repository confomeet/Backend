
#nullable disable
namespace VideoProjectCore6.Models
{
    public class ConfUser
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string ConfId { get; set; }
        
        public string Email { get; set; }

        public string Avatar { get; set; } = "string";

        public string ProsodyId { get; set; }

        public DateTime ConfTime { get; set; }

    }
}
