using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetryMechanism
{
    public abstract class RetryMechanismBase
    {
        private readonly RetryMechanismOptions _retryMechanismOptions;

        public RetryMechanismBase(RetryMechanismOptions retryMechanismOptions)
        {
            _retryMechanismOptions = retryMechanismOptions;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            int currentRetryCount = 0;

            for (; ; )
            {
                try
                {
                    return await func.Invoke();
                }
                catch (Exception ex)
                {
                    currentRetryCount++;

                    bool isTransient = await IsTransient(ex);
                    if (currentRetryCount > _retryMechanismOptions.RetryCount || !isTransient)
                    {
                        throw;
                    }
                }

                await HandleBackOff();
            }
        }

        protected abstract Task HandleBackOff();

        private Task<bool> IsTransient(Exception ex)
        {
            bool isTransient = false;
            var webException = ex as WebException;

            if (webException != null)
            {
                isTransient = new[] {WebExceptionStatus.ConnectionClosed,
                            WebExceptionStatus.Timeout,
                            WebExceptionStatus.RequestCanceled,
                            WebExceptionStatus.KeepAliveFailure,
                            WebExceptionStatus.PipelineFailure,
                            WebExceptionStatus.ReceiveFailure,
                            WebExceptionStatus.ConnectFailure,
                            WebExceptionStatus.SendFailure}
                                .Contains(webException.Status);
            }

            return Task.FromResult(isTransient);
        }
    }
}
