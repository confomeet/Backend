using VideoProjectCore6.DTOs.CommonDto;


namespace VideoProjectCore6.Repositories.IAclRepository
{
    public interface IAclRepository
    {
        Task<APIResult> GetACLs(string? name, string lang = "en");
    }
}
