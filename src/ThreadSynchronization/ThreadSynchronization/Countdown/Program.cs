using System;
using System.Threading;

namespace Countdown
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Starting two operations");

            var thread1 = new Thread(()=>PerformOperation("Operation 1 is completed",1));

            var thread2 = new Thread(() => PerformOperation("Operation 2 is completed", 10));

            thread1.Start();
            thread2.Start();
            _countdownEvent.Wait();//wait for the specified thread count complete,
            Console.WriteLine("Both operations have been completed ");
            _countdownEvent.Dispose();
        }

        static CountdownEvent _countdownEvent = new CountdownEvent(3);
        
        static void PerformOperation(string message,int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine(message);
            _countdownEvent.Signal();
        }
    }
}
