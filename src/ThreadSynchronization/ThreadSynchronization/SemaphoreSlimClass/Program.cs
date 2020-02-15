using System;
using System.Threading;

namespace SemaphoreSlimClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            for (int i = 1; i < 10; i++)
            {
                string threadName = $"Thread  {i}";

                int secondsToWait = 2 + 2 * i;

                var thread=new Thread(()=> AccessDatabase(threadName, secondsToWait));
                thread.Start();
            }
        }

        static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// use semaphoreslim to block too much thread access the same resources
        /// SemaphoreSlim can limit the access thread number 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        static void AccessDatabase(string name, int seconds)
        {
            Console.WriteLine($"{name} waits to access a database");

            _semaphoreSlim.Wait();

            Console.WriteLine($"{name} was granted to an access to a database");

            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            Console.WriteLine($"{name} is completed");

            _semaphoreSlim.Release();
        }
    }
}
