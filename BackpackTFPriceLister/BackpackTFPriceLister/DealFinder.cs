using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class DealFinder
	{
		public static List<ItemSale> FindDeals(string steamid, params object[] filters)
		{
			List<Quality> allowedQualities = new List<Quality>();
			List<ItemSlotPlain> allowedSlots = new List<ItemSlotPlain>();
			bool? craftable = true;
			bool? halloween = null;
			bool? botkiller = null;
			bool verify = false;
			foreach (object obj in filters)
			{
				if (obj is Quality)
				{
					allowedQualities.Add((Quality)obj);
				}

				if (obj is ItemSlotPlain)
				{
					allowedSlots.Add((ItemSlotPlain)obj);
				}

				if (obj is string)
				{
					string s = obj as string;

					if (s.ToLower() == "verify")
					{
						verify = true;
					}
					else if (s.ToLower() == "-halloween")
					{
						halloween = false;
					}
					else if (s.ToLower() == "halloween")
					{
						halloween = true;
					}
					else if (s.ToLower() == "+uncraftable")
					{
						craftable = null;
					}
					else if (s.ToLower() == "-craftable" || s.ToLower() == "uncraftable")
					{
						craftable = false;
					}
					else if (s.ToLower() == "-botkiller")
					{
						botkiller = false;
					}
					else if (s.ToLower() == "botkiller")
					{
						botkiller = true;
					}
				}
			}

			string filtersListString = "";
			foreach (Quality q in allowedQualities)
			{
				filtersListString += q.ToString() + " ";
			}
			foreach (ItemSlotPlain s in allowedSlots)
			{
				filtersListString += s.ToString() + " ";
			}
			foreach (object obj in filters)
			{
				string f = obj as string;
				if (f != null)
				{
					filtersListString += f + " ";
				}
			}
			Logger.Log("Filters: " + filtersListString, ConsoleColor.White);

			if (allowedSlots.Count == 0)
			{
				allowedSlots = null;
			}
			if (allowedQualities.Count == 0)
			{
				allowedQualities = null;
			}
			//if (allowedClasses.Count == 0)
			//{
			//	allowedClasses = null;
			//}

			Price? max = GetMaxPrice();
			if (max == null)
			{
				return null;
			}
			Price min = GetMinPrice(max.Value);

			Logger.Log("Price range: " + min.ToString() + " - " + max.ToString(), ConsoleColor.White);
			Logger.AddLine();

			List<ItemPricing> inRange = FindPricingsInRange(min, max.Value, 
				allowedQualities, allowedSlots, craftable, halloween, botkiller);

			List<ItemSale> relevant = FindRelevantClassifeids(inRange, verify);

			return PickOutDeals(relevant);
		}

		public static List<ItemSale> FindRelevantClassifeids(List<ItemPricing> pricings, bool verify)
		{
			List<ItemSale> results = new List<ItemSale>();

			foreach (ItemPricing p in pricings)
			{
				ItemSale set = new ItemSale(p);

				Logger.Log("Searching classifieds for " + 
					p.ToUnpricedString() + "...", ConsoleColor.DarkGray);
				List<ClassifiedsListing> buf = ClassifiedsScraper.GetClassifieds(
					p.Item, p.Quality, verify, p.Craftable, p.Tradable, p.Australium);

				// ignore items with no sellers
				if (!buf.Exists((c) => c.OrderType == OrderType.Sell))
				{
					continue;
				}

				int i = 0;
				while (i < buf.Count && set.Sellers.Count < 5)
				{
					if (buf[i].OrderType == OrderType.Buy)
					{
						set.Buyers.Add(buf[i]);
					}
					else
					{
						set.Sellers.Add(buf[i]);
					}
					i++;
				}

				results.Add(set);
			}

			return results;
		}

		public static List<ItemPricing> FindPricingsInRange(Price low, Price high, 
			List<Quality> qualitiesAllowed, List<ItemSlotPlain> slotsAllowed,
			bool? craftable = true, bool? halloween = null, bool? botkiller = null)
		{
			List<ItemPricing> results = new List<ItemPricing>();

			Logger.Log("Finding valid items in price range...", ConsoleColor.DarkGray);
			foreach (ItemPricing p in DataManager.PriceData.Prices) // get ALL the datas!
			{
				if (p.PriceLow < low || p.PriceHigh > high)
				{
					continue;
				}

				if (craftable != null && p.Craftable != craftable.Value)
				{
					continue;
				}

				if (botkiller != null && p.Item.IsBotkiller() != botkiller.Value)
				{
					continue;
				}

				bool isHalloween = (p.Item.HalloweenOnly || (p.Item.HasHauntedVersion ?? false));
				if (halloween != null && isHalloween != halloween.Value)
				{
					continue;
				}

				bool filterQualities = qualitiesAllowed != null && qualitiesAllowed.Count != 0;
				if (filterQualities && !qualitiesAllowed.Contains(p.Quality))
				{
					continue;
				}

				bool filterSlots = slotsAllowed != null && slotsAllowed.Count != 0;
				if (filterSlots && !slotsAllowed.Contains(p.Item.PlainSlot))
				{
					continue;
				}

				results.Add(p);
			}

			return results;
		}

		public static Price? GetMaxPrice(string id = DataManager.SEALEDINTERFACE_STEAMID)
		{
			Logger.Log("Opening backpack of #" + id + "...", ConsoleColor.DarkGray);
			TF2BackpackData backpack = null;
			if (id == DataManager.SEALEDINTERFACE_STEAMID ||
				id == null)
			{
				backpack = DataManager.MyBackpackData;
			}
			else
			{
				if (!DataManager.BackpackData.ContainsKey(id))
				{
					if (!DataManager.LoadOtherBackpack(id))
					{
						return null;
					}
				}

				backpack = DataManager.BackpackData[id];
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

		public static List<ItemSale> PickOutDeals(List<ItemSale> relevant)
		{
            const double PRICE_DROPPING_THRESHOLD = 0.96;
			const int PRICE_DROPPING_COUNT = 3;

			List<ItemSale> results = new List<ItemSale>();

			Logger.Log("Trimming deals...");
			foreach (ItemSale sale in relevant)
			{
				ClassifiedsListing highestBuyer = sale.HighestBuyer;
				if (highestBuyer != null)
				{
					if (sale.CheapestSeller.Price < highestBuyer.Price)
					{
						sale.HasQuickDeal = true;
						results.Add(sale);
						continue;
					}
				}

				if (sale.Profit.TotalRefined < 0.1)
				{
					Logger.Log("  No real profit seen in " + sale.Pricing.ToUnpricedString() +
						". Excluded", ConsoleColor.Yellow);
					continue;
				}

				Price threshold = new Price(sale.Pricing.PriceHigh.TotalRefined * PRICE_DROPPING_THRESHOLD);
				int totalBelow = 0;
				foreach (ClassifiedsListing listing in sale.Sellers)
				{
					if (listing.Price <= threshold)
					{
						totalBelow++;
					}
				}

				if (totalBelow >= PRICE_DROPPING_COUNT)
				{
					Logger.Log("  The price is dropping for " + sale.Pricing.ToUnpricedString() + 
						". Excluded.", ConsoleColor.Yellow);
					continue;
				}

				results.Add(sale);
			}

			return results;
		}
	}
}
