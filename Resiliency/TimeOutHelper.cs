using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resiliency
{
    public static class TimeOutHelper
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                var delayTask = Task.Delay(timeout, cts.Token);
                var completedTask = await Task.WhenAny(task, delayTask);

                if (completedTask == delayTask)
                {
                    throw new TimeoutException($"The operation has timed out after {timeout.TotalMilliseconds} milliseconds.");
                }

                cts.Cancel();
                return await task;
            }
        }

        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                var delayTask = Task.Delay(timeout, cts.Token);
                var completedTask = await Task.WhenAny(task, delayTask);

                if (completedTask == delayTask)
                {
                    throw new TimeoutException($"The operation has timed out after {timeout.TotalMilliseconds} milliseconds.");
                }

                cts.Cancel();
                await task;
            }

        }
    }
}

//How to call
//try
//{
//    var result = await SomeAsyncOperation().WithTimeout(TimeSpan.FromSeconds(5));
//    // Continue processing result
//}
//catch (TimeoutException ex)
//{
//    // Handle timeout exception
//}
