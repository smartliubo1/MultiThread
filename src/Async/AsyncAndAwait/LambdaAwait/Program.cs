using System;
using System.Threading;
using System.Threading.Tasks;

namespace LambdaAwait
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task.Run(async () => await AsynchronousProcessing()).GetAwaiter().GetResult();//通过这样即可在Main方法中不使用async 调用async方法。

            //Task task = AsynchronousProcessing();//这样调用async方法也可以
            Console.ReadLine();
        }

        async static Task AsynchronousProcessing()
        {
            Func<string, Task<string>> asyncLambda = async name => 
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return $"Task {name} is running on a thread id {Thread.CurrentThread.ManagedThreadId}." +
                $"Is thread pool thread :{Thread.CurrentThread.IsThreadPoolThread}";
            };
            string result = await asyncLambda("async lambda");
            Console.WriteLine(result);
        }


    }
}
