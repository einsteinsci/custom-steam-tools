using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Utils;
using UltimateUtil;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdHelp : ITradeCommand
	{
		public string[] Aliases => new string[] { "help", "?" };

		public string Description => "Lists all commands, or provides info on a command.";

		public string RegistryName => "help";

		public string Syntax => "help [command]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			string cmdname = args.FirstOrDefault();

			if (cmdname == null)
			{
				foreach (ITradeCommand cmd in sender.Commands)
				{
					LoggerOld.Log(" - {0}: {1}".Fmt(cmd.RegistryName, cmd.Description), ConsoleColor.White);
				}
			}
			else
			{
				ITradeCommand cmd = sender.FindCommand(cmdname);
				if (cmd == null)
				{
					LoggerOld.Log("No command found by name of " + cmdname, ConsoleColor.Red);
					return;
				}

				bool hasAliases = cmd.Aliases.Length > 1;
				string aliases = cmd.Aliases.ToReadableString((s) => "'" + s + "'", includeBraces: false);
				LoggerOld.Log("Command {0}: {1}".Fmt(cmd.RegistryName, cmd.Description));
				LoggerOld.Log(" - Syntax: " + cmd.Syntax);
				if (hasAliases)
				{
					LoggerOld.Log(" - Aliases: " + aliases);
				}
			}
		}
	}
}
