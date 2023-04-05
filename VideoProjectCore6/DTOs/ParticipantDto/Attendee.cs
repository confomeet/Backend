namespace VideoProjectCore6.DTOs.ParticipantDto
#nullable disable
{
    public class Attendee
    {
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? PartyId { get; set; }

    }
}
