using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Schema;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.Fluid;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Skins;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdPriceCheck : ITradeCommand
	{
		public string[] Aliases => new string[] { "pc", "pricecheck", "price" };

		public string Description => "Gets the bp.tf price of an item.";

		public string RegistryName => "pc";

		public string Syntax => "pc {defindex | itemName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.Count == 0)
			{
				VersatileIO.Error("Usage: " + Syntax);
				return;
			}

			string query = string.Join(" ", args);
			Item item = CmdInfo.SearchItem(query);
			PriceCheckResults results = GetPriceCheckResults(item);

			if (results == null)
			{
				return;
			}

			if (!results.HasResults)
			{
				VersatileIO.Error("  No price data found for item " + item.Name);
				return;
			}

			if (results.HasCrates)
			{
				VersatileIO.Info("Crates by series:");
			}
			foreach (CheckedPrice c in results.Uniques)
			{
				VersatileIO.WriteLine("  " + c.ToString(), c.Quality.GetColor());
			}

			foreach (CheckedPrice c in results.Others)
			{
				VersatileIO.WriteLine("  " + c.ToString(), c.Quality.GetColor());
			}

			if (results.HasUnusuals)
			{
				string code = VersatileIO.GetSelection("  Enter a code or continue: ", true, 
					"U", "Get unusual prices.");
				if (code != null && code.Trim().EqualsIgnoreCase("u"))
				{
					VersatileIO.Info("{0} effects priced:", results.Unusuals.Count);
					Price total = Price.Zero;
					foreach (CheckedPrice u in results.Unusuals)
					{
						VersatileIO.WriteLine("  " + u.GetUnusualEffectString(), ConsoleColor.DarkMagenta);
						total += u.Pricing.Pricing.Mid;
					}
					Price avg = total / results.Unusuals.Count;
					VersatileIO.Info("Average Unusual Price: " + avg.ToString());
				}
			}
		}

		public static PriceCheckResults GetPriceCheckResults(Item item)
		{
			if (item == null)
			{
				return null;
			}

			if (item.IsSkin())
			{
				PriceCheckResults res = new PriceCheckResults(item);
				Dictionary<SkinWear, Price> dict = CmdSkins.GetSkinPrices(res.Skin);
				dict.ForEach((w, p) => {
					res.Add(new CheckedPrice(item.GetSkin(), w, p));
				});

				return res;
			}
			else
			{
				PriceCheckResults res = new PriceCheckResults(item);
				List<ItemPricing> itemPricings = DataManager.PriceData.GetAllPriceData(item);
				foreach (ItemPricing p in itemPricings)
				{
					res.AddIfTradableNotChemSet(p);
				}

				return res;
			}
		}
	}
}
