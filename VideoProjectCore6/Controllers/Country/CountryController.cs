using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ConfEventDto;
using VideoProjectCore6.DTOs.ContactDto;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IContactRepository;
using VideoProjectCore6.Repositories.ICountryRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;

namespace VideoProjectCore6.Controllers.ConfEvent
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepsository _ICountryRepository;
        private readonly IUserRepository _IUserRepository;

        public CountryController(ICountryRepsository iCountryRepository, IUserRepository iUserRepository)
        {
            _ICountryRepository = iCountryRepository;
          //  _IUserRepository = iUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromHeader] string lang = "ar")
        {
            return Ok(await _ICountryRepository.getAllCountries(lang, pageIndex, pageSize));
        }

    }
}
