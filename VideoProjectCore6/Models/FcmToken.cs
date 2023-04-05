namespace VideoProjectCore6.Models
#nullable disable
{
    public class FcmToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceInfo { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public virtual User User { get; set; } 
    }
}
