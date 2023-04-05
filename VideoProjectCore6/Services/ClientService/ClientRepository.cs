using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.ClientDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IClientRepository;
using VideoProjectCore6.Utility;

namespace VideoProjectCore6.Services.ClientService
{
    public class ClientRepository : IClientRepository
    {
        private readonly OraDbContext _DbContext;
        public ClientRepository(OraDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task<APIResult> Add(ClientDto dto, int addBy, string lang)
        {
            APIResult result = new();
            var client = new ClientInfo
            {
                AppName = dto.AppName,
                ClientName = dto.ClientName,
                IsActive = dto.IsActive,
                Note = dto.Note,
                AppKey = SecretHMAC.Base64Encode(dto.AppKey)
            };
            try
            {
                _DbContext.ClientInfos.Add(client);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(client.Id, "Ok");
            }
            catch
            {
                return result.FailMe(-1, "Error");
            }
        }

        public async Task<List<ClientInfo>> GetAll(string lang)
        {
            return await _DbContext.ClientInfos.ToListAsync();
        }

        public async Task<List<ValueId>> GetView(string lang)
        {
            return await _DbContext.ClientInfos.Select(c => new ValueId { Id = c.Id, Value = c.ClientName }).ToListAsync();
        }

        public async Task<APIResult> SetActivation(ushort id, bool isActive, int UpdatedBy, string lang)
        {
            APIResult result = new();
            var client = await _DbContext.ClientInfos.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
            {
                return result.FailMe(-1, "Not found");
            }
            client.IsActive = isActive;
            try
            {
                _DbContext.ClientInfos.Update(client);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(client.Id, "Updated");
            }
            catch
            {
                return result.FailMe(-1, "Error");
            }
        }

        public async Task<APIResult> Update(ushort id, ClientDto dto, int UpdatedBy, string lang)
        {
            APIResult result = new();
            var client = await _DbContext.ClientInfos.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
            {
                return result.FailMe(-1, "Not found");
            }
            client.Note = dto.Note;
            client.AppName = dto.AppName;
            client.AppKey = SecretHMAC.Base64Encode(dto.AppKey);
            client.ClientName = dto.ClientName;
            client.IsActive = dto.IsActive;
            try
            {
                _DbContext.ClientInfos.Update(client);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(client.Id, "Updated");
            }
            catch
            {
                return result.FailMe(-1, "Error");
            }
        }
    }
}
