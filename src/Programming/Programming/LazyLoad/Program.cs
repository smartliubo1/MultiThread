using System;
using System.Threading;
using System.Threading.Tasks;

namespace LazyLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var task = ProcessAsynchronously();

            task.GetAwaiter().GetResult();
            Console.WriteLine("Press Any Key to Exit");
            Console.ReadLine();
        }

        static async Task ProcessAsynchronously()
        {
            var unsafeState = new UnsafeState();
            Task[] tasks = new Task[4];

            for (int i = 0; i < 4; i++)
            {
                tasks[i] = Task.Run(()=>Worker(unsafeState));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------");

            var firstState = new DoubleCheckedLocking();
            for (int i = 0; i < 4; i++)
            {
                tasks[i] = Task.Run(() => Worker(firstState));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------");

            var secondState = new BLCDoubleChecked();
            for (int i = 0; i < 4; i++)
            {
                tasks[i] = Task.Run(() => Worker(secondState));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------");

            var thridState = new Lazy<ValueToAccess>(Compute);
            for (int i = 0; i < 4; i++)
            {
                tasks[i] = Task.Run(() => Worker(thridState));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------");

            var fourthState = new BLCThreadSafeFactory();
            for (int i = 0; i < 4; i++)
            {
                tasks[i] = Task.Run(() => Worker(fourthState));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------");
        }

        static ValueToAccess Compute()
        {
            Console.WriteLine($"The value is being constructed on a thread id {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return new ValueToAccess($"Constructed on a thread id {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Worker(IHasValue state)
        {
            Console.WriteLine($"Worker runs on thread id{ Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"State value:{state.Value.Text}");
        }

        static void Worker(Lazy<ValueToAccess> state)
        {
            Console.WriteLine($"Worker runs on thread id{ Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"State value:{state.Value.Text}");
        }

        public class ValueToAccess
        {
            private readonly string _text;

            public ValueToAccess(string text)
            {
                this._text = text;
            }

            public string Text { get { return _text; } }
        }

        class UnsafeState : IHasValue
        {
            private ValueToAccess _value;

            public ValueToAccess Value {
                get
                {

                    if (_value ==null)
                    {
                        _value = Compute();
                    }
                    return _value;
                }   
            }
        }

        class DoubleCheckedLocking : IHasValue
        {
            private object _syncRoot = new object();
            private volatile ValueToAccess _value;

            ValueToAccess IHasValue.Value {

                get
                {
                    if (_value ==null)
                    {
                        lock (_syncRoot)
                        {
                            if (_value == null) _value = Compute();
                        }
                    }
                    return _value;
                }
            }
        }

        class BLCDoubleChecked : IHasValue
        {
            private object _syncRoot = new object();
            private ValueToAccess _value;
            private bool _initialized = false;

            ValueToAccess IHasValue.Value { get { return LazyInitializer.EnsureInitialized(ref _value,ref _initialized,ref _syncRoot,Compute); } }
        }


        class BLCThreadSafeFactory : IHasValue
        {
            private ValueToAccess _value;

            public ValueToAccess Value {
                get { return LazyInitializer.EnsureInitialized(ref _value, Compute); }
            }
        }

        interface IHasValue { 
            ValueToAccess Value { get; }
        }

    }

    
}
