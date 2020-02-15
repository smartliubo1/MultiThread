using System;
using System.Diagnostics;
using System.Threading;

namespace ThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Use ThreadPool's Thread instead of creating new Thread!");

            const int numberOfOperations = 500;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            UseThreads(numberOfOperations);
            stopWatch.Stop();

            Console.WriteLine($"Execution time using threads:{stopWatch.ElapsedMilliseconds}");

            stopWatch.Reset();

            stopWatch.Start();
            UseThreadPool(numberOfOperations);
            stopWatch.Stop();

            Console.WriteLine($"Execution time using threads:{stopWatch.ElapsedMilliseconds}");
        }

        static void UseThreads(int numberOfOperations)
        {
            using (var countdown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Scheduling work by creating threads.");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    var thread = new Thread(() =>
                    {
                        Console.WriteLine($"Current Thread Id is:{Thread.CurrentThread.ManagedThreadId}");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();
                    });
                    thread.Start();
                }
                countdown.Wait();
                Console.WriteLine();
            }
        }

        static void UseThreadPool(int numberOfOperations)
        {
            using (var countdown = new CountdownEvent(numberOfOperations))
            {
                Console.WriteLine("Scheduling work by creating threads.");
                for (int i = 0; i < numberOfOperations; i++)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Console.WriteLine($"Current Thread Id is:{Thread.CurrentThread.ManagedThreadId}");
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                        countdown.Signal();

                    });
                }
                countdown.Wait();
                Console.WriteLine();
            }
        }
    }
}
