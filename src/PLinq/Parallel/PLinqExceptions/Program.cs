using System;
using System.Collections.Generic;
using System.Linq;

namespace PLinqExceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PLinq Exceptions!");

            IEnumerable<int> numbers = Enumerable.Range(-5, 10);

            var query = from number in numbers
                        select 100 / number;
            try
            {
                foreach (var n in query)
                {
                    Console.WriteLine(n);
                }
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("DivideByZero:" + ex);
            }

            Console.WriteLine("--------------");
            Console.WriteLine("Sequential Linq Query Processing");
            Console.WriteLine();

            try
            {
                var parallelQuery = from number in numbers.AsParallel()
                                    select 100 / number;

                parallelQuery.ForAll(Console.WriteLine);
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("DivideByZero parallel:" + ex);
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(e =>
                {
                    if (e is DivideByZeroException)
                    {
                        Console.WriteLine("Diveded by zero - aggregate exception handler!");
                        return true;
                    }
                    return false;
                });
            }
            Console.WriteLine("--------------");
            Console.WriteLine("Parallel Linq Query Processing and Result merging");
        }
    }
}
