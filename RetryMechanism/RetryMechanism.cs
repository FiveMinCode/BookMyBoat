using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetryMechanism
{
    public static class RetryMechanism
    {
        public static async Task<T?> ExecuteAsync<T>(HttpClient httpClient, string apiUrl)
        {
            RetryHelper retryHelper = new RetryHelper();
            return await retryHelper.Retry<T?>(async () =>
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(apiUrl);
                httpResponse.EnsureSuccessStatusCode();
                string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T?>(jsonResponse);
            }, new RetryMechanismOptions(retryPolicies: RetryPolicies.Linear,
                             retryCount: 3,
                             interval: TimeSpan.FromSeconds(5)));
        }
    }
}

// Usage example
//var postResponse = RetryMechanismUse.ExecuteAsync<List<Post>>(httpClient, "https://jsonplaceholder.typicode.com/posts");
