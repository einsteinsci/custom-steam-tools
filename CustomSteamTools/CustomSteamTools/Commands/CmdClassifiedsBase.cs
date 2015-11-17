using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Items;

namespace CustomSteamTools.Commands
{
	public abstract class CmdClassifiedsBase : ITradeCommand
	{
		public abstract string[] Aliases
		{ get; }

		public abstract string RegistryName
		{ get; }

		public virtual string Syntax => RegistryName + " [/verify] {defindex | itemName | searchQuery}";

		public virtual string Description => "Gets the backpack.tf " + RegistryName + " listings for an item";

		public void RunCommand(CommandHandler handler, List<string> args, OrderType orderType)
		{

		}

		public abstract void RunCommand(CommandHandler handler, List<string> args);

		public ListingProperties GetValidProperties(Item item)
		{
			ListingProperties res = new ListingProperties();
			return res;
		}
	}
}
