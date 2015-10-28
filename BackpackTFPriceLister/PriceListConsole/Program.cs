using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using BackpackTFPriceLister;
using BackpackTFPriceLister.Core;
using BackpackTFPriceLister.Utils;

namespace BackpackTFConsole
{
	public class Program
	{
		public static readonly string logFolder = Environment.GetEnvironmentVariable("TEMP") +
			"\\BACKPACK.TF-PRICELIST\\logs\\";
        public static readonly string logFile = logFolder + "log-" + 
			DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt";

		[STAThread]
		public static void Main(string[] args)
		{
			Console.Title = "Trade Helper Console";
			Console.ForegroundColor = ConsoleColor.White;

			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}

			Logger.Logging += DebugLog;
			Logger.LoggingComplex += DebugLogComplex;
			Logger.Prompting = DebugPrompt;

			Logger.Log("Starting program...");

			CommandHandler.PreCommand += PreCommand;

			DataManager.AutoSetup(true, true);

			string input = "";
			while (input.ToLower() != "exit")
			{
				Console.WriteLine();
				input = Logger.GetInput("datamgr> ", false, true);

				if (input.ToLower() != "exit")
				{
					DataManager.RunCommand(input);
				}
			}
		}

		private static bool PreCommand(string commandName, List<string> args)
		{
			if (commandName.ToLower() == "bp")
			{
				if (args.Count == 0)
				{
					args.Add(DataManager.SEALEDINTERFACE_STEAMID);
					return false;
				}
			}
			if (commandName.ToLower() == "deals")
			{
				if (args.Count == 0)
				{
					args.Insert(0, DataManager.SEALEDINTERFACE_STEAMID);
					return false;
				}

				if (args[0].ToLower() == "me")
				{
					args[0] = DataManager.SEALEDINTERFACE_STEAMID;
				}
			}

			return false;
		}

		public static string GetTimeStamp()
		{
			return DateTime.Now.ToString("[HH:mm:ss] ");
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

			File.AppendAllLines(logFile, new string[] { GetTimeStamp() + e.Message });
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

			File.AppendAllLines(logFile, new string[] { GetTimeStamp() + e.Unformatted });
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
