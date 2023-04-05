using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace VideoProjectCore6.Utility.APIRateLimit
{
    public class CustomRateLimitConfiguration : RateLimitConfiguration
    {
        IHttpContextAccessor _httpContextAccessor;
        
        public CustomRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
                : base(ipOptions, clientOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void RegisterResolvers()
        {
            ClientResolvers.Add(new ClientQueryStringResolveContributor(_httpContextAccessor));
        }
    }
}
