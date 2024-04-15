using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.Models;

namespace VideoProjectCore6.Utility.Authorization;

public class PermissionsBasedAuthorizationHandler(IServiceScopeFactory scopeFactory)
    : AuthorizationHandler<HasPermissionRequirement>
{
    readonly IServiceScopeFactory _serviceScopeFactory = scopeFactory;

    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<OraDbContext>();
        if (null == dbContext)
            throw new Exception("Cannot authorize. No access to database");

        List<string> assignedRoles = [];
        foreach (var claim in context.User.Claims.AsEnumerable()) {
            if (claim.Type == ClaimTypes.Role)
                assignedRoles.Add(claim.Value);
        }
        var hasPermission = await
            dbContext.Roles.Where(r => assignedRoles.Contains(r.Name ?? "")).Include(r => r.RolePermissions)
            .SelectMany(r => r.RolePermissions.Select(rp => rp.Name))
            .ContainsAsync(requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}
