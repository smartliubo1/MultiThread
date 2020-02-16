  using System;
using System.Threading;
using System.Threading.Tasks;

namespace CombineTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var firstTask = new Task<int>(()=>TestMethod("first task",3));

            var secondTask = new Task<int>(() => TestMethod("second task", 2));

            //firstTask完成后,会执行后续的代码块
            firstTask.ContinueWith(
                task => Console.WriteLine($"The second answer is {task.Result}" +
                $".Thread id is {Thread.CurrentThread.ManagedThreadId}," +
                $"Is thread pool :{Thread.CurrentThread.IsThreadPoolThread}"),
                TaskContinuationOptions.OnlyOnRanToCompletion|TaskContinuationOptions.ExecuteSynchronously
                );
            firstTask.Start();
            secondTask.Start();

            Thread.Sleep(TimeSpan.FromSeconds(4));

            Console.WriteLine("--------------");

            //secondTask在之前已经执行完成,所以在此时的后续操作不是在线程池中执行的
            Task continuation = secondTask.ContinueWith(
                task => Console.WriteLine($"The second answer is {task.Result}" +
                $".Thread id is {Thread.CurrentThread.ManagedThreadId}," +
                $"Is thread pool :{Thread.CurrentThread.IsThreadPoolThread}"),
                TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
                );
            
            continuation.GetAwaiter().OnCompleted(
                ()=>Console.WriteLine($"Continuation task completed." +
                $"thread id is:{Thread.CurrentThread.ManagedThreadId}.Is thread pool :{Thread.CurrentThread.IsThreadPoolThread}")
                );
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine();

            Console.WriteLine("--------------");
            //为firstTask附加一个子线程,子线程附加后续操作,firstTask执行完后需要等待子线程及子线程的后续操作执行完毕才可以执行完毕.
            firstTask = new Task<int>(() =>
              {
                  //创建子线程,附加到父线程
                  var innerTask = Task.Factory.StartNew(
                      ()=> TestMethod("second task ",5),TaskCreationOptions.AttachedToParent
                      );
                  //设置子线程的后续操作
                  innerTask.ContinueWith(task=> TestMethod("Third task",2),
                      TaskContinuationOptions.AttachedToParent
                      );
                  return TestMethod("First Task", 2);
              });

            firstTask.Start();

            while (!firstTask.IsCompleted)
            {
                Console.WriteLine(firstTask.Status);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
            Console.WriteLine(firstTask.Status);

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        static int TestMethod(string name,int seconds)
        {
            Console.WriteLine($"Task {name} is running on a thread id is:{Thread.CurrentThread.ManagedThreadId}" +
                $",Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            return 42 * seconds;
        }
    }
}
