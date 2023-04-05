#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class ClientInfo
    {
        public ushort Id { get; set; }
        public string AppName { get; set; } 
        public string AppKey { get; set; } 
        public string ClientName { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
    }
}
