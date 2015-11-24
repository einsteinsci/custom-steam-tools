using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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

namespace BumpTimer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public BackgroundWorker WorkerThread
		{ get; private set; }

		public FlashWindowHelper FlashHelper
		{ get; private set; }

		public readonly TimeSpan MAX_TIME = TimeSpan.FromMinutes(30);

		public MainWindow()
		{
			InitializeComponent();
			FlashHelper = new FlashWindowHelper(Application.Current);

			TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
		}

		private void timerResetBtn_Click(object sender, RoutedEventArgs e)
		{
			timerResetBtn.Visibility = Visibility.Hidden;
			resetThumbBtn.Visibility = Visibility.Collapsed;

			timerBar.Value = 0;
			timerBar.Maximum = MAX_TIME.TotalSeconds;

			timerText.Text = "00:00";

			WorkerThread.RunWorkerAsync();
		}

		private void resetThumbBtn_Click(object sender, EventArgs e)
		{
			timerResetBtn_Click(sender, null);
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			WorkerThread = new BackgroundWorker();
			WorkerThread.WorkerSupportsCancellation = true;
			WorkerThread.WorkerReportsProgress = true;

			WorkerThread.DoWork += WorkerThread_DoWork;
			WorkerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;
			WorkerThread.ProgressChanged += WorkerThread_ProgressChanged;
		}

		private void WorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			timerBar.Value = e.ProgressPercentage;
			taskbarItemInfo.ProgressValue = e.ProgressPercentage / MAX_TIME.TotalSeconds;
			TimeSpan time = TimeSpan.FromSeconds(e.ProgressPercentage + 1);
			timerText.Text = time.ToString("mm\\:ss");
			timerBar.ToolTip = time.Minutes.ToString() + ":" + time.Seconds.ToString();
		}

		private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			timerBar.Value = MAX_TIME.TotalSeconds;
			timerText.Text = MAX_TIME.ToString("mm\\:ss");

			timerResetBtn.Visibility = Visibility.Visible;
			resetThumbBtn.Visibility = Visibility.Visible;

			SystemSounds.Asterisk.Play();
			FlashHelper.FlashApplicationWindow();
		}

		private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(2); // to help rounding

			DateTime timeStarted = DateTime.Now;
			for (int i = 0; i <= MAX_TIME.TotalSeconds * 2; i++)
			{
				if (i > 6)
				{
					int stuppid = 0;
				}

				Thread.Sleep(500);

				double progress = (i * 500) / MAX_TIME.TotalMilliseconds;
				int pct = (int)Math.Min(progress * 100.0, 100.0);
				BackgroundWorker bg = sender as BackgroundWorker;

				int totalSecs = (int)(i / 2.0);
				bg.ReportProgress(totalSecs);
			}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			FlashHelper.StopFlashing();
		}
	}
}
