using System;
using System.ComponentModel;
using System.Threading;

namespace BackGroundWorkerClass
{
    class Program
    {
        /// <summary>
        /// 基于事件的异步模式(EAP)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork += Worker_DoWork;
            backgroundWorker.ProgressChanged += Work_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += Worker_Completed;

            backgroundWorker.RunWorkerAsync();

            Console.WriteLine("Progress C to cancel work.");

            do
            {
                if (Console.ReadKey(true).KeyChar == 'C')
                {
                    backgroundWorker.CancelAsync();
                }
            } while (backgroundWorker.IsBusy);
        }

        static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine($"DoWork Thread pool thread id is;{Thread.CurrentThread.ManagedThreadId}");

            var backgroundWorker = (BackgroundWorker)sender;

            for (int i = 1; i < 100; i++)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                backgroundWorker.ReportProgress(i);

                Thread.Sleep(TimeSpan.FromSeconds(0.1));
            }
            e.Result = 42;
        }

        static void Work_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            Console.WriteLine($"{e.ProgressPercentage}% completed.Progress thread pool thread is :{Thread.CurrentThread.ManagedThreadId}");
        }

        static void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine($"Completed thread pool id is:{Thread.CurrentThread.ManagedThreadId}");

            if (e.Error != null)
            {
                Console.WriteLine($"Exception {e.Error.Message} has occured.");
            }
            else if (e.Cancelled)
            {
                Console.WriteLine($"Operation has been canceled.");
            }
            else
            {
                Console.WriteLine($"The answer is :{e.Result}");
            }
        }
    }
}
