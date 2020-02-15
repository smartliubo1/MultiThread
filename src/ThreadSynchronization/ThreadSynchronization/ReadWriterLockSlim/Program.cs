using System;
using System.Collections.Generic;
using System.Threading;

namespace ReadWriterLockSlimClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Thread(Read) { IsBackground = true }.Start();
            new Thread(Read) { IsBackground = true }.Start();
            new Thread(Read) { IsBackground = true }.Start();

            new Thread(() => Write(" write thread 1"))
            { IsBackground = true }.Start();

            new Thread(() => Write(" write thread 2"))
            { IsBackground = true }.Start();

            Thread.Sleep(TimeSpan.FromSeconds(30));
        }

        static ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        static Dictionary<int, int> _items = new Dictionary<int, int>();

        static void Read()
        {
            Console.WriteLine("Reading content of a dictionary");
            while (true)
            {
                try
                {
                    _readerWriterLockSlim.EnterReadLock();
                    foreach (var key in _items.Keys)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    }
                }
                finally
                {
                    _readerWriterLockSlim.ExitReadLock();
                }
            }
        }

        static void Write(string threadName)
        {
            while (true)
            {
                try
                {
                    int newKey = new Random().Next(250);
                    _readerWriterLockSlim.EnterUpgradeableReadLock();
                    if (!_items.ContainsKey(newKey))
                    {
                        try
                        {
                            _readerWriterLockSlim.EnterWriteLock();
                            _items[newKey] = 1;
                            Console.WriteLine($"New Key {newKey} is added to a dictionary by Thread {threadName}");
                        }
                        finally
                        {
                            _readerWriterLockSlim.ExitWriteLock();
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                }
                finally
                {
                    _readerWriterLockSlim.ExitUpgradeableReadLock();
                }
            }
        }
    }
}
