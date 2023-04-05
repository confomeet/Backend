using Microsoft.AspNetCore.Mvc;

namespace VideoProjectCore6.Utilities.ExternalAPI.Interfaces
{
    public interface IExternalAPIService
    {   
        Task<JsonResult> CallAsync<T, U>(string method, string endpoint, T content, List<KeyValuePair<string, string>> headers);

        Task<JsonResult> CallAsync<U>(string method, string endpoint, List<KeyValuePair<string, string>> headers);

        Task<HttpResponseMessage> CallAsync(string method, string endpoint, List<KeyValuePair<string, string>> headers);

        Task<JsonResult> PostFormAsync<U>(string method, string endpoint, IFormFile file, string partyId);

        //Task<string> PostURI(Uri u, HttpContent c);
    }
}
