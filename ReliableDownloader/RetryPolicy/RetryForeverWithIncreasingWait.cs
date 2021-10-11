using Polly;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReliableDownloader.RetryPolicy
{
    public class RetryForeverWithIncreasingWait : IRetryPolicy
    {
        public async Task InvokeAsync(Func<Task> action) => await Policy
              .Handle<HttpRequestException>()
              .WaitAndRetryForeverAsync(
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timespan) =>
                {
                    Debug.WriteLine($"Retrying operation after {timespan} seconds.");
                })
              .ExecuteAsync(action);
    }
}
