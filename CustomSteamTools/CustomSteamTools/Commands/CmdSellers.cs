using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdSellers : CmdClassifiedsBase
	{
		public override string[] Aliases => new string[] { "sellers", "buy", "listsell" };

		public override string RegistryName => "sellers";

		public override void RunCommand(CommandHandler handler, List<string> args)
		{
			RunCommand(handler, args, OrderType.Sell);
		}
	}
}
