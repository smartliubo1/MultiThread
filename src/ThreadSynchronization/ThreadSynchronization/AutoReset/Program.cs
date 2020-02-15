using System;
using System.Threading;

namespace AutoReset
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Thread Notifications!");

            var thread = new Thread(() => Process(10));
            thread.Start();

            Console.WriteLine("Waiting for another thread to complete work");

            _workEvent.WaitOne();
            Console.WriteLine("First Operation is  Completed");

            Console.WriteLine("Performing an operation on  a main thread");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            _mainEvent.Set();

            Console.WriteLine("Now running the second operation on a second thread");

            _workEvent.WaitOne();

            Console.WriteLine("Second Operation is completed");
        }

        private static AutoResetEvent _workEvent = new AutoResetEvent(false);

        private static AutoResetEvent _mainEvent = new AutoResetEvent(false);

        static void Process(int seconds)
        {
            Console.WriteLine("Starting  a long running work .........");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("Work is done");
            _workEvent.Set();

            Console.WriteLine("Waiting for a main thread to complete its work");
            _mainEvent.WaitOne();
            Console.WriteLine("Starting second operation...");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("Work is Done");
            _workEvent.Set();
        }
    }
}
