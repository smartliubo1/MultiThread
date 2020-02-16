using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncVoid
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Async void and async task!");

            Task task = AsyncTask();
            task.Wait();

            AsyncVoid();

            Thread.Sleep(TimeSpan.FromSeconds(3));

            //task = AsyncTaskWithError();
            //while (!task.IsFaulted)
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //}
            //Console.WriteLine(task.Exception);
            //try
            //{
            //    AsyncVoidWithError();
            //    Thread.Sleep(TimeSpan.FromSeconds(3));
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
            int[] numbers = new[] { 1, 2, 3, 4, 5 };

            Array.ForEach(numbers, async number =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                if (number == 3)
                {
                    throw new Exception($"Boom !");
                }
                Console.WriteLine(number);
            });
            Console.ReadLine();
        }

        async static Task AsyncTaskWithError()
        {
            string result = await GetInfoAsync("AsyncTaskException", 2);
            Console.WriteLine(result);
        }

        async static void AsyncVoidWithError()
        {
            string result = await GetInfoAsync("AsyncVoidException", 2);
            Console.WriteLine(result);
        }

        async static Task AsyncTask()
        {
            string result = await GetInfoAsync("AsyncTask", 2);
            Console.WriteLine(result);
        }

        private static async void AsyncVoid()
        {
            string result = await GetInfoAsync("AsyncVoid", 2);
            Console.WriteLine(result);
        }

        async static Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            if (name.Contains("Exception"))
            {
                throw new Exception($"Boom from {name}!");
            }

            return $"Task {name} is running on a thread {Thread.CurrentThread.ManagedThreadId}" +
                $".Is the thread pool thread :{Thread.CurrentThread.IsThreadPoolThread}.";
        }
    }
}
