using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.Repositories.ICountryRepository
{
    public interface ICountryRepsository
    {

        Task<ListCount> getAllCountries(string lang, int pageIndex = 1, int pageSize = 25);

    }
}
