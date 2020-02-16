using System;
using System.Threading;
using System.Threading.Tasks;

namespace AggregateExceptions
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("AggregateExceptions!");

            Task task = AsynchronousProgressing();

            //Task.Run(async () => await AsynchronousProgressing());task调用异步任务
        }

        /// <summary>
        /// 如果task中存在多个异常，那么抛出的异常仅会显示第一个异常信息
        /// 有可能无法发现具体的异常原因。
        /// 因此需要将task中的异常展开然后进行迭代处理
        /// </summary>
        /// <returns></returns>
        async static Task AsynchronousProgressing()
        {
            Console.WriteLine($"1. Single Exception");
            try
            {
                string result = await GetInfoAsync("Task 1", 2);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception details:{ex}");
            }

            Console.WriteLine();
            Console.WriteLine("2. Multiple Exceptions");

            Task<string> task1 = GetInfoAsync("Task 1", 3);
            Task<string> task2 = GetInfoAsync("Task 2", 2);

            try
            {
                string[] result = await Task.WhenAll(task1, task2);
                Console.WriteLine(result.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception details:{ex}");
            }

            Console.WriteLine();
            Console.WriteLine("3. Multiple exceptions with AggregateException");

            task1 = GetInfoAsync("Task 1", 3);
            task2 = GetInfoAsync("Task 2", 2);

            Task<string[]> task3 = Task.WhenAll(task1, task2);
            try
            {
                string[] results = await task3;
                Console.WriteLine(results.Length);
            }
            catch
            {
                var ex = task3.Exception.Flatten();
                var exceptions = ex.InnerExceptions;
                foreach (var exception in exceptions)
                {
                    Console.WriteLine($"Exception details:{exception}");

                    Console.WriteLine();
                }
            }
        }

        async static Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            throw new Exception($"Boom from {name}");
        }
    }
}
