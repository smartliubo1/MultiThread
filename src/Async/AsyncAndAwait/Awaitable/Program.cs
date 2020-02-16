using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Awaitable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task.Run(async () => await AsyncProcessing());

            Thread.Sleep(TimeSpan.FromSeconds(20));
        }

        async static Task AsyncProcessing()
        {
            var sync = new CustomAwaitable(true);

            string result = await sync;
            Console.WriteLine(result);

            var async = new CustomAwaitable(false);

            result = await async;
            Console.WriteLine(result);
        }

        class CustomAwaitable
        {
            private readonly bool _completeSynchronously;

            public CustomAwaitable(bool completeSynchronously)
            {
                this._completeSynchronously = completeSynchronously;
            }

            public CustomAwaiter GetAwaiter()
            {
                return new CustomAwaiter(_completeSynchronously);
            }
        }

        class CustomAwaiter : INotifyCompletion
        {
            private string _result = "Completed Synchronously";
            private readonly bool _completeSynchronously;

            public bool IsCompleted { get { return _completeSynchronously; } }

            public CustomAwaiter(bool completeSynchronously)
            {
                this._completeSynchronously = completeSynchronously;
            }

            public string GetResult() { return _result; }

            public void OnCompleted(Action continuation)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _result = GetInfo();

                    if (continuation != null)
                        continuation();
                });
            }

            private string GetInfo()
            {
                return $"Task is running on a thread id{Thread.CurrentThread.ManagedThreadId}." +
                    $"Is thread pool thread :{Thread.CurrentThread.IsThreadPoolThread}";
            }
        }
    }
}
