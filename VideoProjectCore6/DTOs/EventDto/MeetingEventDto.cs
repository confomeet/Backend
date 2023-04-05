using VideoProjectCore6.DTOs.ParticipantDto;

namespace VideoProjectCore6.DTOs.EventDto
#nullable disable
{
    public class MeetingEventDto : EventDto
    {
        public bool PasswordReq { get; set; }
        public string Password { get; set; }
        public bool? RecordingReq { get; set; }
        public bool? SingleAccess { get; set; }

        public bool AllDay { get; set; }
        public bool AutoLobby { get; set; }

        public bool Notify { get; set; } = true;
        public List<ParicipantDto> Participants { get; set; }
    }
}
