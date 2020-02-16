using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace EAPToTask
{
    class Program
    {
        /// <summary>
        /// 使用TaskCompletionSource<T>将EAP模式转换为任务.
        /// 在设置任务结果时需要使用try/catch捕获设置结果中出现的异常情况.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var taskCompletionSource = new TaskCompletionSource<int>();

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, eventArgs) =>
            {
                var backgroundWorker = (BackgroundWorker)sender;
                for (int i = 0; i < 100; i++)
                {
                    backgroundWorker.ReportProgress(i);
                }
                eventArgs.Result = TaskMethod("Background Worker", 5);
            };

            worker.ProgressChanged += (sender,eventArgs) => {

                Console.WriteLine(eventArgs.ProgressPercentage+"% completed.");
            };

            worker.RunWorkerCompleted += (sender, eventArgs) => {
                if (eventArgs.Error!=null)
                {
                    taskCompletionSource.SetException(eventArgs.Error);
                }
                else if (eventArgs.Cancelled)
                {
                    taskCompletionSource.SetCanceled();
                }
                else
                {
                    taskCompletionSource.SetResult((int)eventArgs.Result);//设置EAP的执行结果至TaskCompletionSource<T>结果处,也可使用TrySetResult
                }
            };

            worker.RunWorkerAsync();

            int result = taskCompletionSource.Task.Result;

            Console.WriteLine($"Result is {result}");
        }

        static int TaskMethod(string name, int seconds)
        {
            Console.WriteLine($"Task {name} is running on the thread {Thread.CurrentThread.ManagedThreadId} ,Is thread pool:{Thread.CurrentThread.IsThreadPoolThread}");

            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            return 42 * seconds;
        }
    }
}
