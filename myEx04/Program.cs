using System;
using System.Threading;

namespace myEx04
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
        private WaitHandle waitHandle;

        private static int nRunCount = 0;
        private static Random rnd = new Random();
        public static bool stopTask = false;
        private Thread taskThread = null;

        public void doSomeWork(WaitHandle waitHandle)
        {
            this.waitHandle = waitHandle;

            this.taskThread = new Thread(new ThreadStart(myTaskA));
            this.taskThread.Start();
        }

        public void Join()
        {
            if (taskThread != null) taskThread.Join();
        }

        public void myTaskA()
        {
            this.waitHandle.WaitOne();
            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Suspeding a thread using ManualResetEvent.....\n");
            Thread f = new Thread(new ThreadStart(myTaskB));
            f.Start();
            while (!f.IsAlive) ;
            this.waitHandle.WaitOne();

            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Task B started\n");
            while (!stopTask)
            {
                myUtil.ConsoleWriteLine(ConsoleColor.DarkGreen, "{0}.....myThread: Task A is running.", ++nRunCount);
                Thread.Sleep(rnd.Next(100, 1000));
                this.waitHandle.WaitOne();
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
            // Note: ManualResetEvent vs AutoResetEvent:
            //     - ManualResetEvent : remains signals until is is manually reset.
            //     - AutoResetEvent   : remains signaled until a single waiting thread is released, and then automatically returns to the non-signaled state.
            // i.e. For AutoResetEvent, it need to Set let the thread execute, and it will stop in next WaitOne statement and wait for another Set.
            //      For ManualResetEvent, only Set, the thread can continue run unitl it has been reset again. 
            ManualResetEvent waitHandle = new ManualResetEvent(true);

            myUtil.ConsoleWriteLine(ConsoleColor.White, "Start a thread inside the thread");
            myUtil.ConsoleWriteLine(ConsoleColor.White, "Thread Start/Stop/Join Sample");

            myThread oMyThread = new myThread();
            // Thread oThread = new Thread(new ThreadStart(oMyThread.myTaskA));

            oMyThread.doSomeWork(waitHandle);

            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Startng myThread.myTaskA.....\n");

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Task A & B started, press a [ENTER] to suspend Task A....\n");
            Console.ReadLine();
            waitHandle.Reset();

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Task A suspended, press a [ENTER] to resume Task A....\n");
            Console.ReadLine();
            waitHandle.Set();


            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "Task A resumed, press a [ENTER] to stop Task A & B....\n");
            Console.ReadLine();
            
            myUtil.ConsoleWriteLine(ConsoleColor.Green, "Start stopping threads, please wait......");

            myThread.stopTask = true;

            // Wait for Task A complete
            oMyThread.Join();

            myUtil.ConsoleWriteLine(ConsoleColor.Yellow, "All thread stopped, please any key to QUIT");
            Console.ReadKey();

        }
    }
}


