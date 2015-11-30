using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CustomSteamTools;
using CustomSteamTools.Commands;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace TF2TradingToolkit
{
	public class VersatileFakeConsoleHandler : VersatileHandlerBase
	{
		public ConsoleWindow Window
		{ get; private set; }

		public static string CurrentInput
		{ get; set; }

		public VersatileFakeConsoleHandler(ConsoleWindow window)
		{
			Window = window;

			CommandHandler.Instance.OnPreCommand += CommandHandler_OnPreCommand;
			CommandHandler.Instance.OnPostCommand += CommandHandler_OnPostCommand;
		}

		private void CommandHandler_OnPreCommand(object sender, PreCommandArgs e)
		{
			if (e.Command is CmdDeals)
			{
				Window.OwningWindow.ClassifiedsView.DealsSearchBtn.Dispatcher.Invoke(() => {
					Window.OwningWindow.ClassifiedsView.DealsSearchBtn.IsEnabled = false;
					Window.OwningWindow.ClassifiedsView.DealsSearchBtn.ToolTip =
						"Deals search currently in progress at Console Window.";
				});
			}
		}

		private void CommandHandler_OnPostCommand(object sender, PostCommandArgs e)
		{
			if (e.Command is CmdDeals && !e.Canceled)
			{
				Window.OwningWindow.ClassifiedsView.Dispatcher.Invoke(() => {
					Window.OwningWindow.ClassifiedsView.DealsSearchBtn.IsEnabled = true;
					Window.OwningWindow.ClassifiedsView.RefreshDealsFiltersTooltip();
					Window.OwningWindow.ClassifiedsView.DealsResults.Clear();
				});
			}
		}

		public override double GetDouble(string prompt)
		{
			string input = GetString(prompt);

			double d;
			if (double.TryParse(input, out d))
			{
				return d;
			}

			return 0;
		}

		public override string GetSelection(string prompt, IDictionary<string, object> options)
		{
			foreach (KeyValuePair<string, object> kvp in options)
			{
				LogLine("[{0}]: {1}".Fmt(kvp.Key, kvp.Value.ToString()), ConsoleColor.White);
			}

			string input = GetString(prompt);

			if (input.IsNullOrWhitespace())
			{
				return options.FirstOrDefault().Key;
			}

			return input;
		}

		public override string GetSelectionIgnorable(string prompt, IDictionary<string, object> options)
		{
			foreach (KeyValuePair<string, object> kvp in options)
			{
				LogLine("[{0}]: {1}".Fmt(kvp.Key, kvp.Value.ToString()), ConsoleColor.White);
			}

			string input = GetString(prompt);

			if (input.IsNullOrWhitespace())
			{
				return null;
			}

			return input;
		}

		public override string GetString(string prompt)
		{
			Window.PromptTxt.Dispatcher.Invoke(() => { Window.PromptTxt.Text = prompt; });

			CurrentInput = null;
			Window.Dispatcher.Invoke(() => { Window.ExecuteBtn.IsEnabled = true; });
			LogPart(prompt, ConsoleColor.Blue);

			while (CurrentInput == null)
			{
				Thread.Sleep(50);

				if (Window.Worker.CancellationPending)
				{
					return null;
				}
			}

			LogLine(CurrentInput, ConsoleColor.Blue);
			return CurrentInput;
		}

		public override void LogLine(string line, ConsoleColor color)
		{
			Window.Dispatcher.Invoke(() => {
				Paragraph p = Window.OutputBox.Document.Blocks.LastOrDefault() as Paragraph;
				if (p == null)
				{
					p = new Paragraph();
					p.Margin = new Thickness(0);
					Window.OutputBox.Document.Blocks.Add(p);
				}

				Run r = new Run(line);
				r.Foreground = new SolidColorBrush(color.ToWPFColor());
				p.Inlines.Add(r);

				p = new Paragraph();
				p.Margin = new Thickness(0);
				Window.OutputBox.Document.Blocks.Add(p);

				Window.OutputBox.ScrollToEnd();
			});
		}

		public override void LogPart(string text, ConsoleColor? color)
		{
			Window.Dispatcher.Invoke(() => {
				Paragraph p = Window.OutputBox.Document.Blocks.LastOrDefault() as Paragraph;
				if (p == null)
				{
					p = new Paragraph();
					p.Margin = new Thickness(0);
					Window.OutputBox.Document.Blocks.Add(p);
				}

				Run last = p.Inlines.LastOrDefault() as Run;
				Run r = new Run(text);
				if (color == null)
				{
					if (last != null)
					{
						r.Foreground = last.Foreground;
					}
					else
					{
						r.Foreground = new SolidColorBrush(Colors.White);
					}
				}
				else
				{
					r.Foreground = new SolidColorBrush(color.Value.ToWPFColor());
				}
				p.Inlines.Add(r);

				Window.OutputBox.ScrollToEnd();
			});
		}
	}
}
