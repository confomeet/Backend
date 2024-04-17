using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.ParticipantDto;

namespace VideoProjectCore6.Repositories.IParticipantRepository;

public interface IParticipantRepository
{
    Task<List<ParticipantSearchView>> SearchParticipant(string toSearch);
    Task<APIResult> AddParticipants(List<ParicipantDto> dto, int addBy, string lang);
    Task<List<MeetingUserLink>> NotifyParticipants(List<Receiver> participants, string? mettingId, List<NotificationLogPostDto> notifications_, Dictionary<string, string> parameters, string template, bool send, bool isDirectInvitation, string? ciscoLink=null);
    Task<APIResult> ParticipantsAsUser(List<ParicipantDto> participants, int addBy, string lang);
    Task<APIResult> ReNotifyParticipant(int id,string email, string lang);
    Task<APIResult> Delete(int id, int deletedBy, string lang);
    Task<APIResult> Update(int id, ParicipantDto dto, int UpdatedBy, string lang);
    Task<APIResult> Liberate(int participantId, int updatedBy, string lang);
    Task<APIResult> UpdateNote(int id, string note, int updatedBy, string lang);

}

