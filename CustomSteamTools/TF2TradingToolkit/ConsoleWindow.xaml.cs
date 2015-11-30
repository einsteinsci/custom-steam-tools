using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Shapes;
using CustomSteamTools;
using CustomSteamTools.Commands;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for ConsoleWindow.xaml
	/// </summary>
	public partial class ConsoleWindow : Window
	{
		public MainWindow OwningWindow
		{ get; private set; }

		public BackgroundWorker Worker
		{ get; private set; }

		public VersatileFakeConsoleHandler Handler
		{ get; private set; }

		public bool CommandShutdown
		{ get; private set; }

		public bool IsShown
		{ get; private set; }

		public ConsoleWindow(MainWindow window)
		{
			OwningWindow = window;

			InitializeComponent();

			Handler = new VersatileFakeConsoleHandler(this);

			Worker = new BackgroundWorker();
			Worker.WorkerSupportsCancellation = true;
			Worker.DoWork += Worker_DoWork;
			Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			string input = "";
			while (true)
			{
				input = VersatileIO.GetString("toolkit> ");
				if (input != null)
				{
					if (input.EqualsIgnoreCase("exit"))
					{
						Dispatcher.Invoke(() => { Close(); });
					}

					DataManager.RunCommand(input);
					VersatileIO.WriteLine();
				}

				if (Worker.CancellationPending)
				{
					VersatileIO.Info("Command complete. Exiting.");
					CommandShutdown = true;
					return;
				}
			}
		}

		private void ExecuteBtn_Click(object sender, RoutedEventArgs e)
		{
			VersatileFakeConsoleHandler.CurrentInput = InputBox.Text;
			
			InputBox.Text = "";
			ExecuteBtn.IsEnabled = false;
			InputBox.Focus();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			IsShown = true;

			Worker.RunWorkerAsync();
			InputBox.Focus();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (CommandShutdown)
			{
				return;
			}

			Handler.LogLine("Closing as soon as command is completed.", ConsoleColor.Yellow);
			Worker.CancelAsync();
			e.Cancel = true;
		}
	}
}
