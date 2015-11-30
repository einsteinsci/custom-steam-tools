using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Backpacks;
using UltimateUtil.Fluid;
using System.ComponentModel;

namespace CustomSteamTools.Classifieds
{
	public static class DealFinder
	{
		public static double Progress
		{ get; private set; }

		/// <summary>
		/// Fired during classified scraping, progress out of total classifieds to scrape.
		/// UserState is set to a <see cref="List{ItemSale}"/> for what's current.
		/// </summary>
		public static event ProgressChangedEventHandler OnProgressChanged;

		public static FlaggedResult<List<ItemSale>, KeyValuePair<ItemSale, string>> FindDealsFlagged(
			string steamid, DealsFilters filters, bool beep = false)
		{
			if (filters == null)
			{
				VersatileIO.Warning("Deals filters object was null. Setting to default.");
				filters = new DealsFilters();
			}
			
			VersatileIO.Info("Filters: " + filters.ToString());
			
			Price? max = GetMaxPrice();
			if (max == null)
			{
				return null;
			}
			Price min = GetMinPrice(max.Value);

			VersatileIO.Info("Price range: {0} - {1}.", min.ToString(), max.ToString());
			VersatileIO.WriteLine();

			List<ItemPricing> inRange = FindPricingsInRange(new PriceRange(min, max.Value), filters);

			List<ItemSale> relevant = FindRelevantClassifeids(inRange);

			var results = PickOutDealsFlagged(relevant, filters.DealsMinProfit);

			if (beep)
			{
				Console.Beep();
			}

			return results;
		}
		public static List<ItemSale> FindDeals(string steamid, DealsFilters filters, bool beep = false)
		{
			return FindDealsFlagged(steamid, filters, beep).Result;
		}

		public static List<ItemSale> FindRelevantClassifeids(List<ItemPricing> pricings)
		{
			List<ItemSale> results = new List<ItemSale>();

			Progress = 0;
			for (int pindex = 0; pindex < pricings.Count; pindex++)
			{
				Progress = (double)pindex / pricings.Count;
				if (OnProgressChanged != null && Progress != 0.0)
				{
					int pct = Progress.ToPercentInt();
					OnProgressChanged(null, new ProgressChangedEventArgs(pct, results));
				}

				ItemPricing p = pricings[pindex];
				ItemSale set = new ItemSale(p);

				VersatileIO.Debug("Searching classifieds for {0}...", p.ToUnpricedString());
				List<ClassifiedsListing> buf = ClassifiedsScraper.GetClassifieds(
					p.Item, p.Quality, p.Craftable, p.Tradable, p.Australium);

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

		public static List<ItemPricing> FindPricingsInRange(PriceRange range, 
			List<Quality> qualitiesAllowed, List<ItemSlotPlain> slotsAllowed,
			bool? craftable = true, bool? halloween = null, bool? botkiller = null)
		{
			DealsFilters deals = new DealsFilters()
			{
				Qualities = qualitiesAllowed,
				Slots = slotsAllowed,
				Craftable = craftable,
				Halloween = halloween,
				Botkiller = botkiller,
			};

			return FindPricingsInRange(range, deals);
		}

		public static List<ItemPricing> FindPricingsInRange(PriceRange range, DealsFilters filters)
		{
			List<ItemPricing> results = new List<ItemPricing>();

			VersatileIO.Info("Finding valid items in price range...");
			foreach (ItemPricing p in DataManager.PriceData.Prices) // get ALL the datas!
			{
				if (!range.Contains(p.Pricing))
				{
					continue;
				}

				if (!filters.MatchesPricing(p))
				{
					continue;
				}

				results.Add(p);
			}

			return results;
		}

		public static Price? GetMaxPrice()
		{
			return GetMaxPrice(Settings.Instance.HomeSteamID64);
		}

		public static Price? GetMaxPrice(string id)
		{
			VersatileIO.Debug("Opening backpack of #{0}...", id);

			Backpack backpack = null;
			if (id == Settings.Instance.HomeSteamID64 ||
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
			foreach (ItemInstance i in backpack.GetAllItems())
			{
				if (i.Item.IsCurrency())
				{
					totalPure += i.Item.GetCurrencyPrice();
				}
			}
			VersatileIO.Info("Found {0} pure.", totalPure);

			return totalPure;
		}

		public static Price GetMinPrice(Price totalPure)
		{
			double _ref = totalPure.TotalRefined;
			double min = 0.15 * _ref;

			return new Price(0, min);
		}

		public static FlaggedResult<List<ItemSale>, KeyValuePair<ItemSale, string>> PickOutDealsFlagged(
			List<ItemSale> relevant, Price? minProfit)
		{
			var fres = new FlaggedResult<List<ItemSale>, KeyValuePair<ItemSale, string>>();

			List<ItemSale> results = new List<ItemSale>();

			VersatileIO.Info("Trimming deals...");
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

				Price profit = sale.Profit;

				if (profit.TotalRefined < 0.1)
				{
					VersatileIO.Warning("  No real profit seen in {0}. Excluded.", sale.Pricing.ToUnpricedString());
					fres.Add(new KeyValuePair<ItemSale, string>(sale, "NOPROFIT"));
					continue;
				}

				Price threshold = new Price(sale.Pricing.Pricing.High.TotalRefined * 
					Settings.Instance.DealsPriceDropThresholdPriceBelow);
				int totalBelow = 0;
				foreach (ClassifiedsListing listing in sale.Sellers)
				{
					if (listing.Price <= threshold)
					{
						totalBelow++;
					}
				}

				if (totalBelow >= Settings.Instance.DealsPriceDropThresholdListingCount)
				{
					VersatileIO.Warning("  The price is dropping for {0}. Excluded.", sale.Pricing.ToUnpricedString());
					fres.Add(new KeyValuePair<ItemSale, string>(sale, "PRICEDROPPING"));
					continue;
				}

				if (minProfit != null && profit < minProfit.Value)
				{
					VersatileIO.Warning("  Profit ({0}) does not meet specified threshold of {1}. Excluded.",
						profit, minProfit.Value);
					fres.Add(new KeyValuePair<ItemSale, string>(sale, "LOWPROFIT"));
					continue;
				}

				results.Add(sale);
			}

			fres.Result = results;
			return fres;
		}
		public static List<ItemSale> PickOutDeals(List<ItemSale> relevant, Price? minprofit)
		{
			return PickOutDealsFlagged(relevant, minprofit).Result;
		}
	}
}
