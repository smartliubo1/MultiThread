using System;
using System.Threading;

namespace SpinWaitClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var thread1 = new Thread(UserModeWait);

            var thread2 = new Thread(HybirdSpinWait);

            Console.WriteLine("Running use mode waiting");
            thread1.Start();
            Thread.Sleep(20000);
            _isCompleted = true;

            Thread.Sleep(TimeSpan.FromSeconds(1));
            _isCompleted = false;

            Console.WriteLine("Running hybird spin wait");

            thread2.Start();

            Thread.Sleep(5);

            _isCompleted = true;
        }

        static volatile bool _isCompleted = false;

        /// <summary>
        /// while loop will cost too much cpu's cost
        /// </summary>
        static void UserModeWait()
        {
            while (!_isCompleted)
            {
                Console.WriteLine(".");
            }
            Console.WriteLine();

            Console.WriteLine("Waiting is completed");
        }

        static void HybirdSpinWait()
        {
            var spinWait = new SpinWait();
            while (!_isCompleted)
            {
                spinWait.SpinOnce();
                Console.WriteLine(spinWait.NextSpinWillYield);
            }
            Console.WriteLine("Spin Waiting is completed");
        }
    }
}
