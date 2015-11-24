﻿using System;
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
			"CUSTOM-STEAM-TOOLS", "logs");

		[Obsolete]
		public static readonly string logFile = Path.Combine(logFolder, "log-" + 
			DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt");

		public static ConsoleVersatileHandler VersatileHandler
		{ get; set; }

		[STAThread]
		public static void Main(string[] args)
		{
			CustomConsoleColors.SetColor(ConsoleColor.DarkYellow, 219, 102, 0);		// strange orange
			CustomConsoleColors.SetColor(ConsoleColor.DarkBlue, 18, 49, 122);		// vintage blue
			CustomConsoleColors.SetColor(ConsoleColor.DarkRed, 128, 0, 0);			// collector's red
			CustomConsoleColors.SetColor(ConsoleColor.DarkMagenta, 134, 80, 172);	// unusual purple
			CustomConsoleColors.SetColor(ConsoleColor.Cyan, 56, 243, 171);			// haunted teal
			CustomConsoleColors.SetColor(ConsoleColor.Blue, 0, 131, 255);			// "this is where you come in" blue
			CustomConsoleColors.SetColor(ConsoleColor.DarkGray, 80, 80, 80);		// darken verbose gray
			CustomConsoleColors.SetColor(ConsoleColor.Gray, 140, 140, 140);			//   to differentiate from debug gray

			Console.Title = "Trade Helper Console";

			Directory.CreateDirectory(logFolder);

			VersatileHandler = new ConsoleVersatileHandler(logFolder);
			VersatileIO.SetHandler(VersatileHandler);
			VersatileIO.MinLogLevel = LogLevel.Verbose;

			VersatileIO.Info("Starting program...");

			CommandHandler.Initialize();
			CommandHandler.Instance.OnPreCommand += PreCommand;

			DataManager.AutoSetup(true);
			CmdDeals.DoBeepOnFinished = true;

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

		// might be used in the future
		private static bool PreCommand(object sender, PreCommandArgs e)
		{
			return false;
		}
	}
}
