using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IAclRepository;

namespace VideoProjectCore6.Services.AclRepository
{
    public class AclRepository : IAclRepository
    {
        private readonly OraDbContext _OraDbContext;

        public AclRepository (OraDbContext oraDbContext)
        {
            _OraDbContext = oraDbContext;
        }

        public async Task<APIResult> GetACLs(string? name, string lang = "en")
        {
            APIResult result = new APIResult();

            try
            {
                var acls = await _OraDbContext.ACLs.Where(e => string.IsNullOrWhiteSpace(name) || e.Name.Contains(name)).Select(w => new AclsGetDto
                {
                    Id = w.Id,
                    Name = w.Name,
                }).ToListAsync();


                return result.SuccessMe(1, "Success", false, APIResult.RESPONSE_CODE.OK, acls);
            }

            catch
            {
                return result.FailMe(-1, "Failed to get ACLs");
            }
        }
    }
}
