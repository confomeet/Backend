using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace VideoProjectCore6.Utility.Authorization;

public class HasPermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(HasPermissionAttribute.PolicyPrefix))
        {
            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            policy.AddRequirements(new HasPermissionRequirement(policyName[HasPermissionAttribute.PolicyPrefix.Length..]));
            return Task.FromResult(policy.Build())!;
        }
        return base.GetPolicyAsync(policyName);
    }
}
