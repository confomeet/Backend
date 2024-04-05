using AspNetCoreRateLimit;

namespace VideoProjectCore6.Utility.APIRateLimit
{
    public class ClientQueryStringResolveContributor : IClientResolveContributor
    {
        private IHttpContextAccessor httpContextAccessor;

        public ClientQueryStringResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            var request = httpContextAccessor.HttpContext?.Request.Headers["IP"];
          //  var queryDictionary =
                //Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(
                //    request.QueryString.ToString());
            if (!string.IsNullOrWhiteSpace(request.ToString()) && request.ToString() != "0")
            {
                return await Task.FromResult(request.ToString());
            }

            return await Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
