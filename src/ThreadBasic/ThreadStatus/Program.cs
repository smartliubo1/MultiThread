using System;
using System.Threading;

namespace ThreadStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");
                Console.WriteLine("starting program");
                Thread thread = new Thread(PrintNumbersWithStatus);
                Thread thread1 = new Thread(DoNothing);
                Console.WriteLine(thread.ThreadState.ToString());
                thread1.Start();
                thread.Start();
                for (int i = 1; i < 30; i++)
                {
                    Console.WriteLine(thread.ThreadState.ToString());//we can see the thread's thread status
                }
                Thread.Sleep(TimeSpan.FromSeconds(6));
                thread.Abort();
                Console.WriteLine("a thread has been aborted.");
                Console.WriteLine(thread.ThreadState.ToString());
                Console.WriteLine(thread1.ThreadState.ToString());
            }
            catch (PlatformNotSupportedException ex)
            {
                Console.WriteLine("we have meet an thread abort exception");
                Console.WriteLine(ex.Message);
            }
        }

        static void DoNothing()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        static void PrintNumbersWithStatus()
        {

            Console.WriteLine("starting...");
            Console.WriteLine(Thread.CurrentThread.ThreadState.ToString());
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine(i);
            }
        }
    }
}
