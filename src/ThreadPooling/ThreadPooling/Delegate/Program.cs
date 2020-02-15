using System;
using System.Threading;

namespace Delegate
{
    class Program
    {

        /// <summary>
        /// Asynchronous Programming Model,APM
        /// .net core目前并不支持委托的异步委托,
        /// 如果使用BeginInvoke则会造成PlatformNotSupportException
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int threadId = 0;

            RunOnThreadPool poolDelegate = Test;

            var thread = new Thread(() => Test(out threadId));
            thread.Start();
            thread.Join();

            Console.WriteLine($"Thread id:{threadId}");

            //.net core目前不支持委托的异步执行
            //与此类型的是Thread.Abort(),目前.NET CORE平台也不支持
            //初步猜测有可能是为了兼容跨平台而导致了以前的部分功能不可使用
            //IAsyncResult asyncResult = poolDelegate.BeginInvoke(out threadId,
            //    new AsyncCallback(Callback), "a delegate asynchronous call");
            //asyncResult.AsyncWaitHandle.WaitOne();
            //string result = poolDelegate.EndInvoke(out threadId, asyncResult);

            string result = poolDelegate.Invoke(out threadId);

            Console.WriteLine($"Thread Pool work thread id:{threadId}");

            Console.WriteLine(result);

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private delegate string RunOnThreadPool(out int threadId);

        private static void Callback(IAsyncResult asyncResult)
        {
            Console.WriteLine("Starting a callback......");
            Console.WriteLine($"State passed to a callback {asyncResult.AsyncState}");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Thread Pool worker thread id:{Thread.CurrentThread.ManagedThreadId}");
        }

        private static string Test(out int threadId)
        {
            Console.WriteLine("Starting a callback......");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;
            return $"Thread Pool work thread id was:{threadId}";
        }

    }
}
