using Microsoft.AspNetCore.Identity;
using VideoProjectCore6.DTOs.RoleDto;

namespace VideoProjectCore6.Repositories.IRoleRepository
{
    public interface IRoleRepository
    {
        Task<List<RoleGetDto>> GetAllRoles(string lang);
        Task<IdentityResult> CreateRoleAsync(RolePostDto rolePostDto, string lang);
        Task CreateRoleWithPermissionsAsync(RoleWithPermissionsPostDto rolePostDto, string lang);
        Task<RolePermissionsGetDTO> GetRolePermissions(int roleId, string lang);
        Task<IdentityResult> DeleteRoleAsync(int roleId, string lang);
        Task<int> UpdateRole(RolePostDto rolePostDto, int roleId, string lang);
        Task<IdentityResult> AddUpdateActionToRoles(int actionId, List<int> roles);
        Task<IdentityResult> AddUpdatePnsActionToRoles(int actionId, List<int> roles);
        Task<IdentityResult> AddUpdatePermissionsToRole(RolePermissionsPostDTO rolePermissionsDTO, string lang);

        Task<List<int>> GetRolesIdByUserId(int userId);

        Task<IdentityResult> CreateHardCodedRoleAsync(string roleName);
    }
}
