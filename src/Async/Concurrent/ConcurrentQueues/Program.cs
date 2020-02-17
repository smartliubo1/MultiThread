 using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentQueues
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task = RunProgram();
            task.Wait();
        }

        /// <summary>
        /// FIFO,thread-safe
        /// </summary>
        /// <returns></returns>
        static async Task RunProgram()
        {
            var taskQueue = new ConcurrentQueue<CustomTask>();
            var cts = new CancellationTokenSource();

            var taskSource = Task.Run(()=>TaskProducer(taskQueue));
                
            Task[] processors = new Task[4];
            for (int i = 1; i <= 4; i++)
            {
                string processorId = i.ToString();
                processors[i - 1] = Task.Run(()=>
                    TaskProcessor(taskQueue, "Processor" + processorId,cts.Token)
                );
            }
            await taskSource;

            cts.CancelAfter(TimeSpan.FromSeconds(5));

            await Task.WhenAll(processors);
        }

        static async Task TaskProcessor(ConcurrentQueue<CustomTask> queue,
            string name,CancellationToken cancellationToken)
        {
            CustomTask workItem;
            bool dequeueSuccessful = false;
            await GetRandomDelay();
            do
            {
                dequeueSuccessful = queue.TryDequeue(out workItem);
                if (dequeueSuccessful)
                {
                    Console.WriteLine($"Task {workItem.Id} has been processed by {name}");
                }
                await GetRandomDelay();
            } while (!cancellationToken.IsCancellationRequested);
        }

        static async Task TaskProducer(ConcurrentQueue<CustomTask> queue)
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(50);
                var workItem = new CustomTask { Id = i };
                queue.Enqueue(workItem);
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
