using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VideoProjectCore6.Models;
using VideoProjectCore6.Utility;
using Microsoft.EntityFrameworkCore;
#nullable disable
namespace VideoProjectCore6.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class KeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "key";
        private const string TIMENAME = "timestamp";
        private const string APPNAME = "appId";

        private readonly OraDbContext _DbContext;


        public KeyAttribute(OraDbContext dbContext)
        {
            _DbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(TIMENAME, out var timestamp) && context.HttpContext.Request.Headers.TryGetValue(APPNAME, out var appId) && context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var key))
            {
                var qs = context.HttpContext.Request.QueryString.ToString();
                if (qs.Length > 0 && qs.StartsWith("?"))
                {
                    qs = qs.Remove(0, 1);
                }
                //IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                //var apiKeys = configuration.GetSection("Keys").Get<Dictionary<string, string>>();
                //string apiKey = apiKeys[appId];
                ClientInfo client=null;
                try 
                {   
                   client = await _DbContext.ClientInfos.Where(x => x.AppName == appId.ToString()).FirstOrDefaultAsync();
                }
                catch (Exception e) 
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 500,
                        Content = "Error occurred!"
                    };
                    return;
                };
                if (client == null)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Incorrect credential"
                    };
                    return;
                }
                if (!client.IsActive)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Inactive key"
                    };
                    return;
                }
                string apiKey = SecretHMAC.Base64Decode(client.AppKey);//client.AppKey
                string toEncode = string.Concat(qs, appId, timestamp);
                if (SecretHMAC.Sign(toEncode, apiKey) != key)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Api Key is not valid"
                    };
                    return;
                }
            }
            else
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Missing data"
                };
                return;
            }
            await next();
        }
    }
}
