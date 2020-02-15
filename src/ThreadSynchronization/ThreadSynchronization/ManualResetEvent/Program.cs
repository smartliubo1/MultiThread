using System;
using System.Threading;

namespace ManualResetEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ManualResetEventSlim Example!");
            var thread1 = new Thread(() => TravelThroughGates("Thread one",5));
            var thread2 = new Thread(() => TravelThroughGates("Thread two", 6));
            var thread3 = new Thread(() => TravelThroughGates("Thread three", 12));

            thread1.Start();
            thread2.Start();
            thread3.Start();

            Thread.Sleep(TimeSpan.FromSeconds(6));

            Console.WriteLine("The gates are now open!");

            _manualResetEventSlim.Set();

            Thread.Sleep(TimeSpan.FromSeconds(2));
            _manualResetEventSlim.Reset();

            Console.WriteLine("The gates has been closed!");

            Thread.Sleep(TimeSpan.FromSeconds(10));

            Console.WriteLine("The gates are now open for the second time!");

            _manualResetEventSlim.Set();

            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine("The gates has been closed again !");
            _manualResetEventSlim.Reset();

        }

        static ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim();

        static void TravelThroughGates(string threadName,int seconds)
        {
            Console.WriteLine($"{threadName} falls to sleep");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine($"{threadName} wait for the gates to open!");
            _manualResetEventSlim.Wait();
            Console.WriteLine($"{threadName} enter the gates!");
        }
    }
}
