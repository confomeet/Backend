namespace VideoProjectCore6.DTOs.EventDto
#nullable disable
{
    public class EventSearchObject
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public bool allParticipant { get; set; } = false;
        //public List<string> Participants { get; set; }
        public string Participant { get; set; }
        public bool Pagination { get; set; } = true;

        public short? TypeOrder { get; set; }

        public string Entity { get; set; }

        // ------------------------------------------
        // ------------------------------------------
        public string Organizer { get; set; }
        public short? EventType { get; set; }

        public string Participants { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        // -------------------------------------------
        // -------------------------------------------
        public string MeetingId { get; set; }
        public bool IsEmpty()
        {
            return StartDate == null && EndDate == null && string.IsNullOrWhiteSpace(Topic) && string.IsNullOrWhiteSpace(SubTopic);
        }
        public bool HasDateSearch()
        {
            return (StartDate != null && StartDate != DateTime.MinValue) || (EndDate != null && EndDate != DateTime.MinValue);
        }
        public bool HasStringSearch()
        {
            return (!string.IsNullOrWhiteSpace(Topic)
                || !string.IsNullOrWhiteSpace(SubTopic)
                || !string.IsNullOrWhiteSpace(Participant)
                || !string.IsNullOrWhiteSpace(MeetingId)
                || !string.IsNullOrWhiteSpace(Organizer)
                || !string.IsNullOrWhiteSpace(Email)
                || !string.IsNullOrWhiteSpace(PhoneNumber)
                || !string.IsNullOrWhiteSpace(Entity)
                || EventType != null);
        }
    }
    
}
