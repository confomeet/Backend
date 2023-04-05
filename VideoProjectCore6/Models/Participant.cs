#nullable disable

using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Models
{
    public partial class Participant
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string MeetingToken { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public byte? RecStatus { get; set; }
        public byte? UserType { get; set; }
        public bool IsModerator { get; set; }
        public Guid? Guid { get; set; }
        public int? GroupIn { get; set; }
        public uint? PartyId { get; set; }
        public virtual Event Event { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public string Charge { get; set; }
        public Receiver AsReceiver()
        {
            return new Receiver
            {
                Id = UserId,
                ParticipantId = Id,
                Email = Email,
                Mobile = Mobile,
                IsModerator = IsModerator,
                UserId = PartyId,
                UserType = UserType
            };
        }
    }
}
