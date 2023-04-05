using VideoProjectCore6.DTOs.ParticipantDto;
#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class FullEventPostDto :EventPostDto
    {
        public List<ParticipantWParent> Participants { get; set; }
        public List<FullEventPostDto> SubEvents { get; set; }
 
    public FullEventPostDto()
    {
        Participants = new();
    } 
    }
}
