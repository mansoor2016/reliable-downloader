using System;
using System.Threading.Tasks;

namespace ReliableDownloader.RetryPolicy
{
    public interface IRetryPolicy
    {
        public Task InvokeAsync(Func<Task> action);
    }
}
