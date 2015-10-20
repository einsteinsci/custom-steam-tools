using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class DealFinder
	{
		public static Dictionary<ItemPricing, List<ClassifiedsListing>> FindRelevantClassifeids(List<ItemPricing> pricings)
		{
			Dictionary<ItemPricing, List<ClassifiedsListing>> results = new Dictionary<ItemPricing, List<ClassifiedsListing>>();

			foreach (ItemPricing p in pricings)
			{
				results.Add(p, new List<ClassifiedsListing>());

				Logger.Log("Finding five lowest classifieds listings for " + 
					p.ToString() + "...", ConsoleColor.DarkGray);
				List<ClassifiedsListing> buf = ClassifiedsScraper.GetClassifieds(
					p.Item, p.Quality, p.Craftable, p.Tradable);

				for (int i = 0; i < buf.Count && i < 5; i++)
				{
					results[p].Add(buf[i]);
				}
			}

			return results;
		}

		public static List<ItemPricing> FindPricingsInRange(Price low, Price high, 
			List<Quality> qualitiesAllowed, List<ItemSlotPlain> slotsAllowed,
			bool? craftable = true, bool? halloween = null)
		{
			List<ItemPricing> results = new List<ItemPricing>();

			foreach (ItemPricing p in PriceLister.PriceData.Prices) // get ALL the datas!
			{
				if (p.PriceLow < low || p.PriceHigh > high)
				{
					continue;
				}

				if (craftable != null && p.Craftable != craftable.Value)
				{
					continue;
				}

				bool isHalloween = (p.Item.HalloweenOnly || (p.Item.HasHauntedVersion ?? false));
				if (halloween != null && isHalloween != halloween.Value)
				{
					continue;
				}

				bool filterQualities = qualitiesAllowed != null || qualitiesAllowed.Count == 0;
				if (filterQualities && !qualitiesAllowed.Contains(p.Quality))
				{
					continue;
				}

				bool filterSlots = slotsAllowed != null || slotsAllowed.Count == 0;
				if (filterSlots && !slotsAllowed.Contains(p.Item.PlainSlot))
				{
					continue;
				}

				results.Add(p);
			}

			return results;
		}

		public static Price? GetMaxPrice(string id = PriceLister.SEALEDINTERFACE_STEAMID)
		{
			Logger.Log("Opening backpack of #" + id + "...", ConsoleColor.DarkGray);
			TF2BackpackData backpack = null;
			if (id == PriceLister.SEALEDINTERFACE_STEAMID ||
				id == null)
			{
				backpack = PriceLister.MyBackpackData;
			}
			else
			{
				if (!PriceLister.BackpackData.ContainsKey(id))
				{
					if (!PriceLister.LoadOtherBackpack(id))
					{
						return null;
					}
				}

				backpack = PriceLister.BackpackData[id];
			}

			Price totalPure = Price.Zero;
			foreach (ItemInstance i in backpack.Items)
			{
				if (i.Item.IsCurrency())
				{
					totalPure += i.Item.GetCurrencyPrice();
				}
			}
			Logger.Log("Found " + totalPure + " pure.");

			return totalPure;
		}

		public static Price GetMinPrice(Price totalPure)
		{
			double _ref = totalPure.TotalRefined;
			double min = 0.15 * _ref;

			return new Price(0, min);
		}

		public static List<ClassifiedsListing> PickOutDeals(Dictionary<ItemPricing, List<ClassifiedsListing>> relevant)
		{
			const double I_AM_HERE = 0.92;

			foreach (KeyValuePair<ItemPricing, List<ClassifiedsListing>> kvp in relevant)
			{
				ItemPricing p = kvp.Key;
				List<ClassifiedsListing> classifieds = kvp.Value;

				
			}

			return null;
		}
	}
}
