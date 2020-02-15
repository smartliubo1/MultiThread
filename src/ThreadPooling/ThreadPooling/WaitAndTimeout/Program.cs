using System;
using System.Runtime.Versioning;
using System.Threading;

namespace WaitAndTimeout
{
    class Program
    {
        /// <summary>
        /// the correct way to handle the wait event and timeout
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("ThreadPool handle operation's wait and timeout !");

            RunOperations(TimeSpan.FromSeconds(50));
            RunOperations(TimeSpan.FromSeconds(110));
        }

        /// <summary>
        /// 运行操作
        /// 1、首先为线程池注册WaitHandle委托
        /// 2、线程池运行操作
        /// 3、运行完成或超时之后，取消注册WaitHandle委托
        /// </summary>
        /// <param name="workerOperationTimeout"></param>
        static void RunOperations(TimeSpan workerOperationTimeout)
        {
            using (var events = new ManualResetEvent(false))
            {
                using (var cts = new CancellationTokenSource())
                {
                    Console.WriteLine("Register timeout operations...");
                    //该方法允许将回调函数放入线程池中的队列中，
                    //当提供当等待事件处理器接收到信号或发生超时时，该回调函数将被调用
                    //可以为线程池中的操作实现超时的功能
                    var worker = ThreadPool.RegisterWaitForSingleObject(
                        events, (state, timeout) => WorkOperationWait(cts, timeout),
                        null,
                        workerOperationTimeout,
                        true);
                    Console.WriteLine("Starting long running operations...");
                    ThreadPool.QueueUserWorkItem(_ => WorkerOperation(cts.Token, events));//执行操作，附带ManualResetEvent信号参数
                    Thread.Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));
                    worker.Unregister(events);
                }
            }
        }
        /// <summary>
        /// ManualResetEvent信号
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="manualResetEvent"></param>
        static void WorkerOperation(CancellationToken cancellationToken, ManualResetEvent manualResetEvent)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Executing time is :{i + 1}");
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Operation has been canceled");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            manualResetEvent.Set();//设置操作完成的信号，通知对应的线程
        }

        /// <summary>
        /// 线程池是否正常完成操作的回调函数
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="isTimeOut"></param>
        static void WorkOperationWait(CancellationTokenSource cancellationTokenSource, bool isTimeOut)
        {
            if (isTimeOut)
            {
                cancellationTokenSource.Cancel();
                Console.WriteLine("work operation is time out and was canceled");
            }
            else
            {
                Console.WriteLine("work operation succeed");
            }
        }
    }
}
