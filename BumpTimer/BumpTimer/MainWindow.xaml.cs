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

		public TimeSpan MaxTime = TimeSpan.FromMinutes(30);

		public const string TIME_FORMAT = "h\\:mm\\:ss";

		bool _hasLoaded = false;

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
			timeSlider.IsEnabled = false;

			timerBar.Value = 0;
			timerBar.Maximum = MaxTime.TotalSeconds;

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
			taskbarItemInfo.ProgressValue = e.ProgressPercentage / MaxTime.TotalSeconds;
			TimeSpan time = TimeSpan.FromSeconds(e.ProgressPercentage + 1);
			timerText.Text = time.ToString(TIME_FORMAT);
			timerBar.ToolTip = time.Minutes.ToString() + ":" + time.Seconds.ToString();
		}

		private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			timerBar.Value = MaxTime.TotalSeconds;
			timerText.Text = MaxTime.ToString(TIME_FORMAT);

			timerResetBtn.Visibility = Visibility.Visible;
			resetThumbBtn.Visibility = Visibility.Visible;
			timeSlider.IsEnabled = true;

			SystemSounds.Asterisk.Play();
			FlashHelper.FlashApplicationWindow();
		}

		private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(2); // to help rounding

			DateTime timeStarted = DateTime.Now;
			for (int i = 0; i <= MaxTime.TotalSeconds * 2; i++)
			{
				Thread.Sleep(500);

				double progress = (i * 500) / MaxTime.TotalMilliseconds;
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

		private void timeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_hasLoaded)
			{
				return;
			}

			MaxTime = TimeSpan.FromMinutes(timeSlider.Value);
			maxTimeText.Text = MaxTime.ToString(TIME_FORMAT);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_hasLoaded = true;
		}
	}
}
