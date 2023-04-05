namespace VideoProjectCore6.DTOs.ParticipantDto
#nullable disable
{
    public class PPParticipantsAsObj
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public List<ParticipantWParent> Participants  { get; set; }
    
    }
}
