using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace ObservableCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Observable collections!");
            foreach (var i in EnumerableEventSequence())
            {
                Console.WriteLine(i);
            }
            Console.WriteLine();
            Console.WriteLine("IEnumerable");

            IObservable<int> observable =
                EnumerableEventSequence().ToObservable();

            using (IDisposable subscription=observable.Subscribe(Console.Write))
            {
                Console.WriteLine();
                Console.WriteLine("IObservable");
            }

            observable = EnumerableEventSequence().ToObservable()
                .SubscribeOn(TaskPoolScheduler.Default);

            using (IDisposable subscription=observable.Subscribe(Console.Write))
            {
                Console.WriteLine();
                Console.WriteLine("IObservable async");                
                Console.WriteLine();
            }
        }

        static IEnumerable<int> EnumerableEventSequence()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                yield return i;
            }
        }


    }
}
