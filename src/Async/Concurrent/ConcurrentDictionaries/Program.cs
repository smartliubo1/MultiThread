using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConcurrentDictionaries
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Concurrent Dictionary example!");

            var dictionary = new Dictionary<int, string>();

            var concurrentDictionary = new ConcurrentDictionary<int, string>();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                lock (dictionary)
                {
                    dictionary[i] = Item;
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"Writing to dictionary with lock :{stopWatch.Elapsed}");

            stopWatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                concurrentDictionary[i] = Item;
            }
            stopWatch.Stop();
            Console.WriteLine($"Writing to a concurrent dictionary :{stopWatch.Elapsed}");

            stopWatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                lock (dictionary)
                {
                    CurrentItem = dictionary[i];
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading from dictionary with a lock:{stopWatch.Elapsed}");

            stopWatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                CurrentItem = concurrentDictionary[i];
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading from a concurrent dictionary:{stopWatch.Elapsed}");

        }

        const string Item = "Dictionary item";

        public static string CurrentItem;
    }
}
