using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDataFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task = ProcessAsynchronously();

            task.Wait();
        }

        async static Task ProcessAsynchronously()
        {
            var cts = new CancellationTokenSource();

            await Task.Run(() =>
             {
                 if (Console.ReadKey().KeyChar == 'C')
                     cts.Cancel();
             });

            var inputBlock = new BufferBlock<int>(
                    new DataflowBlockOptions
                    {
                        BoundedCapacity = 5,
                        CancellationToken = cts.Token
                    }
                );

            var filterBlock = new TransformBlock<int, decimal>(
                    n =>
                    {
                        decimal result = Convert.ToDecimal(n * 0.97);
                        Console.WriteLine($"Filter 1 sent {result} to the next stage on thread id {Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread :{Thread.CurrentThread.IsThreadPoolThread}");
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        return result;
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = 4,
                        CancellationToken = cts.Token
                    }
                );

            var filter2Blcok = new TransformBlock<decimal, string>(
                    n =>
                    {
                        string result = $"--{n}--";
                        Console.WriteLine($"Filter 2 sent {result} to the next stage on thread id {Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread :{Thread.CurrentThread.IsThreadPoolThread}");
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        return result;
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = 4,
                        CancellationToken = cts.Token
                    }
                );

            var outputBlock = new ActionBlock<string>(
                    s =>
                    {
                        Console.WriteLine($"The final result is {s} on thread id {Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread :{Thread.CurrentThread.IsThreadPoolThread}");
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = 4,
                        CancellationToken = cts.Token
                    }
                );

            inputBlock.LinkTo(filterBlock, new DataflowLinkOptions { PropagateCompletion = true });
            filterBlock.LinkTo(filter2Blcok, new DataflowLinkOptions { PropagateCompletion = true });
            filter2Blcok.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            try
            {
                Parallel.For
                (0,
                    20,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = 4,
                        CancellationToken = cts.Token
                    },
                    i =>
                    {
                        Console.WriteLine($"added {i} to source data on thread id {Thread.CurrentThread.ManagedThreadId}.Is Thread Pool Thread :{Thread.CurrentThread.IsThreadPoolThread}");
                        inputBlock.SendAsync(i).GetAwaiter().GetResult();
                    }
                );
                inputBlock.Complete();

                await outputBlock.Completion;
                Console.WriteLine("Press Enter to exit.");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Operation has been canceled.Press Enter to Exit;");
                Console.WriteLine($"Error:{ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
