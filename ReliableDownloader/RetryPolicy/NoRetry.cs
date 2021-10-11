using System;
using System.Threading.Tasks;

namespace ReliableDownloader.RetryPolicy
{
    public class NoRetry : IRetryPolicy
    {
        public async Task InvokeAsync(Func<Task> action) => await action.Invoke();
    }
}
