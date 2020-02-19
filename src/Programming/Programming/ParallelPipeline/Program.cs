using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parallel Pipeline!");

            var cts = new CancellationTokenSource();
            Task.Run(()=> {
                if (Console.ReadKey().KeyChar=='c')
                {
                    cts.Cancel();
                }
            });

            var sourceArrays = new BlockingCollection<int>[CollectionNumber];

            for (int i = 0; i < sourceArrays.Length; i++)
            {
                sourceArrays[i] = new BlockingCollection<int>(Count);
            }

            var filter1 = new PipelineWorker<int, decimal>(sourceArrays,
                    (n)=>Convert.ToDecimal(n*0.97),
                    cts.Token,
                    "filter1"
                );

            var filter2 = new PipelineWorker<decimal, string>(
                    filter1.Output,
                    (s)=>$"--{s}--",
                     cts.Token,
                    "filter2"
                );

            var filter3 = new PipelineWorker<string, string>(
                    filter2.Output,
                    (s) => Console.WriteLine($"The final result is {s} on thread id {Thread.CurrentThread.ManagedThreadId}"),
                     cts.Token,
                    "filter3"
                );

            try
            {
                Parallel.Invoke(
                    () =>
                    {
                        Parallel.For(0, sourceArrays.Length * Count, (j, state)
                                =>
                                {
                                    if (cts.Token.IsCancellationRequested)
                                    {
                                        state.Stop();
                                    }
                                    int k = BlockingCollection<int>.TryAddToAny(sourceArrays, j);
                                    if (k>=0)
                                    {
                                        Console.WriteLine($"added {j} to source data on thread id {Thread.CurrentThread.ManagedThreadId}");
                                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                                    }
                                }
                            );
                        foreach (var arr in sourceArrays)
                        {
                            arr.CompleteAdding();
                        }
                    },
                    ()=>filter1.Run(),
                    () => filter2.Run(),
                    () => filter3.Run()
                    );
            }
            catch(AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine(e.Message+e.StackTrace);
                }

                if (cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Operation has been canceled!Press enter to exit");
                }
                else
                {
                    Console.WriteLine("Press Enter to exit");
                }
                Console.ReadLine();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private const int CollectionNumber = 4;
        private const int Count = 10;

        class PipelineWorker<TSource, TDestination>
        {
            Func<TSource, TDestination> _processor = null;
            Action<TSource> _outputProcessor = null;
            BlockingCollection<TSource>[] _input;
            CancellationToken _token;
            public BlockingCollection<TDestination>[] Output { get;private set; }

            public PipelineWorker(BlockingCollection<TSource>[] input,
                    Func<TSource, TDestination> processor,
                    CancellationToken token,
                    string name
                )
            {
                _input = input;
                Output = new BlockingCollection<TDestination>[_input.Length];
                for (int i = 0; i < Output.Length; i++)
                    Output[i] = null == input[i] ? null : new BlockingCollection<TDestination>(Count);
                _processor = processor;
                _token = token;
                Name = name;
            }

            public PipelineWorker(BlockingCollection<TSource>[] input,
                Action<TSource> render,
                CancellationToken token,
                    string name)
            {
                _input = input;
                _outputProcessor = render;
                _token = token;
                Name = name;
                Output = null;
            }

            public string Name { get; private set; }


            public void Run()
            {
                Console.WriteLine($"{this.Name} is running！");
                while (!_input.All( bc=>bc.IsCompleted) && _token.IsCancellationRequested)
                {
                    TSource receivedItem;
                    int i = BlockingCollection<TSource>.TryTakeFromAny(
                        _input,out receivedItem,50,_token
                        );

                    if (i>=0)
                    {
                        if (Output != null)
                        {
                            TDestination outputItem = _processor(receivedItem);
                            BlockingCollection<TDestination>.AddToAny(Output,
                                outputItem
                                );
                            Console.WriteLine($"{Name} sent {outputItem} to next,on thread id {Thread.CurrentThread.ManagedThreadId}");

                            Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        }
                        else
                        {
                            _processor(receivedItem);
                        }
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    }
                }
                if (Output != null)
                {
                    foreach (var bc in Output)
                    {
                        bc.CompleteAdding();
                    }
                }
            
            }
        }

    }
}
