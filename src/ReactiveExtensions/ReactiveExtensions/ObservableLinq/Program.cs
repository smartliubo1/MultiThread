using System;
using System.Reactive.Linq;

namespace ObservableLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IObservable<long> sequence =
                Observable.Interval(TimeSpan.FromMilliseconds(50))
                .Take(21);

            var evenNumbers = from n in sequence
                              where n % 2 == 0
                              select n;
            
            var oddNumbers= from n in sequence
                            where n % 2 != 0
                            select n;

            var combine = from n in evenNumbers.Concat(oddNumbers)
                          select n;

            var nums = (from n in combine
                        where n % 5 == 0
                        select n)
                      .Do(n => Console.WriteLine($"----Number {n} is processed in Do method"));

            using (var sub1=OutputToConsole(sequence,0)){            }
            using (var sub2 = OutputToConsole(combine, 1)) { }
            using (var sub3 = OutputToConsole(nums, 2)) {
                Console.WriteLine("Process Enter to finish the demo");
                Console.ReadLine();
            }
        }

        static IDisposable OutputToConsole<T>(IObservable<T> sequence,
            int innerLevel)
        {
            string delimiter = innerLevel == 0 ? string.Empty : new string('-', innerLevel * 3);
            return sequence.Subscribe(
                    obj => Console.WriteLine($"{delimiter}{obj}"),
                    ex => Console.WriteLine($"Error:{ex.Message}"),
                    () => Console.WriteLine($"{delimiter} completed.")
                );
        } 
    }
}
