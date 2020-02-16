using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCreate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task1 = new Task(() => TestMethod("task1"));
            Task task2 = new Task(() => TestMethod("task2"));

            task2.Start();

            task1.Start();

            Task.Run(()=>TestMethod("task 3"));

            Task.Factory.StartNew(()=> TestMethod("task 4"));
            Task.Factory.StartNew(() => TestMethod("task 5"),
                TaskCreationOptions.LongRunning);

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        static void TestMethod(string name)
        {
            Console.WriteLine($"Task {name} is running on a thread id is:{Thread.CurrentThread.ManagedThreadId}" +
                $",Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
        }
    }
}
