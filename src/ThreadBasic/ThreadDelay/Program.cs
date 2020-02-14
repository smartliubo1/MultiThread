using System;
using System.Threading;

namespace ThreadDelay
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Thread thread = new Thread(PrintNumbersWithDelay);
            thread.Start();
            Console.WriteLine("-----------");
            PrintNumbersSync(1);
        }

        static void PrintNumbersWithDelay()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine(i);
            }
        }

        static void PrintNumbersSync(int number)
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"{number}" + i);
            }
        }

    }
}
