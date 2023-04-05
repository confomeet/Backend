using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.TabDto;

namespace VideoProjectCore6.Repositories.ITabRepository
{
    public interface ITabRepository
    {
        Task<APIResult> AddTab(TabPermPostDto tabPostDto);
        Task<APIResult> UpdateTab(TabPermPostDto tabPostDto, int rowId);
        Task<APIResult> DeleteTab(int id, string lang);
        Task<List<TabGetDto>> GetTabs(string lang);
        Task<List<UserTabGetDTO>> GetTabsByIds(List<int> tabIds, string lang);
        Task<List<UserTabGetDTO>> GetMyTabs(int userId, string lang);
        Task<int> UpdateIconTab(IFormFile IconImage, int rowId);
        Task<int> UpdateIconStringTab(string iconString, int rowId);
    }
}
