namespace VideoProjectCore6.DTOs.ParticipantDto
#nullable disable
{
    public class ParticipantView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsModerator { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public byte? UserType { get; set; }
        public Boolean ParticipantStatus { get; set; }
        public bool Remind { get; set; } = false;
        public int?  GroupIn { get; set; }
        public string MeetingLink { get; set; }
        public uint? PartyId { get; set; }//--- UserId as it come from PP Request
    }
}
