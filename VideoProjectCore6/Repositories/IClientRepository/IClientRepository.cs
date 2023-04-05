using VideoProjectCore6.DTOs.ClientDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories.IClientRepository
{
    public interface IClientRepository
    {
        Task<APIResult> Add(ClientDto dto, int addBy, string lang);
        Task<APIResult> Update(ushort id, ClientDto dto, int UpdatedBy, string lang);
        Task<APIResult> SetActivation(ushort id, bool isActive, int UpdatedBy, string lang);
        Task<List<ClientInfo>> GetAll(string lang);
        Task<List<ValueId>> GetView(string lang);
    }
}
