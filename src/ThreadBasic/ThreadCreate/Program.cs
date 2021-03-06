﻿using System;
using System.Threading;

namespace ThreadCreate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Thread thread = new Thread(PrintNumbers);
            thread.Start();
            Console.WriteLine("-----------");
            PrintNumbersSync(1);
        }

        static void PrintNumbers()
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine(i);
            }
        }

        static void PrintNumbersSync(int number)
        {
            Console.WriteLine("Starting...");
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"{number}"+i);
            }
        }
    }
}
