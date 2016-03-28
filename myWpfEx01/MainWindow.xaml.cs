using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace myWpfEx01
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ManualResetEvent waitHandler = new ManualResetEvent(true);

        enum CounterStatus
        {
            Idle, Running, Paused
        }

        private CounterStatus currStatus = CounterStatus.Idle;
        private int myCounter = 0;
        private Thread myCounterThread = null;

        private void setStatus(CounterStatus newStatus)
        {
            currStatus = newStatus;
            btnStart.IsEnabled = (currStatus == CounterStatus.Idle);
            btnPause.IsEnabled = ((currStatus == CounterStatus.Running) || (currStatus == CounterStatus.Paused));
            btnStop.IsEnabled = ((currStatus == CounterStatus.Running) || (currStatus == CounterStatus.Paused));
            if (currStatus == CounterStatus.Running) btnPause.Content = "PAUSE";
            else if (currStatus == CounterStatus.Paused) btnPause.Content = "RESUME";
            else btnPause.Content = "";
        }

        public MainWindow()
        {
            InitializeComponent();
            setStatus(CounterStatus.Idle);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            myCounter = 0;
            txtCounter.Text = "0";
            myCounterThread = new Thread(updateCount);
            setStatus(CounterStatus.Running);
            myCounterThread.Start();
            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stopCount();
        }

        private void updateCount()
		{
			while (currStatus == CounterStatus.Running)
			{
                Thread.Sleep(1000);
                myCounter++;
				Application.Current.Dispatcher.BeginInvoke( System.Windows.Threading.DispatcherPriority.Normal,
		                                                    (Action)(() => txtCounter.Text = myCounter.ToString()) );
                waitHandler.WaitOne();
			}
		}

        private void stopCount()
        {
            if ((currStatus == CounterStatus.Running) || (currStatus == CounterStatus.Paused)) 
            {
                setStatus(CounterStatus.Idle);
                if (myCounterThread != null)
                {
                    // use abort to stop it immedicately
                    myCounterThread.Abort();
                    myCounterThread.Join();
                }
                myCounterThread = null;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopCount();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (currStatus == CounterStatus.Running)
            {
                setStatus(CounterStatus.Paused);
                waitHandler.Reset();

            } else if (currStatus == CounterStatus.Paused)
            {
                setStatus(CounterStatus.Running);
                waitHandler.Set();

            }
        }
    }

}
