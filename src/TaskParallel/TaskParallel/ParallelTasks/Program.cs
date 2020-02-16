using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var firstTask = new Task<int>(()=>TaskMethod("First Task",3));

            var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));

            var whenAllTask = Task.WhenAll(firstTask, secondTask);

            whenAllTask.ContinueWith(task => 
                Console.WriteLine($"The first answer is {task.Result[0]},the second answer is {task.Result[1]}"),
                TaskContinuationOptions.OnlyOnRanToCompletion);

            firstTask.Start();
            secondTask.Start();

            Thread.Sleep(TimeSpan.FromSeconds(4));

            var tasks = new List<Task<int>>();

            for (int i = 1; i < 4; i++)
            {
                int counter = i;
                var task = new Task<int>(()=>
                    TaskMethod($"Task{counter}", counter)
                );
                tasks.Add(task);
                task.Start();
            }

            while (tasks.Count>0)
            {
                var completedTask = Task.WhenAny(tasks).Result;

                tasks.Remove(completedTask);

                Console.WriteLine($"A task has been completed with result {completedTask.Result}");
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    
        static int TaskMethod (string name,int seconds)
        {
            Console.WriteLine($"Task {name} is running on a thread id {Thread.CurrentThread.ManagedThreadId}.Is the thread pool:{Thread.CurrentThread.IsThreadPoolThread}");

            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            return 42 * seconds;
        }
    }
}
