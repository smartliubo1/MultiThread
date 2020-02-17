using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BlockingCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Using a queue inside of BlockingCollection!");
            Console.WriteLine();
            Task task = RunProgram();
            task.Wait();

            Console.WriteLine();
            Console.WriteLine("Using a stack inside of BlockingCollection!");
            Console.WriteLine();

            task = RunProgram(new ConcurrentStack<CustomTask>());
            task.Wait();
            
        }


        static async Task RunProgram(IProducerConsumerCollection<CustomTask> collection=null)
        {
            var taskCollection = new BlockingCollection<CustomTask>();
            if (collection!=null)
            {
                taskCollection = new BlockingCollection<CustomTask>(collection);
            }

            var taskSource = Task.Run(() => TaskProducer(taskCollection));

            Task[] processors = new Task[4];
            for (int i = 1; i <= 4; i++)
            {
                string processorId = "Processor" + i;
                processors[i - 1] = Task.Run(()=>TaskProcessor(taskCollection,processorId));
            }
            await taskSource;

            await Task.WhenAll(processors);
        }

        /// <summary>
        /// BlockingCollection默认使用ConcurrentQueue的队列方式工作
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        static async Task TaskProducer(BlockingCollection<CustomTask> collection)
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(20);
                var workItem = new CustomTask { Id = i };
                collection.Add(workItem);
                Console.WriteLine($"Task {workItem.Id} has been posted.");
            }
            collection.CompleteAdding();
        }

        static async Task TaskProcessor(BlockingCollection<CustomTask> collection,string name )
        {
            await GetRandomDelay();
            foreach (CustomTask item in collection.GetConsumingEnumerable())
            {
                Console.WriteLine($"Task {item.Id} has been processed by {name}.");
                await GetRandomDelay();
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
