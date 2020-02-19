using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RxAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reactive Extensions Async!");
            IObservable<string> observable = LongRunningOperationAsync("Task1");

            using (var sub=OutputToConsole(observable))
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            Console.WriteLine("-----------------------");
            Task<string> task = LongRunningOperationTaskAsync("Task2");
            using (var sub=OutputToConsole(task.ToObservable()))
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            Console.WriteLine("---------------------");

        }

        static async Task<T> AwaitOnObservable<T>(IObservable<T> observable)
        {
            T obj = await observable;
            Console.WriteLine($"{obj}");
            return obj;
        }

        static Task<string> LongRunningOperationTaskAsync(string name)
        {
            return Task.Run(()=>LongRunningOperation(name));
        }

        static string LongRunningOperation(string name)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return $"Task {name} is completed.Thread id {Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread:{Thread.CurrentThread.IsThreadPoolThread}";
        }

        static IObservable<string> LongRunningOperationAsync(string name)
        {
            return Observable.Start(()=>LongRunningOperation(name));
        }

        static IDisposable OutputToConsole(
            IObservable<EventPattern<ElapsedEventArgs>> sequence
            )
        {
            return sequence.Subscribe(
                obj => Console.WriteLine($"{obj.EventArgs.SignalTime}"),
                ex => Console.WriteLine($"Error:{ex.Message}"),
                () => Console.WriteLine("Completed.")
                );
        }

        static IDisposable OutputToConsole<T>(
            IObservable<T> sequence
            )
        {
            return sequence.Subscribe(
                obj => Console.WriteLine($"{obj}"),
                ex => Console.WriteLine($"Error:{ex.Message}"),
                () => Console.WriteLine("Completed.")
                );
        }
    }
}
