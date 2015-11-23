using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools;
using CustomSteamTools.Commands;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.Logging;
using UltimateUtil.UserInteraction;

namespace BackpackTFConsole
{
	public class Program
	{
		public static readonly string logFolder = Path.Combine(Environment.GetEnvironmentVariable("TEMP"),
			"BACKPACK.TF-PRICELIST", "logs");
		public static readonly string logFile = logFolder + "log-" + 
			DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt";

		[STAThread]
		public static void Main(string[] args)
		{
			CustomConsoleColors.SetColor(ConsoleColor.DarkYellow, 219, 102, 0); // strange orange
			CustomConsoleColors.SetColor(ConsoleColor.DarkBlue, 18, 49, 122); // vintage blue
			CustomConsoleColors.SetColor(ConsoleColor.DarkRed, 128, 0, 0); // collector's red
			CustomConsoleColors.SetColor(ConsoleColor.DarkMagenta, 134, 80, 172); // unusual purple
			CustomConsoleColors.SetColor(ConsoleColor.Cyan, 56, 243, 171); // haunted teal
			CustomConsoleColors.SetColor(ConsoleColor.Blue, 0, 131, 255); // "this is where you come in" blue
			CustomConsoleColors.SetColor(ConsoleColor.DarkGray, 80, 80, 80);	// darken verbose gray
			CustomConsoleColors.SetColor(ConsoleColor.Gray, 140, 140, 140);		// to set apart debug gray

			Console.Title = "Trade Helper Console";

			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}

			PresetVersatileConsoleIO.Initialize(ConsoleColor.Blue);
			VersatileIO.MinLogLevel = LogLevel.Verbose;

			LoggerOld.Logging += DebugLog;
			LoggerOld.LoggingComplex += DebugLogComplex;
			LoggerOld.Prompting = DebugPrompt;

			VersatileIO.Info("Starting program...");

			CommandHandler.Initialize();
			CommandHandler.Instance.OnPreCommand += PreCommand;

			DataManager.AutoSetup(true);

			string input = "";
			while (input.ToLower() != "exit")
			{
				Console.WriteLine();
				input = VersatileIO.GetString("datamgr> ");

				if (input.ToLower() != "exit")
				{
					DataManager.RunCommand(input);
				}
			}
		}

		// TODO: Switch from e.Name.ToLower() == "" to e.Command is T
		private static bool PreCommand(object sender, PreCommandArgs e)
		{
			if (e.Name.ToLower() == "bp")
			{
				if (e.Args.Count == 0)
				{
					e.Args.Add(Settings.Instance.HomeSteamID64);
					return false;
				}

				if (e.Args[0].ToLower() == "me")
				{
					e.Args[0] = Settings.Instance.HomeSteamID64;
				}
			}
			if (e.Name.ToLower() == "deals")
			{
				if (e.Args.Count == 0)
				{
					e.Args.Insert(0, Settings.Instance.HomeSteamID64);
					return false;
				}

				if (e.Args[0].ToLower() == "me")
				{
					e.Args[0] = Settings.Instance.HomeSteamID64;
				}
			}
			if (e.Name.ToLower() == "weapons")
			{
				for (int i = 0; i < e.Args.Count; i++)
				{
					if (e.Args[i].ToLower() == "exclude=me")
					{
						e.Args[i] = "exclude=" + Settings.Instance.HomeSteamID64;
						break;
					}
				}
			}

			return false;
		}

		public static string GetTimeStamp()
		{
			return DateTime.Now.ToString("[HH:mm:ss] ");
		}

		[Obsolete]
		private static void DebugLog(object sender, LogEventArgsOld e)
		{
			if (e.Foreground.HasValue)
			{
				Console.ForegroundColor = e.Foreground.Value;
			}

			if (e.Background.HasValue)
			{
				Console.BackgroundColor = e.Background.Value;
			}

			Console.WriteLine("[OLD LOGGING] " + e.Message);

			File.AppendAllLines(logFile, new string[] { GetTimeStamp() + e.Message });
		}

		[Obsolete]
		private static void DebugLogComplex(object sender, LogComplexEventArgs e)
		{
			Console.Write("[OLD LOGGING] ");
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

		[Obsolete]
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
				Console.WriteLine("[OLD LOGGING] " + e.Prompt);
			}
			else
			{
				Console.Write("[OLD LOGGING] " + e.Prompt);
			}

			return Console.ReadLine();
		}
	}
}
