using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.ICountryRepository;

namespace VideoProjectCore6.Services.CountryService
{
    public class CountryRepository : ICountryRepsository

    {

        private readonly OraDbContext _DbContext;


        public CountryRepository(OraDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task<ListCount> getAllCountries(string lang, int pageIndex = 1, int pageSize = 25)
        {
            
            
            var countries = await _DbContext.Countries.ToListAsync();
            
            int total = countries.Count();
            
            return new ListCount
            {
                Count = total,
                Items = countries.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };

        }
    }
}
