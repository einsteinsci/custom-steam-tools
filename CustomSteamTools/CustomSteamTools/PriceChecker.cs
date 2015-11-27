using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Backpacks;
using CustomSteamTools.Commands;
using CustomSteamTools.Lookup;
using CustomSteamTools.Market;
using CustomSteamTools.Schema;
using CustomSteamTools.Skins;
using UltimateUtil;
using UltimateUtil.Fluid;

namespace CustomSteamTools
{
	public static class PriceChecker
	{
		public static PriceRange? GetNormalPrice(Item item, Quality quality, 
			bool craftable = true, bool australium = false, UnusualEffect unusual = null)
		{
			PriceCheckResults pcres = CmdPriceCheck.GetPriceCheckResults(item);

			if (pcres == null)
			{
				return null;
			}

			if (unusual == null || quality != Quality.Unusual)
			{
				CheckedPrice cp = pcres.All.FirstOrDefault((c) => c.Quality == quality && 
					c.Pricing.Craftable == craftable && c.Pricing.Australium == australium);
				return cp?.Pricing.Pricing;
			}
			else
			{
				CheckedPrice cp = pcres.All.FirstOrDefault((u) => u.Quality == quality && u.Unusual.ID == unusual.ID);
				return cp?.Pricing.Pricing;
			}
		}

		public static PriceRange? GetSkinPrice(Item item, SkinWear wear)
		{
			Skin skin = item.GetSkin();

			Price? p = CmdSkins.GetSkinPrice(skin, wear);
			if (p == null)
			{
				return null;
			}

			return new PriceRange(p.Value);
		}

		public static PriceRange? GetKillstreakPrice(Item item, Quality quality, KillstreakType tier, bool australium)
		{
			Price? res = CmdKillstreak.PriceKillstreak(item, quality, tier, australium);
			if (res == null)
			{
				return null;
			}

			return new PriceRange(res.Value);
		}

		public static PriceRange? GetMarketPriceRange(string hash)
		{
			MarketPricing p = DataManager.MarketPrices.GetPricing(hash);
			if (p == null)
			{
				return null;
			}

			return new PriceRange(p.Price);
		}

		public static FlaggedResult<PriceRange?, string> GetPriceFlagged(ItemPriceInfo item)
		{
			List<string> flags = new List<string>();

			PriceRange? res = null;
			if (item.Skin != null)
			{
				res = GetSkinPrice(item.Item, item.SkinWear.GetValueOrDefault());
				flags.AddIfMissing("market");
			}

			if (res == null && item.Killstreak != KillstreakType.None)
			{
				res = GetKillstreakPrice(item.Item, item.Quality, item.Killstreak, item.Australium);
				flags.AddIfMissing("market");
			}

			if (res == null)
			{
				res = GetNormalPrice(item.Item, item.Quality, item.Craftable, item.Australium, item.Unusual);
			}

			if (res == null) // still
			{
				string hash = MarketPricing.GetMarketHash(item.Item, item.Killstreak, item.Quality);
				res = GetMarketPriceRange(hash);
				flags.AddIfMissing("market");
			}

			return new FlaggedResult<PriceRange?, string>(res, flags);
		}
		public static FlaggedResult<PriceRange?, string> GetPriceFlagged(ItemInstance inst)
		{
			return GetPriceFlagged(new ItemPriceInfo(inst));
		}
		public static PriceRange? GetPrice(ItemPriceInfo item)
		{
			return GetPriceFlagged(item).Result;
		}
		public static PriceRange? GetPrice(ItemInstance inst)
		{
			return GetPrice(new ItemPriceInfo(inst));
		}
	}
}
