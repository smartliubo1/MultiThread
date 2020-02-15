using System;
using System.Threading;

namespace AsyncOperationInPool
{
    class Program
    {
        /// <summary>
        /// reuse the thread by threadpooling
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const int x = 1;
            const int y = 2;
            const string lambdaState = "lambda state 2";
            ThreadPool.QueueUserWorkItem(AsyncOperation, "async state");

            Thread.Sleep(TimeSpan.FromSeconds(1));

            ThreadPool.QueueUserWorkItem(state =>
            {
                Console.WriteLine($"Operation State:{state ?? (null)}");
                Console.WriteLine($"Work thread is:{Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"Is Current Thread in ThreadPool:{Thread.CurrentThread.IsThreadPoolThread}");
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }, "lambda state");

            Thread.Sleep(TimeSpan.FromSeconds(1));

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"Operation state:{x + y},{lambdaState}");
                Console.WriteLine($"Work thread is:{Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine($"Is Current Thread in ThreadPool:{Thread.CurrentThread.IsThreadPoolThread}");
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }, "lambda state");

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private static void AsyncOperation(object state)
        {
            Console.WriteLine($"Operation State:{state ?? (null)}");
            Console.WriteLine($"Work thread is:{Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"Is Current Thread in ThreadPool:{Thread.CurrentThread.IsThreadPoolThread}");

            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }

    }
}
