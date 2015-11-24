using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdRange : ITradeCommand
	{
		public string[] Aliases => new string[] { "range", "list" };

		public string Description => "Lists all items within a given price range and match a given filters";

		public string RegistryName => "range";

		public string Syntax => "range {priceMinRef} {priceMaxRef} " + Filters.GetSyntax(false);

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.Count < 2)
			{
				VersatileIO.Error("Syntax: " + Syntax);
				return;
			}

			Price min, max;
			if (!Price.TryParse(args[0], out min))
			{
				VersatileIO.Error("Invalid price: " + args[0]);
				return;
			}
			if (!Price.TryParse(args[1], out max))
			{
				VersatileIO.Error("Invalid price: " + args[1]);
			}
			PriceRange range = new PriceRange(min, max);

			Filters filters = new Filters();
			for (int i = 2; i < args.Count; i++)
			{
				filters.HandleArg(args[i]);
			}

			VersatileIO.Debug("Searching pricings...");
			List<ItemPricing> res = GetInRange(range, filters);
			foreach (ItemPricing p in res)
			{
				VersatileIO.WriteComplex("  " + p.Quality.GetColorCode() + p.ToUnpricedString() + 
					"&7 for " + p.GetPriceString());
			}

			VersatileIO.Info("{0} pricings matching filters [{1}].", res.Count, filters.ToString());
		}

		public static List<ItemPricing> GetInRange(PriceRange range, Filters filters)
		{
			return DataManager.PriceData.Prices.FindAll((p) => filters.MatchesPricing(p) && range.Contains(p.Pricing));
		}
	}
}
