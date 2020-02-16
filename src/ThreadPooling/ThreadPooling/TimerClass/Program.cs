using System;
using System.Threading;

namespace TimerClass
{
    class Program
    {
        /// <summary>
        /// 使用Timer来实现线程池中周期性的调用
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Timer in thraed pool");
            DateTime start = DateTime.Now;
            _timer = new Timer(_=>TimerOperation(start),null,
                TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(2));
            Thread.Sleep(TimeSpan.FromSeconds(6));
           
            _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.5));

            Console.ReadLine();
            _timer.Dispose();
        }

        static Timer _timer;

        static void TimerOperation(DateTime start)
        {
            TimeSpan elapsed = DateTime.Now - start;
            Console.WriteLine($"{elapsed.Seconds} seconds from {start}.Timer Thread Pool thread id is:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
