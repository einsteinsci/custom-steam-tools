using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdDeals : ITradeCommand
	{
		public string[] Aliases => new string[] { "deals", "getdeals", "profitable" };

		public string Description => "Lists the best 'deals' currently available to a user on bp.tf. Can take a while.";

		public string RegistryName => "deals";

		public string Syntax => "deals [steamID64] " + Filters.GetSyntax(true);

		public static bool DoBeepOnFinished
		{ get; set; }

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			Filters filters = new Filters();

			string steamid = Settings.Instance.HomeSteamID64;
			
			foreach (string s in args)
			{
				if (!s.StartsWith("/"))
				{
					steamid = s;
					continue;
				}

				filters.HandleArg(s);
			}

			List<ItemSale> sales = DealFinder.FindDeals(steamid, filters);
			if (sales == null)
			{
				// already logged
				return;
			}

			sales.Sort((a, b) => a.Profit.TotalRefined.CompareTo(b.Profit.TotalRefined));

			VersatileIO.WriteLine();
			VersatileIO.Info("{0} deals found:", sales.Count);

			foreach (ItemSale s in sales)
			{
				VersatileIO.WriteComplex("  " + s.ToComplexString());
			}
		}
	}
}
