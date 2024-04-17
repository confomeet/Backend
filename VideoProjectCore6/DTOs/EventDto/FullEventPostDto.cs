#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class FullEventPostDto :EventPostDto
    {
        public List<ParticipantDto.ParicipantDto> Participants { get; set; }
 
    public FullEventPostDto()
    {
        Participants = new();
    } 
    }
}
