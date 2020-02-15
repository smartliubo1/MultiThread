using System;
using System.Threading;

namespace ThreadGround
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
              
            var sampleForeground = new ThreadSample(10);
            var sampleBackground = new ThreadSample(20);

            var threadOne = new Thread(sampleForeground.CountNumbers);
            threadOne.Name = "Fore Ground";//线程默认为前台线程
            var threadTwo = new Thread(sampleBackground.CountNumbers);
            threadTwo.Name = "Back Ground";
            threadTwo.IsBackground = true;
            threadOne.Start();//前台线程结束后,后台线程也会随之结束
            threadTwo.Start();
            //应用程序进程会等待所有前台线程完成后再结束,如果只剩下回台线程则会直接结束工作
        }


        class ThreadSample
        {
            private readonly int _iterations;

            public ThreadSample(int iterations)
            {
                this._iterations = iterations;
            }

            public void CountNumbers()
            {
                for (int i = 0; i < _iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine($"{Thread.CurrentThread.Name} prints {i}");
                }
            }
        }
    }
}
