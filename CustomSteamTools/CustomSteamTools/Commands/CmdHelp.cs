using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

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
					VersatileIO.WriteComplex("&f  {0}: &7{1}".Fmt(cmd.RegistryName, cmd.Description));
				}
			}
			else
			{
				ITradeCommand cmd = sender.FindCommand(cmdname);
				if (cmd == null)
				{
					VersatileIO.WriteLine("No command found by name of " + cmdname, ConsoleColor.Red);
					return;
				}

				bool hasAliases = cmd.Aliases.Length > 1;
				string aliases = cmd.Aliases.ToReadableString((s) => "'" + s + "'", includeBraces: false);
				VersatileIO.WriteComplex("&fCommand '{0}': &7{1}".Fmt(cmd.RegistryName, cmd.Description));
				VersatileIO.WriteComplex("&f  Syntax: &7" + cmd.Syntax);
				if (hasAliases)
				{
					VersatileIO.WriteComplex("&f  Aliases: &7" + aliases);
				}
			}
		}
	}
}
