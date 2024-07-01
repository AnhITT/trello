using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using BusinessLogic_Layer.Entity;
using DataAccess_Layer.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
                var token = GetAccessToken();

                if (string.IsNullOrEmpty(token))
                    return false;

                string UrlApi = _configuration["UrlApi"];
                string url = $"{UrlApi}taskcard/CheckTaskCard?idTask={idTask}";
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
        public async Task<List<AttachmentFile>> GetFilesForTask(Guid taskId)
        {
            using (var client = new HttpClient())
            {
                //var token = GetAccessToken();

                //if (string.IsNullOrEmpty(token))
                //    return null;

                string UrlApi = _configuration["UrlUploadApi"];
                string url = $"{UrlApi}FileUpload/GetFileToTask?idTask={taskId}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResultObject>(resultString);

                    if (result.Success)
                    {
                        return JsonConvert.DeserializeObject<List<AttachmentFile>>(result.Data.ToString());
                    }
                }

                return new List<AttachmentFile>();
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
