using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdBuyers : CmdClassifiedsBase
	{
		public override string[] Aliases => new string[] { "buyers", "sell", "listbuy" };

		public override string RegistryName => "buyers";

		public override void RunCommand(CommandHandler handler, List<string> args)
		{
			RunCommand(handler, args, OrderType.Buy);
		}
	}
}
