using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Contracts;
using VideoProjectCore6.Attributes;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ContactDto;
using VideoProjectCore6.Repositories.IContactRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.DTOs.FileDto;

namespace VideoProjectCore6.Controllers.Contant
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _IContantRepository;
        private readonly IUserRepository _IUserRepository;

        public ContactController(IContactRepository iContantRepository, IUserRepository iUserRepository)
        {
            _IContantRepository = iContantRepository;
            _IUserRepository = iUserRepository;
        }

        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = 100_000_000_000)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ContactIndividualDto dto, [FromHeader] string lang, [FromForm] IFormFile attachment)
        {

            var result = await _IContantRepository.Add(dto, _IUserRepository.GetUserID(), attachment, lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }

        [TypeFilter(typeof(KeyAttribute))]
        [HttpPost("Remote")]
        public async Task<ActionResult> AddRemote([FromBody] ContactDto dto, [FromQuery] int userId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.Add(dto, userId, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public async Task<ActionResult> Contact([FromQuery] byte tabId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.Contacts(_IUserRepository.GetUserID(), tabId, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("MyContacts")]
        public async Task<ActionResult> MyContact([FromBody] SearchFilterDto searchFilterDto, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.AllMyContacts(searchFilterDto, _IUserRepository.GetUserID(), lang));
        }

        [TypeFilter(typeof(KeyAttribute))]
        [HttpGet("Remote")]
        public async Task<ActionResult> MyContactRemote([FromQuery] int userId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.MyContact(userId, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("ContactById")]
        public async Task<ActionResult> ContactById(int Id, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.ContactById(Id, _IUserRepository.GetUserID(), lang));
        }

        [TypeFilter(typeof(KeyAttribute))]
        [HttpPut("Remote/{id}")]
        public async Task<ActionResult> RemotUpdate(int id, [FromBody] ContactDto dto, [FromQuery] int userId, [FromHeader] string lang = "ar")
        {
            return Ok(await _IContantRepository.Update(id, dto, userId, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ContactDto dto, [FromHeader] string lang = "ar")
        {
            var result = await _IContantRepository.Update(id, dto, _IUserRepository.GetUserID(), lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _IContantRepository.Delete(id, _IUserRepository.GetUserID(), lang);
            return result.Id > 0 ? Ok(result) : BadRequest(result);
        }
        [TypeFilter(typeof(KeyAttribute))]
        [HttpDelete("Remote/{id}")]
        public async Task<ActionResult> RemoteDelete(int id, [FromQuery] int userId, [FromHeader] string lang)
        {
            return Ok(await _IContantRepository.Delete(id, userId, lang));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Search")]
        public async Task<IActionResult> search([FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 25)
        {
            try
            {
                var result = await _IContantRepository.Search(_IUserRepository.GetUserID(), name, email, pageIndex, pageSize);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error getting users");
            }
        }

        [TypeFilter(typeof(KeyAttribute))]
        [HttpGet("Remote/Search")]
        public async Task<ActionResult> search_(int userId, string toSearch, [FromHeader] string lang)
        {
            return Ok(await _IContantRepository.Search(userId, toSearch));
        }

        [HttpPost("PrivateJoin/{meetingId}/{hash}")]
        public async Task<IActionResult> joinTo([FromRoute] string meetingId, [FromRoute] string hash, [FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.MeetingJWT(meetingId, hash, lang);
            return Ok(obj);
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GenerateDirectContactUrl")]
        public async Task<ActionResult> GenerateCurrentUserUrl()
        {
            var result = await _IContantRepository.GenerateDirectContactUrl(_IUserRepository.GetUserID());

            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        /// <summary>
        /// ////////////////////////Section/Company search//////////////////////////
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Sections")]
        public async Task<IActionResult> Sections([FromQuery] string text, [FromQuery] int companyId, /*[FromHeader] int pageIndex = 1, [FromHeader] int pageSize = 24,*/ [FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.SearchSections(_IUserRepository.GetUserID(), companyId, text, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Companies")]
        public async Task<IActionResult> Companies([FromQuery] string text, /*[FromHeader] int pageIndex = 1, [FromHeader] int pageSize = 24, */[FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.SearchCompanies(_IUserRepository.GetUserID(), text, lang);
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Managers")]
        public async Task<IActionResult> Companies([FromQuery] string text, [FromQuery] int companyId,/*[FromHeader] int pageIndex = 1, [FromHeader] int pageSize = 24, */[FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.SearchDirectManagers(_IUserRepository.GetUserID(), companyId, text, lang);
            return Ok(obj);
        }



        //////// Upper tabs /////////////////
        ///
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Tabs")]
        public async Task<IActionResult> ContactClasses([FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.ContactClasses(lang);
            return Ok(obj);
        }


        //////// Upload Photo related to contact /////////////////
        ///
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ContactPhoto")]
        public async Task<IActionResult> ContactPhoto([FromQuery] int contactId, [FromForm] FilePostDto filePostDto, [FromHeader] string lang = "ar")
        {
            object obj = await _IContantRepository.EditProfilePhoto(contactId, filePostDto, false, false, lang);
            return Ok(obj);
        }


    }
}
