using System;
using System.Threading;

namespace Mutex
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const string MutexName = "C sharp multi thread example";

            using (var mutex=new System.Threading.  (false,MutexName))
            {
                if (!mutex.WaitOne(TimeSpan.FromSeconds(5),false))
                {
                    Console.WriteLine("Second instance is running! ");
                }
                else
                {
                    Console.WriteLine("Running");
                    Console.ReadLine();
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
