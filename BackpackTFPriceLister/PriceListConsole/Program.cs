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

		private static void DebugLog(object sender, LogEventArgs e)
		{
			switch (e.Type)
			{
				case MessageType.Normal:
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case MessageType.Debug:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				case MessageType.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case MessageType.Emphasis:
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}

			Console.WriteLine(e.Message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static string DebugPrompt(object sender, PromptEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			if (e.NewlineAfterPrompt)
			{
				Console.WriteLine(e.Prompt);
			}
			else
			{
				Console.Write(e.Prompt);
			}

			string res = Console.ReadLine();
			Console.ForegroundColor = ConsoleColor.White;

			return res;
		}
	}
}
