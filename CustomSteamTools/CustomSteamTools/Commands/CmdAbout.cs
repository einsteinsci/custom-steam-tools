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
		}
	}
}