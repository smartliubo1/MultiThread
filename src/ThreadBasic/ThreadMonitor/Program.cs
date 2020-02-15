using System;
using System.Threading;

namespace ThreadMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Death Lock Example!");
            object lock1 = new object();

            object lock2 = new object();

            new Thread(() => DeathLock(lock1, lock2)).Start();

            lock (lock2)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"Monitor.TryEnter allows not to get stuck,returning false after a specified timeout ia elapsed");

                if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
                {
                    Console.WriteLine("Acquired a protected resource successfully!");
                }
                else
                {
                    Console.WriteLine("Timeout acquiring a resource!");
                }
            }

            new Thread(() => DeathLock(lock1, lock2)).Start();

            Console.WriteLine("-------------------------------------");
            lock (lock2)
            {
                Console.WriteLine("this will be a deadlock!");
                Thread.Sleep(1000);
                lock (lock1)
                {
                    Console.WriteLine("Acquired a protected resource successfully!");
                }
            }

        }

        static void DeathLock(object lock1, object lock2)
        {
            lock (lock1)
            {
                Thread.Sleep(1000);
                lock (lock2){ }
            }
            //Monitor's Enter and Exit method  is the lock(xx){} constructor
            //try
            //{
            //    Monitor.Enter(lock1);
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("monitor failed"); ;
            //}
            //finally
            //{
            //    Monitor.Exit(lock1);
            //}
        }
    }
}
