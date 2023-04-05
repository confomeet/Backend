using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.ParticipantDto
{
    public class OuterParticipant: ParicipantDto
    {
        
        public string EmiratesId { get; set; }
        public string UUID { get; set; }
        public string Charge { get; set; }


        public Participant asParticipant()
        {
            return new Participant
            {
                EventId = EventId,
                Email = Email,
                UserId = (int)LocalUserId,
                Note = Note,
                Description = Description,
                IsModerator = IsModerator,
                UserType= UserType,
                Mobile= Mobile,
            };
        }
    }
}
