using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;

namespace MsaasBackend.Tests.Utils
{
    public static class MockExtensions
    {
        public static async Task VerifyWithTimeoutAsync<T>(this Mock<T> mock, Expression<Action<T>> expression,
            Times times, int timeoutInMs = 5000, int retryInterval = 100)
            where T : class
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                try
                {
                    mock.Verify(expression, times);
                    return;
                }
                catch (MockException)
                {
                    if (stopwatch.ElapsedMilliseconds > timeoutInMs) throw;
                }

                await Task.Delay(retryInterval);
            }
        }
    }
}