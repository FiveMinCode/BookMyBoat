using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
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
}
