using System;
using System.Threading;

namespace myEx03
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
            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Startng Task B inside Task A.....\n");
            Thread f = new Thread(new ThreadStart(myTaskB));
            f.Start();
            while (!f.IsAlive);
            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Task B started\n");

            while (!stopTask)
            {
                myUtil.ConsoleWriteLine(ConsoleColor.DarkGreen, "{0}.....myThread: Task A is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
            }
            myUtil.ConsoleWriteLine(ConsoleColor.Red, "Task A start waiting for Task B");
            f.Join();
            myUtil.ConsoleWriteLine(ConsoleColor.Red, "Task A continue to quit");
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
            myUtil.ConsoleWriteLine(ConsoleColor.White, "Start a thread inside the thread");
            myUtil.ConsoleWriteLine(ConsoleColor.White, "Thread Start/Stop/Join Sample");

            myThread oMyThread = new myThread();
            Thread oThread = new Thread(new ThreadStart(oMyThread.myTaskA));

            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Startng myThread.myTaskA.....\n");

            oThread.Start();
            while (!oThread.IsAlive) ;
            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Task A started, press a [ENTER] to stop Task A & B....\n");
            Console.ReadLine();

            myThread.stopTask = true;

            // Wait for Task A complete
            oThread.Join();  

            myUtil.ConsoleWriteLine(ConsoleColor.Green, "myThread: myTask A & B have been stopped");

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Please any key to QUIT");
            Console.ReadKey();

        }
    }
}


