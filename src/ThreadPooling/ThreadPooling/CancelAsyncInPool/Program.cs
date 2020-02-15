using System;
using System.Threading;

namespace CancelAsyncInPool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cancel the async operation in ThreadPool!");

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation1(cancellationToken));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation2(cancellationToken));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            using (var cts = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cts.Token;
                ThreadPool.QueueUserWorkItem(_ => AsyncOperation3(cancellationToken));
                Thread.Sleep(TimeSpan.FromSeconds(2));
                cts.Cancel();
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

        }

        /// <summary>
        /// 通过轮询检查操作是否被取消了
        /// </summary>
        /// <param name="cancellationToken"></param>
        static void AsyncOperation1(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting the first task");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"first task's run time:{i}");
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("The first task has been canceled");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The first task has been completed successfully");
        }
        /// <summary>
        /// 通过操作之外的代码来进行处理，控制何时取消以及取消之后的处理
        /// </summary>
        /// <param name="cancellationToken"></param>
        static void AsyncOperation2(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Starting the second task");

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"second task's run time:{i}");
                    cancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                Console.WriteLine("The second task has been canceled");

            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("The second task has been canceled");
                Console.WriteLine($"exception is {ex.Message.ToString()}");
            }
        }

        /// <summary>
        /// 注册回调函数。
        /// 当操作被取消时，线程池调用回调函数，这样允许链式传递一个取消逻辑到另一个异步操作中。
        /// </summary>
        /// <param name="cancellationToken"></param>
        private static void AsyncOperation3(CancellationToken cancellationToken)
        {
            bool cancellationFlag = false;

            //注册的委托只有当操作被取消时才会实际被调用，不会立即执行
            cancellationToken.Register(() =>
            {
                cancellationFlag = true;
                Console.WriteLine("其他操作，我被回调然后取消本次异步操作了");
            });

            Console.WriteLine("Starting the third task");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"third task's run time:{i}");
                if (cancellationFlag)
                {
                    Console.WriteLine("The third task has been canceled");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            Console.WriteLine("The third task has been completed successfully");

        }
    }
}
