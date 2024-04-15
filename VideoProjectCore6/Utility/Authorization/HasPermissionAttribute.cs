using Microsoft.AspNetCore.Authorization;

namespace VideoProjectCore6.Utility.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "HasPermission:";
    public HasPermissionAttribute(string permission)
        : base(PolicyPrefix + permission)
    {
    }

    public string Permission
    {
        get
        {
            return Policy!.Substring(PolicyPrefix.Length);
        }
        set
        {
            if (null == value)
                throw new ArgumentNullException("value", "Argument value of Permission.set(string value) is null");
            Policy = PolicyPrefix + value;
        }
    }
}
