using VideoProjectCore6.DTOs.ParticipantDto;
#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class EventWParticipant : EventPostDto
    {
        public List<ParicipantDto> Participants { get; set; }

        public List<int> GroupIds { get; set; }
    }
}
