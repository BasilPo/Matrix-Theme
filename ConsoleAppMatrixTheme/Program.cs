using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleAppMatrixTheme
{
    class Matrix
    {
        //object for locking
        static object forLock = new object();
        static Random random = new Random();
        static List<int> columnList = new List<int>();

        static void ChainGeneration()
        {
            //the thread is "sleeping" - so that chains at different periods of time began to move
            Thread.Sleep(random.Next(10, 5000));
            Console.CursorVisible = false;
            //setting chain properties
            int length, speed, column = 0, columnRandom;
            //array of column indexes (0 to WindowWidth)
            int[] columns = new int[Console.WindowWidth];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = i;
            while (true)
            {
                //and within the thread and within the same fall cycle - the chain will have different properties
                length = random.Next(3, 11);
                speed = random.Next(10, 200);
                //search unallocated column
                for (int i = columns.Length - 1, j, tmp; i >= 1; i--) 
                {
                    j = random.Next(i + 1); //Fisher-Yates Shuffle
                    tmp = columns[j];
                    columns[j] = columns[i];
                    columns[i] = tmp;
                }
                columnRandom = columns.Length;
                while (columnRandom-- > 0) 
                {
                    column = columns[columnRandom];
                    //because the List is not thread safe ...
                    lock (forLock)
                    {
                        if (!columnList.Contains(column)) 
                        {
                            columnList.Add(column);
                            break;
                        }
                    }
                }
                for (int step = 0; step < Console.WindowHeight; step++)
                {
                    for (int row = 0; row <= step; row++)
                    {
                        lock (forLock)
                        {
                            Console.SetCursorPosition(column, row);
                            if (row < step - length)
                                Console.WriteLine(' ');
                            else
                            {
                                //setting color and character type
                                switch (row - step)
                                {
                                    case 0:
                                        Console.ForegroundColor = ConsoleColor.White;
                                        break;
                                    case -1:
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        break;
                                    default:
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        break;
                                }
                                Console.WriteLine((char)random.Next(33, SByte.MaxValue));
                            }
                            //when the chain reaches the end - it disappears
                            if (step == Console.WindowHeight - 1 && step == row)
                            {
                                for (int i = row - length; i < Console.WindowHeight; i++)
                                {
                                    Console.SetCursorPosition(column, i);
                                    Console.WriteLine(' ');
                                }
                            }
                        }
                    }
                    Thread.Sleep(speed);
                }
                //make this column available again for threads
                lock (forLock)
                {
                    columnList.Remove(column);
                }
            }
        }

        static void Main(string[] args)
        {
            //Clear Console
            Console.Clear();
            //array of threads - the number of threads corresponds to a third of the width of the console
            Thread[] threads = new Thread[Console.WindowWidth / 3];
            //launching threads
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ChainGeneration);
                threads[i].Start();
            }
        }
    }
}