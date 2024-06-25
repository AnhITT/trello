using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic_LayerDataAccess_Layer.Common
{
    public class CallApi
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CallApi(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _configuration = configuration;
        }
       
        public async Task<bool> IsNotFindTask(Guid idTask)
        {
            try
            {
                string UrlApi = _configuration["UrlApi"];
                var token = GetAccessToken();
                string url = $"{UrlApi}taskcard/CheckTaskCard?idTask={idTask}";

                if (string.IsNullOrEmpty(token))
                    return false;

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return bool.Parse(result);
                }
                else
                {
                    return false;
                }    
            }
            catch (HttpRequestException e)
            {
                return false;
            }
        }

        public string GetAccessToken()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null && context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
