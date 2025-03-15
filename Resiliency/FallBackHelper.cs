using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resiliency
{
    public class RetryHelper
    {
        public async Task<T> Retry<T>(Func<Task<T>> func, RetryMechanismOptions options)
        {
            int retryCount = options.RetryCount;
            TimeSpan interval = options.Interval;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return await func();
                }
                catch
                {
                    if (i == retryCount - 1) throw;
                    await Task.Delay(interval);
                }
            }

            return default;
        }
    }

    public class RetryMechanismOptions
    {
        public RetryPolicies RetryPolicies { get; set; }
        public int RetryCount { get; set; }
        public TimeSpan Interval { get; set; }

        public RetryMechanismOptions(RetryPolicies retryPolicies, int retryCount, TimeSpan interval)
        {
            RetryPolicies = retryPolicies;
            RetryCount = retryCount;
            Interval = interval;
        }
    }

    public enum RetryPolicies
    {
        Linear
    }

    public class CircuitBreakerOptions
    {
        public string Key { get; set; }
        public int ExceptionThreshold { get; set; }
        public int SuccessThresholdWhenCircuitBreakerHalfOpenStatus { get; set; }
        public TimeSpan DurationOfBreak { get; set; }

        public CircuitBreakerOptions(string key, int exceptionThreshold, int successThresholdWhenCircuitBreakerHalfOpenStatus, TimeSpan durationOfBreak)
        {
            Key = key;
            ExceptionThreshold = exceptionThreshold;
            SuccessThresholdWhenCircuitBreakerHalfOpenStatus = successThresholdWhenCircuitBreakerHalfOpenStatus;
            DurationOfBreak = durationOfBreak;
        }
    }

    public class CircuitBreakerHelper
    {
        private CircuitBreakerOptions _options;
        private CircuitBreakerStateStore _stateStore;

        public CircuitBreakerHelper(CircuitBreakerOptions options, CircuitBreakerStateStore stateStore)
        {
            _options = options;
            _stateStore = stateStore;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            if (_stateStore.IsOpen(_options.Key))
            {
                throw new Exception("Circuit breaker is open.");
            }

            try
            {
                var result = await func();
                _stateStore.RecordSuccess(_options.Key);
                return result;
            }
            catch
            {
                _stateStore.RecordFailure(_options.Key);
                throw;
            }
        }
    }

    public class CircuitBreakerStateStore
    {
        // Simplified implementation, details omitted
        public bool IsOpen(string key) => false;
        public void RecordSuccess(string key) { }
        public void RecordFailure(string key) { }
    }

    public class AsyncHelper
    {
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, string funcKey, Func<Task<T>> fallbackFunc = null)
        {
            try
            {
                // Retry mechanism
                RetryHelper retryHelper = new RetryHelper();
                RetryMechanismOptions retryOptions = new RetryMechanismOptions(RetryPolicies.Linear, 3, TimeSpan.FromSeconds(5));

                return await retryHelper.Retry(func, retryOptions);
            }
            catch
            {
                try
                {
                    // Circuit breaker mechanism
                    CircuitBreakerOptions circuitBreakerOptions = new CircuitBreakerOptions(funcKey, 5, 5, TimeSpan.FromMinutes(5));
                    CircuitBreakerHelper circuitBreakerHelper = new CircuitBreakerHelper(circuitBreakerOptions, new CircuitBreakerStateStore());

                    return await circuitBreakerHelper.ExecuteAsync(func);
                }
                catch (Exception)
                {
                    if (fallbackFunc != null)
                    {
                        return await fallbackFunc();
                    }

                    throw;
                }
            }
        }
    }

    // How to call
    public class Caller
    {
        public async Task<string> PrimaryHttpOperation()
        {
            using (HttpClient client = new HttpClient())
            {
                // Replace with your actual API URL
                string apiUrl = "https://api.example.com/data";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
                }
            }
        }

        public async Task<string> FallbackHttpOperation()
        {
            // Simulating fallback data
            await Task.Delay(1000);
            return "Fallback data";
        }

        public async Task CallExecuteAsync()
        {
            AsyncHelper helper = new AsyncHelper();
            string funcKey = "http-operation-key";

            try
            {
                string result = await helper.ExecuteAsync(PrimaryHttpOperation, funcKey, FallbackHttpOperation);
                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
            }
        }
    }
}
