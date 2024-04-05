using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VideoProjectCore6.Utilities.ExternalAPI.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace VideoProjectCore6.Utilities.ExternalAPI
{ 
    public class ExternalAPIService : IExternalAPIService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _host;
        private readonly ILogger<ExternalAPIService> _logger;

        public ExternalAPIService(IConfiguration configuration, ILogger<ExternalAPIService> logger)
        {
            _host = configuration["MainSystemHost"] ?? "";
            _logger = logger;
        }

        public async Task<JsonResult> CallAsync<T, U>(string method, string endpoint, T content, List<KeyValuePair<string, string>> headers)
        {

            HttpClient _client = new HttpClient();

            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);


            _client.BaseAddress = new Uri(endpoint);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = new HttpResponseMessage();
            switch (method)
            {
                case "POST":
                    response = await _client.PostAsJsonAsync(
                                    endpoint,
                                    content);
                    break;

                //case "GET":
                //    response = await _client.GetFJsonAsync(
                //                    _host + endpoint,
                //                    content);
                //    break;
            }


            var responseString = await response.Content.ReadAsStringAsync();
            
            return new JsonResult(JsonConvert.DeserializeObject<U>(responseString));
        }

        public async Task<JsonResult> CallAsync<U>(string method, string endpoint, List<KeyValuePair<string, string>> headers)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);

            HttpResponseMessage response = new HttpResponseMessage();
            switch (method)
            {
                case "GET":
                    response = await _client.GetAsync(
                                    endpoint);
                    break;

                case "DELETE":
                    response = await _client.DeleteAsync(
                                    endpoint);
                    break;
            }

            var responseString = await response.Content.ReadAsStringAsync();

            return new JsonResult(JsonConvert.DeserializeObject<U>(responseString))
                { StatusCode = (int)response.StatusCode };
        }
        
        public async Task<HttpResponseMessage> CallAsync(string method, string endpoint, List<KeyValuePair<string, string>> headers)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);

            HttpResponseMessage response = new HttpResponseMessage();
            switch (method)
            {
                case "GET":
                    response = await _client.GetAsync(
                        _host + endpoint);
                    break;
            }

            return response;
        }


        public async Task<JsonResult> PostFormAsync<U>(string method, string endpoint, IFormFile file, string pParty_id)
        {

            string boundary = Guid.NewGuid().ToString();

            HttpClient _client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();

            MultipartFormDataContent multipart = new MultipartFormDataContent(boundary)
            {
                {new StringContent(pParty_id),"pParty_id"}
            };


            multipart.Headers.Remove("Content-Type");
            multipart.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);



            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _client.DefaultRequestHeaders.Add("Accept", "*/*");
            _client.DefaultRequestHeaders.Add("User-Agent", "C# app");


            


            var body = new StringContent(pParty_id);

            multipart.Add(body, "pParty_id");

            if(file != null)
            {
                var fileContent =  new StreamContent(file.OpenReadStream())
                {
                    Headers = {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                 }
                };

                multipart.Add(fileContent, ":body", file.FileName);
            }
            

            body.Headers.ContentDisposition = new ContentDispositionHeaderValue("pParty_id");

            switch (method)
            {
                case "POST":
                    response = await _client.PostAsync(
                       endpoint, multipart);
                    break;
            }

            var responseString = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("########################## "+responseString);

            response.EnsureSuccessStatusCode();
            _client.Dispose();

            return new JsonResult(JsonConvert.DeserializeObject<U>(responseString))
            { StatusCode = (int) response.StatusCode };
        }


    }
}
