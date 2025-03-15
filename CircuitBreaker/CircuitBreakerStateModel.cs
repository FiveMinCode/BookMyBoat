using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreakerStateModel
    {
        public CircuitBreakerStateEnum State { get; set; }
        public int ExceptionAttempt { get; set; }
        public int SuccessAttempt { get; set; }
        public Exception LastException { get; set; }
        public DateTime LastStateChangedDateUtc { get; set; }
        public bool IsClosed { get; set; }
    }
}
