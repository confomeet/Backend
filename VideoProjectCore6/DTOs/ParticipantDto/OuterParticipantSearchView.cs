namespace VideoProjectCore6.DTOs.ParticipantDto
{
    public class OuterParticipantSearchView : ParticipantSearchView
    {
        public uint? UserId { get; set; }
        public int? UserType { get; set; }

        public byte? Status { get; set; }
    }
}
