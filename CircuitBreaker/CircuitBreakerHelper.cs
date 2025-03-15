using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerHelper
    {
        private readonly CircuitBreakerOptions _circuitBreakerOptions;
        private readonly CircuitBreakerStateStore _stateStore;
        private readonly object _halfOpenSyncObject = new object();

        public CircuitBreakerHelper(CircuitBreakerOptions circuitBreakerOptions, CircuitBreakerStateStore stateStore)
        {
            _circuitBreakerOptions = circuitBreakerOptions;
            _stateStore = stateStore;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            if (!IsClosed(_circuitBreakerOptions.Key))
            {
                if (_stateStore.GetLastStateChangedDateUtc(_circuitBreakerOptions.Key).Add(_circuitBreakerOptions.DurationOfBreak) < DateTime.UtcNow)
                {
                    bool lockTaken = false;
                    try
                    {
                        Monitor.TryEnter(_halfOpenSyncObject, ref lockTaken);
                        if (lockTaken)
                        {
                            HalfOpen(_circuitBreakerOptions.Key);

                            var result = await func.Invoke();

                            Reset(_circuitBreakerOptions.Key);

                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trip(_circuitBreakerOptions.Key, ex);
                        throw;
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(_halfOpenSyncObject);
                        }
                    }
                }

                throw new Exception("Circuit breaker timeout hasn't yet expired.", _stateStore.GetLastException(_circuitBreakerOptions.Key));
            }

            try
            {
                var result = await func.Invoke();

                return result;
            }
            catch (Exception ex)
            {
                Trip(_circuitBreakerOptions.Key, ex);
                throw;
            }
        }

        private bool IsClosed(string key)
        {
            return _stateStore.IsClosed(key);
        }

        private void HalfOpen(string key)
        {
            _stateStore.ChangeState(key, CircuitBreakerStateEnum.HalfOpen);
            _stateStore.ChangeLastStateChangedDateUtc(key, DateTime.UtcNow);
        }

        private void Reset(string key)
        {
            _stateStore.IncreaseSuccessAttemp(key);

            if (_stateStore.GetSuccessAttempt(key) >= _circuitBreakerOptions.SuccessThresholdWhenCircuitBreakerHalfOpenStatus)
            {
                _stateStore.RemoveState(key);
            }
        }

        private void Trip(string key, Exception ex)
        {
            _stateStore.IncreaseExceptionAttemp(key);

            if (_stateStore.GetExceptionAttempt(key) >= _circuitBreakerOptions.ExceptionThreshold)
            {
                _stateStore.SetLastException(key, ex);
                _stateStore.ChangeState(key, CircuitBreakerStateEnum.Open);
                _stateStore.ChangeLastStateChangedDateUtc(key, DateTime.UtcNow);
            }
        }
    }
}
