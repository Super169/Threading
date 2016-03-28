using System;
using System.Threading;

namespace myEx01
{

    class Program
    {

        private static int nRunCount = 0;
        private static Random rnd = new Random();
        public static bool stopTask = false;
        private static object myLocker = new Object();

        private static void ConsoleWriteLine(ConsoleColor c, string msg, params object[] arg)
        {
            // Prevent other process jump inside and change color
            lock (myLocker)
            {
                ConsoleColor co = Console.ForegroundColor;
                Console.ForegroundColor = c;
                Console.WriteLine(msg, arg);
                Console.ForegroundColor = co;
            }
        }


        static void myTaskA()
        {
            while (true)
            {
                ConsoleWriteLine(ConsoleColor.DarkGreen, "{0}.....myThread: Task A is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
            }
        }

        static void myTaskB()
        {
            while (!stopTask)
            {
                ConsoleWriteLine(ConsoleColor.DarkRed, "{0}.....myThread: Task B is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
            }
            ConsoleWriteLine(ConsoleColor.Red, "Task B start cleanup for stop");
            Thread.Sleep(2000);
            ConsoleWriteLine(ConsoleColor.Red, "Task B completed");
        }

        static void Main(string[] args)
        {
            ConsoleWriteLine(ConsoleColor.White, "Single class threadding");
            ConsoleWriteLine(ConsoleColor.White, "Thread Start/Stop/Join Sample");

            Thread oThread_01 = new Thread(myTaskA);
            Thread oThread_02 = new Thread(myTaskB);

            ConsoleWriteLine(ConsoleColor.Green, "Startng myThread.myTask.....\n");

            oThread_01.Start();
            oThread_02.Start();
            while (!(oThread_01.IsAlive && oThread_02.IsAlive)) ;


            ConsoleWriteLine(ConsoleColor.Yellow, "myThread.myTask started, press a [ENTER] to abort Task A....\n");
            Console.ReadLine();

            oThread_01.Abort();
            oThread_01.Join();  // This Join is just wait for abort finished
            Thread.Sleep(1);

            ConsoleWriteLine(ConsoleColor.Yellow, "myThread.myTaskA stopped, press a [ENTER] to stop Task B....\n");
            Console.ReadLine();

            stopTask = true;
            oThread_02.Join();  // This Join will wait for Task2 to complete
            Thread.Sleep(1);

            Console.WriteLine();
            ConsoleWriteLine(ConsoleColor.Green, "myThread: myTaskA & myTaskB have been stopped");

            try
            {
                ConsoleWriteLine(ConsoleColor.Yellow, "\nTry to restart myTaskA thread");
                oThread_01.Start();
            }
            catch (ThreadStateException ex)
            {
                Console.WriteLine("ThreadStateException trying to restart myThread.myTask. ");
                Console.WriteLine("Expected since aborted threads cannot be restarted, exception message:");
                Console.WriteLine(ex.Message);
            }

            try
            {
                ConsoleWriteLine(ConsoleColor.Yellow, "\nTry to restart myTaskB thread");
                oThread_01.Start();
            }
            catch (ThreadStateException ex)
            {
                Console.WriteLine("ThreadStateException trying to restart myThread.myTask.");
                Console.WriteLine("Expected since completed threads cannot be restarted, exception nessage:");
                Console.WriteLine(ex.Message);
            }

            ConsoleWriteLine(ConsoleColor.Yellow, "Please any key to QUIT");
            Console.ReadKey();

        }
    }
}


