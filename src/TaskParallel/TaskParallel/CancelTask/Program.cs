using System;
using System.Threading;
using System.Threading.Tasks;

namespace CancelTask
{
    class Program
    {
        /// <summary>
        /// task取消之后的任务状态也为RanToCompletion,因为取消也被task任务完成了对应的task操作
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Cancel the task!");

            var cts = new CancellationTokenSource();

            var longTask = new Task<int>(
                () => TaskMethod("Task 1", 10, cts.Token),
                cts.Token);
            Console.WriteLine(longTask.Status);
            cts.Cancel();
            Console.WriteLine("First task has been cancelled before execution");

            //Import:如果为两个task指定了一个CancellationTokenSource，
            //如果第一个task先取消,那么后续的task在执行时会造成InvalidOperationException
            //因为两个task共用了一个CancellationTokenSource
            //所以不能为两个任务指定使用同一个CancellationTokenSource
            cts.Dispose();
            cts = new CancellationTokenSource();//清理之前的值，然后重新赋值，才可以继续被其他任务使用
            
            longTask = new Task<int>(
               () => TaskMethod("Task 2", 10, cts.Token),
                cts.Token);
            longTask.Start();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }
            cts.Cancel();
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine(longTask.Status);
            }

            Console.WriteLine($"a task has been completed with result {longTask.Result} and status is {longTask.Status}");
        }

        private static int TaskMethod(string name, int seconds,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Task {name} is running on a thread Id:{Thread.CurrentThread.ManagedThreadId}.Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");

            for (int i = 0; i < seconds; i++)
            {
                Console.WriteLine($"{name} is running on time{i}");
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (cancellationToken.IsCancellationRequested)
                    return -1;
            }
            return 42 * seconds;
        }
    }
}
