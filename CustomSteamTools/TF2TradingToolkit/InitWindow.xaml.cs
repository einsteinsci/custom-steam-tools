using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using UltimateUtil.UserInteraction;
using UltimateUtil;
using UltimateUtil.Fluid;
using CustomSteamTools;
using CustomSteamTools.Commands;
using System.Threading;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for InitWindow.xaml
	/// </summary>
	public partial class InitWindow : MahApps.Metro.Controls.MetroWindow
	{
		public sealed class InitVersatileHandler : VersatileHandlerBase
		{
			InitWindow _window;

			public InitVersatileHandler(InitWindow window)
			{
				_window = window;
			}

			public override double GetDouble(string prompt)
			{
				double? res = null;
				while (res == null)
				{
					InputNumberWindow inw = new InputNumberWindow(prompt);
					inw.ShowDialog();

					res = inw.InputResult;
				}

				return res.Value;
			}

			public override string GetSelection(string prompt, IDictionary<string, object> options)
			{
				throw new NotImplementedException();
			}

			public override string GetSelectionIgnorable(string prompt, IDictionary<string, object> options)
			{
				throw new NotImplementedException();
			}

			public override string GetString(string prompt)
			{
				InputStringWindow isw = new InputStringWindow(prompt);
				isw.ShowDialog();

				return isw.InputResult ?? "";
			}

			public override void LogLine(string line, ConsoleColor color)
			{
				line = line.Replace("\n", Environment.NewLine);

				_window.LogBox.Dispatcher.Invoke(() => {
					_window.LogBox.Text += line + Environment.NewLine;
					_window.LogBox.ScrollToEnd();
				});

				_window.LastItemText.Dispatcher.Invoke(() => {
					if (line.IsNullOrWhitespace())
					{
						_window.LastItemText.Text = "Loading...";
					}
					else
					{
						_window.LastItemText.Text = line.Trim();
					}
				});
			}

			public override void LogPart(string text, ConsoleColor? color)
			{
				text = text.Replace("\n", Environment.NewLine);

				_window.LogBox.Dispatcher.Invoke(() => {
					_window.LogBox.Text += text;
					_window.LogBox.ScrollToEnd();
				});
			}
		}

		public BackgroundWorker InitWorker
		{ get; private set; }

		public InitVersatileHandler Handler
		{ get; private set; }

		private bool _cancelled = false;

		private string _retrieval;

		public bool Force
		{ get; private set; }

		public InitWindow(bool force, string ret = null)
		{
			InitializeComponent();
			_retrieval = ret;
			Force = force;

			Handler = new InitVersatileHandler(this);

			InitWorker = new BackgroundWorker();
			InitWorker.WorkerReportsProgress = true;

			InitWorker.ProgressChanged += InitWorker_ProgressChanged;
			InitWorker.DoWork += InitWorker_DoWork;
			InitWorker.RunWorkerCompleted += InitWorker_RunWorkerCompleted;
		}

		private void InitWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			DialogResult = !_cancelled;
			Close();
		}

		private void InitWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			VersatileIO.WriteLine();
			VersatileIO.MinLogLevel = UltimateUtil.Logging.LogLevel.Verbose;

			CommandHandler.Initialize();

			if (_retrieval == null)
			{
				DataManager.AutoSetup(Force, InitWorker);
			}
			else if (_retrieval == "schema")
			{

			}
		}

		private void InitWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			InitProgress.Value = e.ProgressPercentage;

			double p = e.ProgressPercentage / 100.0;
			TaskbarInfo.ProgressValue = p;

			if (e.UserState is bool)
			{
				if ((bool)e.UserState)
				{
					TaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
				}
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (DialogResult == true)
			{
				return;
			}

			e.Cancel = true;
		}

		private void ToggleLog_Click(object sender, RoutedEventArgs e)
		{
			if (LogBox.Visibility.IsAnyOf(Visibility.Collapsed, Visibility.Hidden))
			{
				LogBox.Visibility = Visibility.Visible;
				ToggleLog.Content = "Hide Log";
			}
			else
			{
				LogBox.Visibility = Visibility.Collapsed;
				ToggleLog.Content = "Show Log";
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
			VersatileIO.SetHandler(Handler, true);
			InitWorker.RunWorkerAsync();
		}

		private void CancelSetupBtn_Click(object sender, RoutedEventArgs e)
		{
			_cancelled = true;
			InitWorker.CancelAsync();
		}
	}
}
