using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PLinqPartition
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("partitioner!");
            var partitioner = new StringPartitioner(GetTypes());

            var parallelQuery = from t in partitioner.AsParallel()
                                select EmulatoProcessing(t);

            parallelQuery.ForAll(PrintInfo);
        }

        static void PrintInfo(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));
            Console.WriteLine($"{typeName} type was printed on a thread id {Thread.CurrentThread.ManagedThreadId}.Is thread pool thread {Thread.CurrentThread.IsThreadPoolThread}");
        }

        static string EmulatoProcessing(string typeName)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(150));

            Console.WriteLine($"{typeName} type was processed on a thread id :{Thread.CurrentThread.ManagedThreadId}.Has{typeName.Length % 2:'even':'odd'} length");

            return typeName;
        }

        static IEnumerable<string> GetTypes()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetExportedTypes());

            return from type in types
                   where type.Name.StartsWith("Web")
                   select type.Name;
        }

        public class StringPartitioner : Partitioner<string>
        {
            private readonly IEnumerable<string> _data;

            public StringPartitioner(IEnumerable<string> data)
            {
                this._data = data;
            }

            public override bool SupportsDynamicPartitions
            {
                get
                {
                    return false;
                }
            }

            public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
            {
                var result = new List<IEnumerator<string>>(2);
                result.Add(CreateEnumerator(true));
                result.Add(CreateEnumerator(false));
                return result;
            }

            IEnumerator<string> CreateEnumerator(bool isEven)
            {
                foreach (var d in _data)
                {
                    if(!(d.Length%2 ==0 ^ isEven))
                    yield return d;
                }
            }
        }
    }
}
