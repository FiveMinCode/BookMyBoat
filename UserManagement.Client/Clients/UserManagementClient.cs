using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using UserManagement.Client.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace UserManagement.Client.Clients
{
    public class UserManagementClient : IUserManagementClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UserManagementClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<UserBookings>> GetAll(string accessToken)
        {
            var httpClient = _httpClientFactory.CreateClient("UserManagementClient");
            var request = new HttpRequestMessage(HttpMethod.Get, "/UserManagement/GetBookings");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return new List<UserBookings>();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserBookings>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
