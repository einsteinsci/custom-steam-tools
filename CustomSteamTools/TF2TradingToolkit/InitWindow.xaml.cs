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

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for InitWindow.xaml
	/// </summary>
	public partial class InitWindow : Elysium.Controls.Window
	{
		public double InitProgressVal
		{
			get
			{
				int pct = LoadedPercent;
				if (SettingsComplete)
				{
					pct += 20;
				}

				return pct / 120.0;
			}
		}

		public int LoadedPercent
		{ get; private set; }

		public bool SettingsComplete
		{ get; private set; }

		public BackgroundWorker InitWorker
		{ get; private set; }

		public InitWindow()
		{
			InitializeComponent();
			
			DialogResult = null;
		}

		public sealed class InitVersatileHandler : VersatileHandlerBase
		{
			public override double GetDouble(string prompt)
			{
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}

			public override void LogLine(string line, ConsoleColor color)
			{
				throw new NotImplementedException();
			}

			public override void LogPart(string text, ConsoleColor? color)
			{
				throw new NotImplementedException();
			}
		}
	}
}
