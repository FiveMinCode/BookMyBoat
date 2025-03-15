using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryMechanism
{
    internal class RetryLinearMechanismStrategy : RetryMechanismBase
    {
        private readonly RetryMechanismOptions _retryMechanismOptions;

        public RetryLinearMechanismStrategy(RetryMechanismOptions retryMechanismOptions)
            : base(retryMechanismOptions)
        {
            _retryMechanismOptions = retryMechanismOptions;
        }

        protected override async Task HandleBackOff()
        {
            await Task.Delay(_retryMechanismOptions.Interval);
        }
    }
}
