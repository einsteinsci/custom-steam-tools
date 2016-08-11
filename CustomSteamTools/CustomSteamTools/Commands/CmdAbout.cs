using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdAbout : ITradeCommand
	{
		public static string AsmVersion
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyFileVersionAttribute ver = asm.GetCustomAttribute<AssemblyFileVersionAttribute>();

				return "v" + ver.Version;
			}
		}

		public static string AsmDescription
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyDescriptionAttribute desc = asm.GetCustomAttribute<AssemblyDescriptionAttribute>();

				return desc.Description;
			}
		}

		public static string AsmCopyright
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyCopyrightAttribute tm = asm.GetCustomAttribute<AssemblyCopyrightAttribute>();

				return tm.Copyright;
			}
		}

		public string RegistryName => "about";

		public string[] Aliases => new string[] { "about" };

		public string Description => "Displays information about the program.";

		public string Syntax => "about";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			VersatileIO.Write("Custom Steam Tools Library ", ConsoleColor.Green);
			VersatileIO.WriteLine(AsmVersion, ConsoleColor.Yellow);
			VersatileIO.WriteLine(AsmDescription);
			VersatileIO.WriteLine(AsmCopyright, ConsoleColor.Gray);

			VersatileIO.WriteLine("License: GPL v3 (link below)");
			VersatileIO.WriteLine("  https://github.com/einsteinsci/custom-steam-tools/blob/master/LICENSE.txt", ConsoleColor.Gray);
			VersatileIO.WriteLine();

			VersatileIO.WriteLine("Special Thanks:", ConsoleColor.Green);
			VersatileIO.WriteLine();

			VersatileIO.WriteLine("JSON.NET for all the JSON parsing");
			VersatileIO.WriteLine("  Project: http://www.newtonsoft.com/json", ConsoleColor.Gray);
			VersatileIO.WriteLine("  License: https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md", ConsoleColor.Gray);

			VersatileIO.WriteLine("The HTML Agility Pack for all the data scraping");
			VersatileIO.WriteLine("  Project: https://htmlagilitypack.codeplex.com/", ConsoleColor.Gray);
			VersatileIO.WriteLine("  License: https://htmlagilitypack.codeplex.com/license", ConsoleColor.Gray);

			VersatileIO.WriteLine("Backpack.tf for tons of trading data");
			VersatileIO.WriteLine("  URL: https://backpack.tf/", ConsoleColor.Gray);

			VersatileIO.WriteLine("And of course, Valve for TF2");
			VersatileIO.WriteLine("  URL: http://www.teamfortress.com/", ConsoleColor.Gray);
		}
	}
}