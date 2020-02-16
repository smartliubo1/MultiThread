using System;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task = AsynchronousWithTPL();
            task.Wait();

            task = AsynchronousWithAwait();

            task.Wait();
        }

        async static Task AsynchronousWithAwait()
        {
            try
            {
                string result = await GetInfoAsync("Task2");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        static Task AsynchronousWithTPL()
        {
            Task<string> task = GetInfoAsync("Task1");

            Task task2 = task.ContinueWith(t => Console.WriteLine(t.Result),
                TaskContinuationOptions.NotOnFaulted);

            Task task3 = task.ContinueWith(t => Console.WriteLine(t.Exception.InnerException),
                TaskContinuationOptions.OnlyOnFaulted);

            return Task.WhenAny(task2, task3);
        } 


        async static Task<string> GetInfoAsync(string name)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            //throw new Exception();
            return $"Task {name} is running on a thread id {Thread.CurrentThread.ManagedThreadId}." +
                $"Is thread pool thread :{Thread.CurrentThread.IsThreadPoolThread}";
        }
    }
}
