using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APMToTask
{
    class Program
    {
        /// <summary>
        /// 使用Task<T>.Factory.FromAsync方法将APM转换为TPL,
        /// 转换成任务之后并不会立即执行任务,对应的任务状态是WaitingForActivation,
        /// TPL并未立即启动该任务
        ///                                                                                                                                                                                                                                      
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int threadId;
            AsynchronousTask asynchronousTask = Test;
            IncompatibleAsynchronousTask incompatibleAsynchronousTask = Test;

            Console.WriteLine("Option 1");
            Task<string> task1 = Task<string>.Factory.FromAsync(
                    asynchronousTask.BeginInvoke("AsyncTaskThread",Callback,"a delegate asynchronous call"),
                    asynchronousTask.EndInvoke
                );
            task1.ContinueWith(task => Console.WriteLine($"Callback is finished,now running a continuation! Result:{task.Result }"));

            while (!task1.IsCompleted)
            {
                Console.WriteLine(task1.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task1.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine("----------------------------");
            Console.WriteLine("Operation 2");

            task1 = Task<string>.Factory.FromAsync(
                    asynchronousTask.BeginInvoke,
                    asynchronousTask.EndInvoke,
                    "AsyncTaskThread",
                    "a delegate asynchronous call"
                );
            task1.ContinueWith(task => Console.WriteLine($"Task is Completed,now running a continuation! Result:{task.Result }"));

            while (!task1.IsCompleted)
            {
                Console.WriteLine(task1.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task1.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.WriteLine("----------------------------");
            Console.WriteLine("Operation 3");

            IAsyncResult asyncResult = incompatibleAsynchronousTask.BeginInvoke(
                out threadId,
                Callback,
                "a delegate asynchronous call"
                );
            //TPL执行APM中带有out参数的特殊情况,
            //需要使用IAsyncResult对象传入TPL参数中
            task1 = Task<string>.Factory.FromAsync(asyncResult,
                _=>incompatibleAsynchronousTask.EndInvoke(out threadId,asyncResult)
                );

            task1.ContinueWith(task => Console.WriteLine($"Task is Completed,now running a continuation! Result:{task.Result },ThreadId:{threadId}"));
            while (!task1.IsCompleted)
            {
                Console.WriteLine(task1.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(task1.Status);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Console.ReadLine();
        }

        private delegate string AsynchronousTask(string threadNam);

        private delegate string IncompatibleAsynchronousTask(out int threadId);

        private static void Callback(IAsyncResult asyncResult)
        {
            Console.WriteLine("Starting a call back.......");
            Console.WriteLine($"State passed to a callback:{asyncResult.AsyncState}");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Thread pool work thread id:{Thread.CurrentThread.ManagedThreadId}");
        }

        private static string Test(string threadName)
        {
            Console.WriteLine("String ................");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(2));

            Thread.CurrentThread.Name = threadName;
            return $"Thread name ;{Thread.CurrentThread.Name}";
        }

        private static string Test(out int threadId)
        {
            Console.WriteLine("Starting.........");
            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadId = Thread.CurrentThread.ManagedThreadId;

            return $"Thread Pool Worker thread id is:{threadId}";
        }
    }
}
