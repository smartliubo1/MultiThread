using System;
using System.Threading;

namespace ThreadAbort
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");

                Thread thread = new Thread(PrintNumbersAbort);
                thread.Start();
                Thread.Sleep(TimeSpan.FromSeconds(6));
                thread.Abort();//it will cause an exception
                //捕获异常了之后可以发现,实际上该线程没有被终止,仍会继续打印出数字信息
                Console.WriteLine("A Thread has been aborted");
                Thread thread1 = new Thread(PrintNumbersSync);
                thread1.Start();
                PrintNumbersSync();
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine("we have meet an thread abort exception") ;
                Console.WriteLine(ex.Message);
            }
            catch (PlatformNotSupportedException ex)
            {
                Console.WriteLine("we have meet an thread abort exception");
                Console.WriteLine(ex.Message);
            }
        }

        static void PrintNumbersAbort()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine(i);
            }
        }

        static void PrintNumbersSync()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"After Abort" + i);
            }
        }
    }
}
