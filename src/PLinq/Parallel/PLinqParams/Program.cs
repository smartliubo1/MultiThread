using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PLinqParams
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("parallel Query Params!");

            var parellelQuery = from t in GetTypes().AsParallel()
                                select EmulateProcessing(t);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                parellelQuery.WithDegreeOfParallelism
                    (Environment.ProcessorCount).WithExecutionMode
                    (ParallelExecutionMode.ForceParallelism)
                    .WithMergeOptions(ParallelMergeOptions.Default)
                    .WithCancellation(cts.Token)
                    .ForAll(Console.WriteLine);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("______________");
                Console.WriteLine($"Opearion has been canceled .{ex}");
            }
            Console.WriteLine("----------");
            Console.WriteLine("Unordered PLINQ query execution");

            var unorderedQuery = from i in ParallelEnumerable.Range(1, 30)
                                 select i;

            foreach (var i in unorderedQuery)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("Ordered PLINQ query execution");
            var orderedQuery = from i in ParallelEnumerable.Range(1, 30).AsOrdered()
                               select i;
            foreach (var i in orderedQuery)
            {
                Console.WriteLine(i);
            }
        }


        static void PrintInfo(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine($"{typeName} type was printed on a thread id {Thread.CurrentThread.ManagedThreadId}");
        }

        static string EmulateProcessing(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine($"{typeName} type was printed on a thread id {Thread.CurrentThread.ManagedThreadId}. Is thread pool thread {Thread.CurrentThread.IsThreadPoolThread}");
            return typeName;
        }

        static IEnumerable<string> GetTypes()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetExportedTypes()
                   //where type.Name.StartsWith("W")
                   select type.Name;
        }
    }
}
