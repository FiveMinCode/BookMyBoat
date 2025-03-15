namespace Resiliency
{
    public class Bulkhead
    {
        private readonly SemaphoreSlim _semaphore;

        // This class uses a SemaphoreSlim to control the maximum number of concurrent requests.
        // The ExecuteAsync method acquires a slot, executes the provided function,
        // and releases the slot once the execution is complete.
        public Bulkhead(int maxConcurrentRequests)
        {
            _semaphore = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await func();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    // Example usage:
    public class HttpService
    {
        private readonly HttpClient _client;
        private readonly Bulkhead _bulkhead;

        public HttpService(int maxConcurrentRequests)
        {
            _client = new HttpClient();
            _bulkhead = new Bulkhead(maxConcurrentRequests);
        }

        public async Task<string> GetAsync(string url)
        {
            return await _bulkhead.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
                }
            });
        }
    }
