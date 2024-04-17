#nullable disable
using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.DTOs.ParticipantDto
{
    public class ParicipantDto
    {
        public int? Id { get; set; }
        public int? LocalUserId { get; set; }
        public string FullName { get; set; }
        public int EventId { get; set; }
        public string MeetingToken { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public bool IsModerator { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public uint? UserId { get; set; }//--- UserId as it come from PP Request
        public int? GroupIn { get; set; }

        public Receiver asReceiver(int? participantId)
        {          
         return new Receiver(LocalUserId, FullName, Mobile, Email, null, participantId,null,null);
        }
    }
}
