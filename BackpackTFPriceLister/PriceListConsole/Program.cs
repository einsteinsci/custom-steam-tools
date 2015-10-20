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
using Newtonsoft.Json;
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
