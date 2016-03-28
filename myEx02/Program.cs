using System;
using System.Threading;

namespace myEx02
{

    public static class myUtil
    {
        private static object myLocker = new Object();

        public static void ConsoleWriteLine(ConsoleColor c, string msg, params object[] arg)
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
    }

    public class myThread
    {
        private static int nRunCount = 0;
        private static Random rnd = new Random();
        public static bool stopTask = false;

        public void myTaskA()
        {
            while (true)
            {
                myUtil.ConsoleWriteLine(ConsoleColor.DarkGreen, "{0}.....myThread: Task A is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
            }
        }

        public void myTaskB()
        {
            while (!stopTask)
            {
                myUtil.ConsoleWriteLine(ConsoleColor.DarkRed, "{0}.....myThread: Task B is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
            }
            myUtil.ConsoleWriteLine(ConsoleColor.Red, "Task B start cleanup for stop");
            Thread.Sleep(2000);
            myUtil.ConsoleWriteLine(ConsoleColor.Red, "Task B completed");


        }

    }

    class Program
    {

        static void Main(string[] args)
        {
            myUtil.ConsoleWriteLine(ConsoleColor.White, "Threadding on other class");
            myUtil.ConsoleWriteLine(ConsoleColor.White, "Thread Start/Stop/Join Sample");

            myThread oMyThread = new myThread();
            Thread oThread_01 = new Thread(new ThreadStart(oMyThread.myTaskA));
            Thread oThread_02 = new Thread(new ThreadStart(oMyThread.myTaskB));

            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Startng myThread.myTask.....\n");

            oThread_01.Start();
            oThread_02.Start();
            while (!(oThread_01.IsAlive && oThread_02.IsAlive)) ;
            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "myThread.myTask started, press a [ENTER] to abort Task A....\n");
            Console.ReadLine();

            oThread_01.Abort();
            oThread_01.Join();  // This Join is just wait for abort finished
            Thread.Sleep(1);

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "myThread.myTaskA stopped, press a [ENTER] to stop Task B....\n");
            Console.ReadLine();

            myThread.stopTask = true;
            oThread_02.Join();  // This Join will wait for Task2 to complete
            Thread.Sleep(1);

            Console.WriteLine();
            myUtil.ConsoleWriteLine(ConsoleColor.Green, "myThread: myTaskA & myTaskB have been stopped");

            try
            {
                myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "\nTry to restart myTaskA thread");
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
                myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "\nTry to restart myTaskB thread");
                oThread_02.Start();
            }
            catch (ThreadStateException ex)
            {
                Console.WriteLine("ThreadStateException trying to restart myThread.myTask.");
                Console.WriteLine("Expected since completed threads cannot be restarted, exception nessage:");
                Console.WriteLine(ex.Message);
            }

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Please any key to QUIT");
            Console.ReadKey();

        }
    }
}


