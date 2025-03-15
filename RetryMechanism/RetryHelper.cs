using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryMechanism
{
    public class RetryHelper
    {
        public async Task<T> Retry<T>(Func<Task<T>> func, RetryMechanismOptions retryMechanismOptions)
        {
            RetryMechanismBase retryMechanism = null;

            if (retryMechanismOptions.RetryPolicies == RetryPolicies.Linear)
            {
                retryMechanism = new RetryLinearMechanismStrategy(retryMechanismOptions);
            }

            return await retryMechanism.ExecuteAsync(func);
        }
    }
}
