using System;
using System.Threading;

namespace ThreadParams
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var sample = new ThreadSample(10);

            var threadOne = new Thread(sample.CountNumbers);
            threadOne.Name = "Thread One";
            threadOne.Start();
            threadOne.Join();
            Console.WriteLine("--------------------------");

            var threadTwo = new Thread(Count);
            threadTwo.Name = "Thread Two";
            threadTwo.Start(8);
            threadTwo.Join();
            Console.WriteLine("--------------------------");

            var threadThree = new Thread(()=>CountNumbers(12));
            threadThree.Name = "Thread Three";
            threadThree.Start();
            threadThree.Join();
            Console.WriteLine("---------------------------");

            int i = 10;
            var threadFour = new Thread(()=>PrintNumber(i));
            threadFour.Start();

            i = 20;
            var threadFive= new Thread(() => PrintNumber(i));//多个lambda表达式中使用相同的变量,它们会共享该变量的值
            threadFive.Start();
            
        }

        static void Count(object iterations)
        {
            CountNumbers((int)iterations);
        }

        static void CountNumbers(int iterations)
        {
            for (int i = 1; i < iterations; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                Console.WriteLine($"{Thread.CurrentThread.Name} prints {i}");
            }
        }

        static void PrintNumber(int number)
        {
            Console.WriteLine(number);
        }

        class ThreadSample
        {
            private readonly int _iterations;

            public ThreadSample(int iterations)
            {
                this._iterations = iterations;
            }

            public void CountNumbers()
            {
                for (int i = 0; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine($"{Thread.CurrentThread.Name} prints {i}");
                }
            }
        }
    }
}
