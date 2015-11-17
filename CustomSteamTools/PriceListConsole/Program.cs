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
			CustomConsoleColors.SetColor(ConsoleColor.DarkYellow, 255, 149, 63); // strange orange
			CustomConsoleColors.SetColor(ConsoleColor.DarkBlue, 82, 113, 165); // vintage blue
			CustomConsoleColors.SetColor(ConsoleColor.DarkRed, 166, 40, 40); // collector's red
			CustomConsoleColors.SetColor(ConsoleColor.DarkMagenta, 120, 70, 165); // unusual purple
			CustomConsoleColors.SetColor(ConsoleColor.Cyan, 60, 255, 190); // haunted teal

			Console.Title = "Trade Helper Console";
			Console.ForegroundColor = ConsoleColor.White;

			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}

			LoggerOld.Logging += DebugLog;
			LoggerOld.LoggingComplex += DebugLogComplex;
			LoggerOld.Prompting = DebugPrompt;

			LoggerOld.Log("Starting program...");

			CommandHandler.Instance.OnPreCommand += PreCommand;

			DataManager.AutoSetup(true, true);

			string input = "";
			while (input.ToLower() != "exit")
			{
				Console.WriteLine();
				input = LoggerOld.GetInput("datamgr> ", false, true);

				if (input.ToLower() != "exit")
				{
					DataManager.RunCommand(input);
				}
			}
		}

		private static bool PreCommand(object sender, PreCommandArgs e)
		{
			if (e.Name.ToLower() == "bp")
			{
				if (e.Args.Count == 0)
				{
					e.Args.Add(DataManager.SEALEDINTERFACE_STEAMID);
					return false;
				}

				if (e.Args[0].ToLower() == "me")
				{
					e.Args[0] = DataManager.SEALEDINTERFACE_STEAMID;
				}
			}
			if (e.Name.ToLower() == "deals")
			{
				if (e.Args.Count == 0)
				{
					e.Args.Insert(0, DataManager.SEALEDINTERFACE_STEAMID);
					return false;
				}

				if (e.Args[0].ToLower() == "me")
				{
					e.Args[0] = DataManager.SEALEDINTERFACE_STEAMID;
				}
			}
			if (e.Name.ToLower() == "weapons")
			{
				for (int i = 0; i < e.Args.Count; i++)
				{
					if (e.Args[i].ToLower() == "exclude=me")
					{
						e.Args[i] = "exclude=" + DataManager.SEALEDINTERFACE_STEAMID;
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
