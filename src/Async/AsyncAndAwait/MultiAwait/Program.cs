using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiAwait
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

        static Task AsynchronousWithTPL()
        {
            var containerTask = new Task(()=> {
                Task<string> task = GetInfoAsync("TPL 1");
                task.ContinueWith(t=> {
                    Console.WriteLine(t.Result);
                    Task<string> task2 = GetInfoAsync("TPL 2");

                    task2.ContinueWith(innerTask=> 
                        Console.WriteLine(innerTask.Result),
                        TaskContinuationOptions.NotOnFaulted|
                        TaskContinuationOptions.AttachedToParent
                    );
                task2.ContinueWith(innerTask => 
                        Console.WriteLine(innerTask.Exception.InnerException),
                        TaskContinuationOptions.OnlyOnFaulted|
                        TaskContinuationOptions.AttachedToParent
                        );
                },
                TaskContinuationOptions.NotOnFaulted|
                TaskContinuationOptions.AttachedToParent);

                task.ContinueWith(t=>
                    Console.WriteLine(t.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted|
                    TaskContinuationOptions.AttachedToParent
                );
            });
            containerTask.Start();
            return containerTask;
        }

        async static Task AsynchronousWithAwait()
        {
            try
            {
                string result = await GetInfoAsync("Async 1");
                Console.WriteLine(result);
                result = await GetInfoAsync("Async 2");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        async static Task<string> GetInfoAsync(string name)
        {
            Console.WriteLine($"Task {name} started.");
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (name=="TPL 2")
            {
                throw new Exception("Boom!");
            }
            return $"Task {name} is running on a thread id {Thread.CurrentThread.ManagedThreadId}." +
                $"Is thread pool thread :{Thread.CurrentThread.IsThreadPoolThread}";
        }
    }
}
