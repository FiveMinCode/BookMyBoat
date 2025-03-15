using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public static class CircuitBreaker
    {

        public static async Task<T?> ExecuteAsync<T>(HttpClient httpClient, string apiUrl)
        {
            var options = new CircuitBreakerOptions(key: "CurrencyConverterSampleAPI",
                                        exceptionThreshold: 5,
                                        successThresholdWhenCircuitBreakerHalfOpenStatus: 5,
                                        durationOfBreak: TimeSpan.FromMinutes(5));

            CircuitBreakerHelper helper = new CircuitBreakerHelper(options, new CircuitBreakerStateStore());

            return await helper.ExecuteAsync(async () =>
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(apiUrl);
                httpResponse.EnsureSuccessStatusCode();
                string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse);
            });
        }
    }
}


// Usage example
//var postResponse = CircuitBreakerImplementation.ExecuteAsync<List<Post>>(httpClient, "https://jsonplaceholder.typicode.com/posts");

