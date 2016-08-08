using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Market;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdKillstreak : ITradeCommand
	{
		public string[] Aliases => new string[] { "ks", "killstreak", "pricekillstreak" };

		public string Description => "Prices a killstreak item.";

		public string RegistryName => "ks";

		public string Syntax => "ks [/tier=TIER] [/quality=QUALITY] [/aus] {defindex | itemName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			Quality? quality = null;
			KillstreakType? killstreak = null;
			bool? aus = null;

			string query = "";
			foreach (string s in args)
			{
				if (!s.StartsWith("/"))
				{
					query += s + " ";
					continue;
				}

				if (s.StartsWithIgnoreCase("/tier="))
				{
					string tStr = s.Substring("/tier=".Length);
					killstreak = KillstreakTypes.ParseNullable(tStr);
				}

				if (s.StartsWithIgnoreCase("/quality="))
				{
					string qStr = s.Substring("/quality=".Length);
					quality = ItemQualities.ParseNullable(qStr);
				}
				
				if (s.EqualsIgnoreCase("/aus"))
				{
					aus = true;
				}
			}
			query = query.TrimEnd(' ');

			Item item = CmdInfo.SearchItem(query);

			if (item == null)
			{
				// already logged
				return;
			}

			if (item.PlainSlot != ItemSlotPlain.Weapon)
			{
				VersatileIO.Error("Killstreaks cannot be applied to a {0} slot item.", item.PlainSlot);
				return;
			}

			while (quality == null)
			{
				string qStr = VersatileIO.GetString("Quality? ");
				if (qStr.EqualsIgnoreCase("esc"))
				{
					VersatileIO.Warning("Cancelled.");
					return;
				}

				quality = ItemQualities.ParseNullable(qStr);

				if (quality == null)
				{
					VersatileIO.Error("Invalid quality. Try again.");
				}
			}

			while (killstreak == null)
			{
				string ksStr = VersatileIO.GetString("Killstreak Tier? ");
				if (ksStr.EqualsIgnoreCase("esc"))
				{
					VersatileIO.Warning("Cancelled.");
					return;
				}

				killstreak = KillstreakTypes.ParseNullable(ksStr);

				if (killstreak == null)
				{
					VersatileIO.Error("Invalid killstreak. Try again.");
				}
			}

			if (!item.CanBeAustralium())
			{
				aus = false;
			}
			while (aus == null)
			{
				string ausStr = VersatileIO.GetString("Australium? ");
				if (ausStr.EqualsIgnoreCase("esc"))
				{
					VersatileIO.Warning("Cancelled.");
					return;
				}

				aus = BooleanUtil.ParseLoose(ausStr);
				if (aus == null)
				{
					VersatileIO.Error("Invalid boolean. Try again.");
				}
			}

			Price? price = PriceKillstreak(item, quality.Value, killstreak.Value, aus ?? false);

			if (price == null)
			{
				// already logged
				return;
			}
			
			VersatileIO.WriteComplex("{0}  Price for {1} " + item.ToString(quality.Value, aus ?? false, killstreak.Value) + 
				"{2}: " + price.Value, 
				ConsoleColor.White, quality.Value.GetColor(), ConsoleColor.White);
		}

		public static Price? PriceKillstreak(Item item, Quality quality, KillstreakType killstreak, bool australium)
		{
			if (item.PlainSlot != ItemSlotPlain.Weapon)
			{
				VersatileIO.Error("Item must be a weapon.");
				return null;
			}

			if (killstreak == KillstreakType.None)
			{
				VersatileIO.Warning("Killstreak type is None. Pricings from market data will not be" +
					" as reliable as backpack.tf prices.");
			}

			List<MarketPricing> viable = DataManager.MarketPrices.Pricings.FindAll((p) => p.Item == item);

			List<MarketPricing> pricings = new List<MarketPricing>();
			foreach (MarketPricing p in viable)
			{
				if (p.Quality != quality)
				{
					continue;
				}

				if (p.Killstreak != killstreak)
				{
					continue;
				}

				pricings.Add(p);
				break;
			}

			if (pricings.Count == 0)
			{
				VersatileIO.Error("No killstreak prices found on community market for " +
					item.ToString(quality, australium, killstreak));
				return null;
			}
			else if (pricings.Count == 1)
			{
				VersatileIO.Success("Pricing successful!");
				return pricings[0].Price;
			}
			else
			{
				VersatileIO.Warning("Multiple market pricings found for {0}. Returning the average price.",
					item.ToString(quality, australium, killstreak));
				Price sum = Price.Zero;
				foreach (MarketPricing p in pricings)
				{
					sum += p.Price;
				}

				Price avg = sum / pricings.Count;
				VersatileIO.Success("Pricing successful!");
				return avg;
			}
		}
	}
}
