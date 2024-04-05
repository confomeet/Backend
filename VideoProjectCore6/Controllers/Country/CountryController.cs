using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.Repositories.ICountryRepository;

namespace VideoProjectCore6.Controllers.ConfEvent
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepsository _ICountryRepository;

        public CountryController(ICountryRepsository iCountryRepository)
        {
            _ICountryRepository = iCountryRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "ar")
        {
            return Ok(await _ICountryRepository.getAllCountries(lang, pageIndex, pageSize));
        }

    }
}
