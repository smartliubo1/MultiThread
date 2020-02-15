using System;
using System.Threading;
using System.Diagnostics;

namespace ThreadPriority
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine($"Current Thread Priority {0}", Thread.CurrentThread.Priority);

            Console.WriteLine("Runing all cores");
            RunThreads();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            Console.WriteLine("Running a single core");
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            RunThreads();
        }

        static void RunThreads()
        {
            var sample = new ThreadSample();

            var threadOne = new Thread(sample.CountNumbers);
            threadOne.Name = "Thread One";
            var threadTwo = new Thread(sample.CountNumbers);
            threadTwo.Name = "Thread Two";

            threadOne.Priority = System.Threading.ThreadPriority.Highest;

            threadTwo.Priority = System.Threading.ThreadPriority.Lowest;

            threadOne.Start();
            threadTwo.Start();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            sample.Stop();

        }

        class ThreadSample
        {
            private bool _isStopped = false;

            public void Stop()
            {
                _isStopped = true;
            }

            public void CountNumbers()
            {
                long counter = 0;
                while (!_isStopped)
                {
                    counter++;
                }
                Console.WriteLine("{0} with {1} priority  has a count={2}",
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.Priority,
                    counter.ToString());
            }
        }
    }
}
