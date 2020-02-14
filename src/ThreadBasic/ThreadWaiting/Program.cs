using System;
using System.Threading;

namespace ThreadDelay
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Thread thread = new Thread(PrintNumbersWaiting);
            thread.Start();
            thread.Join();//使得应用程序的主线程等待thread完成任务之后,再继续执行(这会阻塞当前的主线程)
            Console.WriteLine("waiting thread is completed,we continue do other works.");//continue
        }

        static void PrintNumbersWaiting()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine(i);
            }
        }
    }
}
