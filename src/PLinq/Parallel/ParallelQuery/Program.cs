using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parallel Tasks!");

            Parallel.Invoke(
                () => EmulateProcessing("Task1"),
                () => EmulateProcessing("Task2"),
                () => EmulateProcessing("Task3")
                );

            var cts = new CancellationTokenSource();

            var result = Parallel.ForEach(
                    Enumerable.Range(1,30),
                    new ParallelOptions { 
                        CancellationToken=cts.Token,
                        MaxDegreeOfParallelism=Environment.ProcessorCount,
                        TaskScheduler=TaskScheduler.Default
                    },
                    (i, state) =>
                    {
                        if (i==20)
                        {
                            state.Break();
                            Console.WriteLine($"Loop is Stopped:{state.IsStopped}");
                        }
                        Console.WriteLine(i+state.ToString());
                    }
                );
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"IsCompleted:{result.IsCompleted}");

            Console.WriteLine($"Lowest break iteration;{result.LowestBreakIteration}");
        }

        static string EmulateProcessing(string taskName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(new Random(DateTimeOffset.Now.Millisecond).Next(250, 350)));
            Console.WriteLine($"{taskName} task was processed on a thread id {Thread.CurrentThread.ManagedThreadId}.Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            return taskName;
        }


    }
}
