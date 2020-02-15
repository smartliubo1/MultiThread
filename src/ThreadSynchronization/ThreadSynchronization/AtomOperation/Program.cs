using System;
using System.Threading;

namespace AtomOperation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Incorrect counter with no lock");

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
            Console.WriteLine("Correct counter by using atom operation");
            var counterWithLock = new CounterNoLock();

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
            private int _count;
            public int Count { get { return _count; } }

            public override void Decrement()
            {
                _count--;
            }

            public override void Increment()
            {
                _count++;
            }
        }

        /// <summary>
        /// User InterLocked(atom operation) instead of lock object
        /// Interlocked provide the common atom operations
        /// </summary>
        class CounterNoLock : CounterBase
        {
            private int _count;
            public int Count { get { return _count; } } 

            public override void Decrement()
            {
                Interlocked.Decrement(ref _count);
            }

            public override void Increment()
            {
                Interlocked.Increment(ref _count);
            }
        }

        abstract class CounterBase
        {
            public abstract void Increment();
            public abstract void Decrement();
        }
    }
}
