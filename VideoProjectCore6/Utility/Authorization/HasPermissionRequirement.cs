using Microsoft.AspNetCore.Authorization;

namespace VideoProjectCore6.Utility.Authorization;

public class HasPermissionRequirement : IAuthorizationRequirement
{
    public HasPermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
};
