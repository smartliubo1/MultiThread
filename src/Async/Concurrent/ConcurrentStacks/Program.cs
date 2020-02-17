using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentStacks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Task.Run(()=> RunProgram());

            Task task = RunProgram();
            task.Wait();
        }

        /// <summary>
        /// LIFO
        /// </summary>
        /// <returns></returns>
        static async Task RunProgram()
        {
            var taskStack = new ConcurrentStack<CustomTask>();
            var cts = new CancellationTokenSource();
            var taskSource = Task.Run(()=>TaskProducer(taskStack));

            Task[] tasks = new Task[4];
            for (int i = 1; i <= 4; i++)
            {
                string processId = i.ToString();
                tasks[i - 1] = Task.Run(
                    ()=>TaskProcessor(taskStack,"Processor"+processId,cts.Token));
            }
            await taskSource;

            cts.CancelAfter(TimeSpan.FromSeconds(2));

            await Task.WhenAll(tasks);

        }

        static async Task TaskProcessor(ConcurrentStack<CustomTask> stack,
            string name,CancellationToken cancellationToken)
        {
            await GetRandomDelay();
            do
            {
                CustomTask workItem;
                bool popSuccessful = stack.TryPop(out workItem);
                if (popSuccessful)
                {
                    Console.WriteLine($"Task {workItem.Id} has been processed by {name}");
                }
                await GetRandomDelay();
            } while (!cancellationToken.IsCancellationRequested);
        }

        static async Task TaskProducer(ConcurrentStack<CustomTask> stack)
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(50);
                var workItem = new CustomTask { Id = i };
                stack.Push(workItem);
                Console.WriteLine($"Task {workItem.Id} has been posted.");
            }
        }

        static Task GetRandomDelay()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
            return Task.Delay(delay);
        }

        class CustomTask
        {
            public int Id { get; set; }
        }
    }
}
