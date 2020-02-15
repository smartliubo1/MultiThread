using System;
using System.Threading;

namespace BarrierClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var thread1 = new Thread(()=>PlayMusic("The guitarist","play an amazing solo",5));

            var thread2 = new Thread(()=>PlayMusic("The singer","sing his song",2));

            thread1.Start();
            thread2.Start();
        }

        static Barrier _barrier = new Barrier(3, b =>  Console.WriteLine($"End of phase {b.CurrentPhaseNumber+1}") );//when the specified thread number use the SignalAndWait() method , Barrier will do the delegate method.

        static void PlayMusic(string name,string message,int seconds)
        {
            Console.WriteLine("-------------------");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine($"{name} starts to {message}");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine($"{name} finish to {message}");
            _barrier.SignalAndWait();
        }
    }
}
