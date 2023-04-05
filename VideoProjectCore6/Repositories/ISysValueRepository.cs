using VideoProjectCore6.DTOs;
using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.Repositories
{
    public interface ISysValueRepository
    {
        Task<List<EventTypeValues>> GetTypeAll(string lang, string type);
        Task<Dictionary<int, string>> GetTypeAllDictionary_(string lang, string type);
        Task<Dictionary<int, string>> GetTypeAllDictionary(string lang, string type);
        Task<string> GetValueByShortcut(string shortcut, string lang);
        Task<int> GetIdByShortcut(string shortcut);
        Task<string> GetValueById(int id, string lang);
        Task<string> AddRecord(int id, string table_name, string field_name, int parent_id, string parent_field_name);
        Task<string> AddTranslation(TranslationDto translationDto);
        Task<List<CountryDto>> GetAllCountry(string lang);
        Task<Dictionary<int, string>> GetAllCountryDictionary(string lang);
    }
}
