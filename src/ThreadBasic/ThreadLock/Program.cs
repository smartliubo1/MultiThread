using System;
using System.Threading;

namespace ThreadLock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Incorrect counter");

            var counter = new Counter();
            var thread1 = new Thread(() => TestCounter(counter));
            var thread2 = new Thread(() => TestCounter(counter));
            var thread3 = new Thread(() => TestCounter(counter));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread1.Join();
            thread2.Join();
            thread3.Join();

            Console.WriteLine($"Total count:{counter.Count}");
            
            Console.WriteLine("---------------------");
            Console.WriteLine("Correct counter");
            var counterWithLock = new CounterWithLock();

            var threadLock1 = new Thread(() => TestCounter(counterWithLock));

            var threadLock2 = new Thread(() => TestCounter(counterWithLock));

            var threadLock3 = new Thread(() => TestCounter(counterWithLock));

            threadLock1.Start();
            threadLock2.Start();
            threadLock3.Start();

            threadLock1.Join();
            threadLock2.Join();
            threadLock3.Join();

            Console.WriteLine($"Total count:{counterWithLock.Count}");
        }

        static void TestCounter(CounterBase counterBase)
        {
            for (int i = 0; i < 1000; i++)
            {
                counterBase.Increment();
                counterBase.Decrement();
            }
        }

        class Counter : CounterBase
        {
            public int Count { get; private set; }

            public override void Decrement()
            {
                Count--;
                //Console.WriteLine($"After Decrement is:{Count}");
            }

            public override void Increment()
            {
                Count++;
                //Console.WriteLine($"After Increment is:{Count}");
            }
        }

        class CounterWithLock : CounterBase
        {
            private readonly object _syncRoot = new object();

            public int Count { get; private set; }

            public override void Decrement()
            {
                lock (_syncRoot)
                {
                    Count--;
                    //Console.WriteLine($"With Lock After Decrement is:{Count}");
                }
            }

            public override void Increment()
            {
                lock (_syncRoot)
                {
                    Count++;
                    //Console.WriteLine($"With Lock After Increment is:{Count}");
                }
            }
        }

        abstract class CounterBase
        {
            public abstract void Increment();
            public abstract void Decrement();
        }
    }
}
