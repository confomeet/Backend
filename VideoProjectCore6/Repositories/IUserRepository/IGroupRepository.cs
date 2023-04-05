using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;

namespace VideoProjectCore6.Repositories.IUserRepository
{
    public interface IGroupRepository
    {
        Task<ListCount> GetGroups(string text, string name, string lang, int pageSize, int pageIndex);

        Task<APIResult> AddGroup(UserGroupPostDto userGroupPostDto, int createdBy);

        Task<APIResult> EditGroup(UserGroupPostDto userGroupPostDto, int groupId, int updatedBy);
        Task<APIResult> DeleteGroup(int groupId, string lang);
        Task<APIResult> AddUsersToGroup(GroupPostUserDto groupPostUserDto, string lang);
        Task<APIResult> RemoveUsersFromGroup(GroupPostUserDto groupPostUserDto, string lang);

        Task<ListCount> GetUsersByGroupId(int groupId, int? pageIndex, int? pageSize);
    }
}
