using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace ObservableObject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IObservable<int> observable = Observable.Return(0);
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");

            observable = Observable.Empty<int>();
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");

            observable = Observable.Throw<int>(new Exception());
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");

            observable = Observable.Repeat(42);
            using (IDisposable subscription = OutputToConsole(observable.Take(5))) { }
            Console.WriteLine("---------------------");

            observable = Observable.Range(0, 10);
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");

            observable = Observable.Create<int>(obj=> 
            {
                for (int i = 0; i < 10; i++)
                {
                    obj.OnNext(i);
                }

                return Disposable.Empty;
            });
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");

            observable = Observable.Generate(
                    0, //initial state
                    i=>i<5,//while true continue the sequence
                    i=>++i,//iteration
                    i=>i*2//selecting result
                );
            using (var subscription = OutputToConsole(observable)) { }
            Console.WriteLine("---------------------");


            IObservable<long> ol = Observable.Interval(TimeSpan.FromSeconds(1));
            using (var subscription = OutputToConsole(ol)) {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            Console.WriteLine("---------------------");

            ol = Observable.Timer(DateTimeOffset.Now.AddSeconds(2));
            using (var subscription = OutputToConsole(ol))
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            Console.WriteLine("---------------------");
        }

        static IDisposable OutputToConsole<T>(IObservable<T> sequence)
        {
            return sequence.Subscribe(
                    obj => Console.WriteLine($"{obj}"),
                    ex => Console.WriteLine($"Error:{ex.Message}"),
                    () => Console.WriteLine("Completed")
                );
        }
    }
}
