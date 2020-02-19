using System;
using System.Reactive.Subjects;
using System.Threading;

namespace SubjectClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("observable subject!");
            Console.WriteLine("Subject");

            var subject = new Subject<string>();
            subject.OnNext("A");

            using (var subscription=OutputToConsole(subject))
            {
                subject.OnNext("B");
                subject.OnNext("C");
                subject.OnNext("D");
                subject.OnCompleted();
                subject.OnNext("Will not be printed out");
            }

            Console.WriteLine("Replay Subject");
            var replaySubject = new ReplaySubject<string>();
            replaySubject.OnNext("A");

            using(var subscription = OutputToConsole(replaySubject))
            {
                replaySubject.OnNext("B");
                replaySubject.OnNext("C");
                replaySubject.OnNext("D");
                replaySubject.OnCompleted();
            }

            Console.WriteLine($"Buffered ReplaySubject");

            var bufferedSubject = new ReplaySubject<string>(3);//缓存事件，稍后订阅也可以获得之前的事件
            bufferedSubject.OnNext("A");
            bufferedSubject.OnNext("B");
            bufferedSubject.OnNext("C");
            using (var subscription = OutputToConsole(bufferedSubject))
            {
                bufferedSubject.OnNext("D");
                bufferedSubject.OnCompleted();
            }

            Console.WriteLine("Time window ReplaySubject");

            var timeSubject = new ReplaySubject<string>(TimeSpan.FromMilliseconds(200));

            timeSubject.OnNext("A");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            timeSubject.OnNext("B");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            timeSubject.OnNext("C");
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            using (var subscription=OutputToConsole(timeSubject))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                timeSubject.OnNext("D");
                timeSubject.OnCompleted();
            }

            Console.WriteLine("Async Subject");

            var asyncsubject = new AsyncSubject<string>();
            asyncsubject.OnNext("A");

            using (var subscription=OutputToConsole(asyncsubject))
            {
                asyncsubject.OnNext("B");
                asyncsubject.OnNext("C");
                asyncsubject.OnNext("D");
                asyncsubject.OnCompleted();
            }

            Console.WriteLine("Behavior Subject..");
            var behaviorSubject = new BehaviorSubject<string>("Default");
            using (var subscription=OutputToConsole(behaviorSubject))
            {
                behaviorSubject.OnNext("B");
                behaviorSubject.OnNext("C");
                behaviorSubject.OnNext("D");
                behaviorSubject.OnCompleted();
            }


        }

        static IDisposable OutputToConsole<T>(IObservable<T> sequence)
        {
            return sequence.Subscribe(
                obj => Console.WriteLine($"{obj}"),
                ex => Console.WriteLine($"Error:{ex.Message}"),
                () => Console.WriteLine($"Completed")
                );
        }
    } 
}
