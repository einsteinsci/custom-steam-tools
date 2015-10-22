using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackpackTFPriceLister;
using System.Windows.Forms;
using System.IO;
using System.Net;
using BackpackTFPriceLister.ItemDataJson;
using BackpackTFPriceLister.PriceDataJson;

namespace BackpackTFConsole
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Console.Title = "Backpack.tf Console";
			Console.ForegroundColor = ConsoleColor.White;
			Logger.Logging += DebugLog;
			Logger.LoggingComplex += DebugLogComplex;
			Logger.Prompting = DebugPrompt;

			CommandHandler.PreCommand += PreCommand;

			PriceLister.AutoSetup(true, true);

			string input = "";
			while (input.ToLower() != "exit")
			{
				Console.WriteLine();
				input = DebugPrompt(null, new PromptEventArgs("bp.tf Console> "));

				if (input.ToLower() != "exit")
				{
					PriceLister.RunCommand(input);
				}
			}
		}

		private static bool PreCommand(string commandName, List<string> args)
		{
			if (commandName.ToLower() == "bp")
			{
				if (args.Count == 0)
				{
					args.Add(PriceLister.SEALEDINTERFACE_STEAMID);
					return false;
				}
			}
			if (commandName.ToLower() == "deals")
			{
				if (args.Count == 0)
				{
					args.Insert(0, PriceLister.SEALEDINTERFACE_STEAMID);
					return false;
				}

				if (args[0].ToLower() == "me")
				{
					args[0] = PriceLister.SEALEDINTERFACE_STEAMID;
				}
			}

			return false;
		}

		private static void DebugLog(object sender, LogEventArgs e)
		{
			if (e.Foreground.HasValue)
			{
				Console.ForegroundColor = e.Foreground.Value;
			}

			if (e.Background.HasValue)
			{
				Console.BackgroundColor = e.Background.Value;
			}

			Console.WriteLine(e.Message);
		}

		private static void DebugLogComplex(object sender, LogComplexEventArgs e)
		{
			foreach (object obj in e.Arguments)
			{
				if (obj is ConsoleColor)
				{
					Console.ForegroundColor = (ConsoleColor)obj;
					continue;
				}

				if (obj is string)
				{
					Console.Write(obj as string);
					continue;
				}
			}

			Console.WriteLine();
		}

		private static string DebugPrompt(object sender, PromptEventArgs e)
		{
			if (e.Foreground.HasValue)
			{
				Console.ForegroundColor = e.Foreground.Value;
			}

			if (e.Background.HasValue)
			{
				Console.ForegroundColor = e.Background.Value;
			}

			if (e.NewlineAfterPrompt)
			{
				Console.WriteLine(e.Prompt);
			}
			else
			{
				Console.Write(e.Prompt);
			}

			return Console.ReadLine();
		}
	}
}
