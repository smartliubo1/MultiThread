using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace CustomObservable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Customize your observable!");

            var observer = new CustomObserver();
            var goodObservable = new CustomSequence(new[] { 1,2,3,4,5});
            var badObservable = new CustomSequence(null);

            using (IDisposable subscription=goodObservable.Subscribe(observer))
            {
            }

            using (IDisposable subscription=goodObservable.SubscribeOn(TaskPoolScheduler.Default)
                .Subscribe(observer)
                )
            {
                Thread.Sleep(100);
            }

            using (IDisposable subscription = badObservable.SubscribeOn(TaskPoolScheduler.Default)
                .Subscribe(observer)
                )
            {
                Console.WriteLine();
            }
        }

        class CustomObserver : IObserver<int>
        {
            public void OnCompleted()
            {
                Console.WriteLine("Completed");
            }

            public void OnError(Exception error)
            {
                Console.WriteLine($"Error:{error.Message}");
            }

            public void OnNext(int value)
            {
                Console.WriteLine($"Next value:{value}.Thread Id:{Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread:{Thread.CurrentThread.IsThreadPoolThread}");
            }
        }

        class CustomSequence : IObservable<int>
        {
            private readonly IEnumerable<int> _numbers;

            public CustomSequence(IEnumerable<int> numbers)
            {
                this._numbers = numbers;
            }

            public IDisposable Subscribe(IObserver<int> observer)
            {
                foreach (var number in _numbers)
                {
                    observer.OnNext(number);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            }
        }
    }
}
