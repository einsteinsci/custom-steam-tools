using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Commands;
using CustomSteamTools.Items;
using CustomSteamTools.Lookup;
using CustomSteamTools.Market;
using UltimateUtil;
using UltimateUtil.Fluid;

namespace CustomSteamTools
{
	public static class PriceChecker
	{
		public static PriceRange? GetNormalPrice(Item item, Quality quality, 
			bool craftable = true, UnusualEffect unusual = null)
		{
			PriceCheckResults pcres = CmdPriceCheck.GetPriceInfo(item);

			if (pcres == null)
			{
				return null;
			}

			if (unusual == null)
			{
				CheckedPrice cp = pcres.All.FirstOrDefault((c) => c.Quality == quality && c.Pricing.Craftable == craftable);
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

		public static PriceRange? GetKillstreakPrice(Item item, Quality quality, KillstreakType tier)
		{
			Price? res = CmdKillstreak.PriceKillstreak(item, quality, tier);
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

		public static FlaggedResult<PriceRange?, string> GetPriceFlagged(ItemInstance item)
		{
			List<string> flags = new List<string>();

			PriceRange? res = null;
			if (item.Item.IsSkin())
			{
				res = GetSkinPrice(item.Item, item.GetSkinWear().GetValueOrDefault());
				flags.AddIfMissing("market");
			}

			if (res == null && item.GetKillstreak() != KillstreakType.None)
			{
				res = GetKillstreakPrice(item.Item, item.Quality, item.GetKillstreak());
				flags.AddIfMissing("market");
			}

			if (res == null)
			{
				res = GetNormalPrice(item.Item, item.Quality, item.Craftable, item.GetUnusual());
			}

			if (res == null) // still
			{
				string hash = MarketPricing.GetMarketHash(item.Item, item.GetKillstreak(), item.Quality);
				res = GetMarketPriceRange(hash);
				flags.AddIfMissing("market");
			}

			return new FlaggedResult<PriceRange?, string>(res, flags);
		}
		public static PriceRange? GetPrice(ItemInstance item)
		{
			return GetPriceFlagged(item).Result;
		}
	}
}
