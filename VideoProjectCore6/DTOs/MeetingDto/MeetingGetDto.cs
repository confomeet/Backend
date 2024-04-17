using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.MeetingDto
{
    public class MeetingGetDto
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public string Password { get; set; }
        public bool? RecordingReq { get; set; }
        public string MeetingId { get; set; }
        public int Status { get; set; }

        public bool? AutoLobby { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        //public List<int> Participants { get; set; }

        public MeetingGetDto()
        {
            // Participants = new List<int>();
        }

        public static MeetingGetDto GetDTO(Meeting meeting)
        {
            MeetingGetDto dto = new()
            {
                Id = meeting.Id,
                MeetingId = meeting.MeetingId,
                Description = meeting.Description,
                Topic = meeting.Topic,
                Status = meeting.Status,
                StartDate = meeting.StartDate,
                EndDate = meeting.EndDate,
                TimeZone = meeting.TimeZone,
                Password = meeting.Password,
                RecordingReq = Convert.ToBoolean(meeting.RecordingReq),
                AutoLobby = Convert.ToBoolean(meeting.AutoLobby),
                UserId = meeting.UserId,
                EventId = meeting.EventId
            };

            return dto;
        }
    }

    public class IsAttended
    {
        public bool IsOnline { get; set; }
        public bool IsLate { get; set; }
        public DateTime? LastLogIn { get; set; }
    }

    public class UserLog
    {
        public List<LogEntry> UserLogs { get; set; }

        public UserLog()
        {
            UserLogs = new List<LogEntry>();
        }
    }
    public class LogEntry
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
