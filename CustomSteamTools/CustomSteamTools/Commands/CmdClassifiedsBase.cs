using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Items;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	public abstract class CmdClassifiedsBase : ITradeCommand
	{
		public abstract string[] Aliases
		{ get; }

		public abstract string RegistryName // "buyers" or "sellers"
		{ get; }

		public virtual string Syntax => RegistryName + " [/q=Quality] [/c] [/uc | /nc] [/t] [/nt]" +
			" [/aus] [/noaus] {defindex | itemName | searchQuery}";

		public virtual string Description => "Gets a list of backpack.tf " + RegistryName + " for an item";

		public abstract void RunCommand(CommandHandler handler, List<string> args);

		public void RunCommand(CommandHandler handler, List<string> args, OrderType orderType)
		{
			#region args
			bool? preCraftable = null, preTradable = null, preAus = null;
			Quality? preQuality = null;
			while (args.Exists((s) => s.StartsWith("/")))
			{
				string a = args[0].TrimStart('/').ToLower();
				args.RemoveAt(0);

				if (a.StartsWith("q="))
				{
					string qStr = a.Substring("q=".Length);
					preQuality = ItemQualities.ParseNullable(qStr);
				}
				else if (a == "aus")
				{
					preAus = true;
				}
				else if (a == "noaus")
				{
					preAus = false;
				}
				else if (a == "c")
				{
					preCraftable = true;
				}
				else if (a == "nc" || a == "uc")
				{
					preCraftable = false;
				}
				else if (a == "t")
				{
					preTradable = true;
				}
				else if (a == "nt")
				{
					preTradable = false;
				}
			}
			#endregion

			if (args.Count == 0)
			{
				VersatileIO.Error("Usage: " + Syntax);
				return;
			}

			string query = string.Join(" ", args);
			Item item = CmdInfo.SearchItem(query);
			if (item == null)
			{
				return;
			}

			VersatileIO.Info("Item: " + item.Name);
			ListingProperties flags = GetValidProperties(item);

			#region props
			Quality quality = Quality.Unique;
			if (flags.Quality != Quality.Unique && preQuality == null)
			{
				bool found = false;
				while (!found)
				{
					string qStr = VersatileIO.GetString("Quality? ");
					found = ItemQualities.TryParse(qStr, out quality);
				}
			}
			else if (preQuality != null)
			{
				quality = preQuality.Value;
			}

			bool australium = false;
			if (flags.Australium && preAus == null)
			{
				bool found = false;
				while (!found)
				{
					string bStr = VersatileIO.GetString("Australium? ");
					found = BooleanUtil.TryParseLoose(bStr, out australium);
				}
			}
			else if (preAus != null)
			{
				australium = preAus.Value;
			}

			bool tradable = true;
			if (flags.Tradable && preTradable == null)
			{
				bool found = false;
				while (!found)
				{
					string tStr = VersatileIO.GetString("Tradable? ");
					found = BooleanUtil.TryParseLoose(tStr, out tradable);
				}
			}
			else if (preTradable != null)
			{
				tradable = preTradable.Value;
			}

			bool craftable = true;
			if (flags.Craftable && preCraftable == null)
			{
				bool found = false;
				while (!found)
				{
					string cStr = VersatileIO.GetString("Craftable? ");
					found = BooleanUtil.TryParseLoose(cStr, out craftable);
				}
			}
			else if (preCraftable != null)
			{
				craftable = preCraftable.Value;
			}
			#endregion props

			ListingProperties props = new ListingProperties();
			props.Quality = quality;
			props.Australium = australium;
			props.Craftable = craftable;
			props.Tradable = tradable;

			List<ClassifiedsListing> res = GetListings(item, props, orderType);

			if (res.IsEmpty())
			{
				return;
			}

			Price? best = null;
			string seller = "{UNKNOWN}";
			foreach (ClassifiedsListing listing in res)
			{
				VersatileIO.WriteComplex(listing.ToString(true, 100, '\u00A7'), '\u00A7');

				if (best == null)
				{
					best = listing.Price;
					continue;
				}

				if ((orderType == OrderType.Buy && listing.Price > best.Value) ||
					(orderType == OrderType.Sell && listing.Price < best.Value))
				{
					best = listing.Price;
					seller = listing.ListerNickname ?? listing.ListerSteamID64;
				}
			}
			VersatileIO.Info("Best deal: {0} from {1}.", best.ToString(), seller);
		}

		public static List<ClassifiedsListing> GetListings(Item item, ListingProperties props, 
			OrderType orderType)
		{
			VersatileIO.Info("Searching bp.tf classifieds for {0}...", props.ToString(item));
			List<ClassifiedsListing> all = ClassifiedsScraper.GetClassifieds(item, props);

			List<ClassifiedsListing> res = new List<ClassifiedsListing>();
			foreach (ClassifiedsListing c in all)
			{
				if (c.OrderType == orderType)
				{
					res.Add(c);
				}
			}

			if (res.IsNullOrEmpty())
			{
				VersatileIO.Warning("No classifieds found for {0}.", props.ToString(item));
				return res.EmptyIfNull().ToList();
			}

			VersatileIO.Info("{0} listings found.", res.Count);
			return res;
		}

		public static ListingProperties GetValidProperties(Item item)
		{
			ListingProperties res = new ListingProperties();

			List<ItemPricing> pricings = DataManager.PriceData.GetAllPriceData(item);

			if (pricings.HasItems())
			{
				Quality testQ = pricings.First().Quality;
				if (!pricings.TrueForAll((p) => p.Quality == testQ))
				{
					res.Quality = Quality.Strange;
				}
			}

			if (pricings.Exists((p) => p.Australium))
			{
				res.Australium = true;
			}

			if (pricings.Exists((p) => !p.Craftable))
			{
				res.Craftable = true;
			}

			if (pricings.Exists((p) => !p.Tradable))
			{
				res.Tradable = true;
			}

			return res;
		}
	}
}
