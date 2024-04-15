using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.MeetingDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.ParticipantDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Services;

namespace VideoProjectCore6.Repositories.IMeetingRepository
{
    public interface IMeetingRepository
    {

        /// <summary>
        /// Add a new schedule meeting.
        /// </summary>
        /// <param name="meetingDto">The meeting details to be added of type MeetingPostDto</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
          Task<APIResult> AddMeeting(MeetingPostDto dto, int addBy, string lang);
        
        /// <summary>
        /// Update an existing meeting.
        /// </summary>
        /// <param name="rowId">The id of the record</param>
        /// <param name="meetingDto">The meeting details to be added of type MeetingPostDto</param>
        /// <param name="userId">The id of the user</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<APIResult> UpdateMeeting(int id, MeetingPostDto dto, int userId, string lang);

        /// <summary>
        ///    Get a list of the meeting which created by the user.
        /// </summary>
        /// <param name="userId">The id of the certain user</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<List<MeetingGetDto>> GetMeetings(int userId, string lang);


        /// <summary>
        /// Get a  meeting by row id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MeetingGetDto> GetMeetingById(int id, string lang);

        /// <summary>
        /// Get a  meeting accord to meetingId which created by the user.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<MeetingGetDto> GetMeetingByMeetingId(string meetingId, string lang);

        /// <summary>
        /// Get a  meeting accord to meetingId and password.
        /// </summary>
        /// <param name="userId">The id of the certain user</param>
        /// <param name="meetingId"></param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<MeetingGetDto> GetMeetingByMeetingIdAndPassword(string meetingId, string password, string lang);


        /// <summary>
        /// Set the status of an existing meeting.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="newStatus">FINISHED = -1 OR PENDING = 0 OR STARTED = 1</param>
        /// <param name="cahngeAppointment">Convert the meeting appointment to the current date if true</param>
        /// <returns></returns>
        Task<int> SetMeetingStatus(string meetingId, Constants.MEETING_STATUS newStatus, bool cahngeAppointment, string lang);

        /// <summary>
        /// Get true if the password of the meeting is required otherwise false.
        /// </summary>
        /// <param name="meetingId">The id of the meeting</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<bool> MeetingHasPassword(string meetingId);

        /// <summary>
        /// Determine if meeting id is existed before.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns>true if exist otherwise false</returns>
        Task<bool> IfExistMeeting(string meetingId);

        Task<APIResult> MeetingJWT(int participantId,Guid guid,int? userId, string lang);
        Task<object> MeetingJWT(string meetingId, int? userId,JoinData user,  string lang);

        Task<bool> LogInToMeeting(string meetingNo);
        Task<bool> InviteToMeeting(string meetingId, List<string> contacts, string lang = "en",bool local = true);

    }
}
