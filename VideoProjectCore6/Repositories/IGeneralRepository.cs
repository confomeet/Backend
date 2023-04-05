
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.SysLookUpDtos;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories
{
    public interface IGeneralRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> Save();

        Task<bool> FindLanguage(string lang);

        Task<bool> FindShortCut(string ShortCut);

        string GenerateShortCut(string tableName, string columnName);
         string Base64Decode(string base64EncodedData);

         string Base64Encode(string plainText);

        Task<AddNewTransResult> insertTrans(int inserttype, string target, string lang, string value);

        //  Task<SysTranslation> updateTranslation(string lang, string value, string shortCut);

        Task<int> DeleteTranslation(string shortCut);

        EventStatus CheckStatus(DateTime startDate, DateTime endDate, int id, string meetingId, string lang, List<ConfEvent> allRooms);
        Task<AddNewTransResult> insertTranslation(string shortcut, string value, string currentLanguage);

        Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary);


        Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, List<Dictionary<string, string>> TranslationDictionary);

        Task<List<SysTranslation>> insertListTranslationAsync(List<TranslationDto> TranslationList);

        Task<List<Dictionary<string, string>>> insertDictionaryTranslationAsync(string shortCut, List<Dictionary<string, string>> TranslationDictionary);

        Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut);
        //  Task<ClassDto> getShortCutId(string shortcut);

        int GetNewValueBySec();
        int GetNewValueForSMSBySec();
        int GetNextSecForPayment();

        Task<string> GetTranslateByShortCut(string lang, string shortCut);

        string getTranslateByIdFromLookUpValueId(string lang, int id);

        //  string SecureHashGeneration(string StringForHash);
        //string SecureHashDecryption(string StringForHash);
        int? CalculateFee(int serviceId);

        string GetDecviceInfo(string userAgent);
         Task<List<AllTranslationDto>> GetAllTranslation(string shortcut);
        public string GenerateURL(Dictionary<string, string> DictionaryQueryString, string URL);
        public string SecureHashGenerationHMACSHA256(string StringForHash, string Securekey);
        public string ConvertFromHTMLTpPDF(string HTML);


        Task<string> getServiceNameTranslateAsync(string lang, int? PaymentId);
        Task<string> getServiceNameTranslateByAppIdAsync(string lang, int? applicationId);

        int AppDevice(string userAgent);
        int TypeofSign(int id);

        string generateMeetingToken(User user, string meetingId, bool isAdmin);

        Task<DTOs.CommonDto.APIResult> GetContactToken(int userId);

        Task<List<Dictionary<string, string>>> AddNewLookUpType(List<Dictionary<string, string>> eventTypePostDTO, string type);

        Task<DTOs.CommonDto.APIResult> DeleteLookUpType(int id, string type);
        Boolean CheckParticipantStatus(string email, int id, string meetingId, List<ConfEvent> allRooms, List<ConfUser> allUsers);

        Task<Dictionary<int, string>> GetActions(string lang);

        public int GetNewValueBySec(string name);

        Task<DTOs.CommonDto.APIResult> EditLookUpType(List<Dictionary<string, string>> value, int id, string v);

        //Task UserNotifySignalR(HashSet<int?> userIds, string lang);
    }
}
