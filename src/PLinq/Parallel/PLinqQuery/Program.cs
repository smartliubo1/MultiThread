using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PLinqQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parallel Query!");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var query = from t in GetTypes()
                        select EmulateProcessing(t);
            foreach (string name in query)
            {
                PrintInfo(name);
            }
            stopWatch.Stop();
            Console.WriteLine("-----------");
            Console.WriteLine("Sequential LINQ query.");
            Console.WriteLine($"Time span ;{stopWatch.Elapsed}");
            Console.WriteLine("Press Enter to continue.....");


            stopWatch.Reset();
            stopWatch.Start();

            var parallelQuery = from t in ParallelEnumerable.AsParallel(GetTypes())
                                select EmulateProcessing(t);

            parallelQuery.ForAll(PrintInfo);

            stopWatch.Stop();
            Console.WriteLine("-----------");
            Console.WriteLine("Parallel Linq Query. The results are being merged on a single thread.");
            Console.WriteLine($"Time span ;{stopWatch.Elapsed}");
            Console.WriteLine("Press Enter to continue.....");


            stopWatch.Reset();
            stopWatch.Start();

            query = from t in GetTypes().AsParallel().AsSequential()
                    select EmulateProcessing(t);

            foreach (string name in query)
            {
                PrintInfo(name);
            }
            stopWatch.Stop();

            Console.WriteLine("-----------");
            Console.WriteLine("Parallel LINQ query ,transformed into sequential.");
            Console.WriteLine($"Time span ;{stopWatch.Elapsed}");
        
            
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
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }
    }
}
