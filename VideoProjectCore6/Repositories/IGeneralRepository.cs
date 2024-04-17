
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories
{
    public interface IGeneralRepository
    {
        string GenerateShortCut(string tableName, string columnName);

         string Base64Encode(string plainText);

        Task<int> DeleteTranslation(string shortCut);

        EventStatus CheckStatus(DateTime startDate, DateTime endDate, int id, string? meetingId, string lang, List<ConfEvent> allRooms);
        Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary);
        Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut);
        int GetNewValueBySec();
        Task<string> GetTranslateByShortCut(string lang, string shortCut);
        Boolean CheckParticipantStatus(string email, int id, string? meetingId, List<ConfEvent> allRooms, List<ConfUser> allUsers);
        Task<Dictionary<int, string>> GetActions(string lang);
    }
}
