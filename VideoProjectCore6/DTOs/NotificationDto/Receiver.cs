#nullable disable
namespace VideoProjectCore6.DTOs.NotificationDto
{
    public class Receiver
    {
        public int? Id { get; set; }
        public int? ParticipantId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public uint? UserId { get; set; }
        public int? UserType { get; set; }
        public string EmiratesId { get; set; }
        public string UuId { get; set; }
        public List<string> Tokens { get; set; }
        public bool IsModerator { get; set; } = false;
        public string Charge { get; set; } 

        public Receiver() { }
        public Receiver(int? id,string name, string mobile, string email, List<string> tokens, int? participantId, uint? UserId, int? UserType,string emiratesId, string _UUID)
        {
            Id = id;
            Name = name;
            Mobile = mobile;
            Email = email;
            Tokens = tokens;
            ParticipantId = participantId;
            EmiratesId = emiratesId;
            UuId = _UUID;
        }
        public Receiver (string email)
        {
            Id = null;
            Name = email;
            Mobile = null;
            Email = email;
        }
    }
}
