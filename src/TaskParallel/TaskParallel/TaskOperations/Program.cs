using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TaskMethod("Main Task Method");

            Task<int> task = CreateTask("task 1");
            task.Start();

            int result = task.Result;

            Console.WriteLine($"task1's result is :{result}");

            Task<int> task2 = CreateTask("task 2");
            task2.RunSynchronously();//同步执行
            int result2 = task2.Result;
            Console.WriteLine($"task 2's result is :{result2}");

            Task<int> task3 = CreateTask("task 3");

            task3.Start();

            while (!task3.IsCompleted)
            {
                Console.WriteLine(task3.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

            Console.WriteLine(task3.Status);

            int result3 = task3.Result;

            Console.WriteLine($"task 3's result is :{result3}");
        }

        static Task<int> CreateTask(string name)
        {
            return new Task<int>(()=>TaskMethod(name));
        }

        static int TaskMethod(string name)
        {
            Console.WriteLine($"Task {name} is running on a thread id is:{Thread.CurrentThread.ManagedThreadId}" +
                $",Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return 42;
        }
    }
}
