using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Classifieds
{
	public static class DealFinder
	{
		public static List<ItemSale> FindDeals(string steamid, Filters filters, bool beep = false)
		{
			if (filters == null)
			{
				VersatileIO.Warning("Deals filters object was null. Setting to default.");
				filters = new Filters();
			}

			#region oldFilters
			/*
			List<Quality> allowedQualities = new List<Quality>();
			List<ItemSlotPlain> allowedSlots = new List<ItemSlotPlain>();
			bool? craftable = true;
			bool? halloween = null;
			bool? botkiller = null;
			Price? minProfit = null;

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
					
					if (s.EqualsIgnoreCase("/nohalloween"))
					{
						halloween = false;
					}
					else if (s.EqualsIgnoreCase("/halloween"))
					{
						halloween = true;
					}
					else if (s.EqualsIgnoreCase("/anycraftable"))
					{
						craftable = null;
					}
					else if (s.EqualsIgnoreCase("/uncraftable"))
					{
						craftable = false;
					}
					else if (s.EqualsIgnoreCase("/nobotkiller"))
					{
						botkiller = false;
					}
					else if (s.EqualsIgnoreCase("/botkiller"))
					{
						botkiller = true;
					}
					else if (s.ToLower().StartsWith("/minprofit="))
					{
						string smin = s.ToLower().Substring("/minprofit=".Length);
						Price buf;
						if (!Price.TryParse(smin, out buf))
						{
							VersatileIO.Error("Invalid minProfit: " + smin);
							return null;
						}
						minProfit = buf;
					}
				}
			}
			*/

			/*
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
			*/
			#endregion oldFilters

			VersatileIO.Info("Filters: " + filters.ToString());

			//if (allowedSlots.Count == 0)
			//{
			//	allowedSlots = null;
			//}
			//if (allowedQualities.Count == 0)
			//{
			//	allowedQualities = null;
			//}
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

			VersatileIO.Info("Price range: {0} - {1}.", min.ToString(), max.ToString());
			VersatileIO.WriteLine();

			List<ItemPricing> inRange = FindPricingsInRange(new PriceRange(min, max.Value), filters);

			List<ItemSale> relevant = FindRelevantClassifeids(inRange);

			List<ItemSale> results = PickOutDeals(relevant, filters.DealsMinProfit);

			if (beep)
			{
				Console.Beep();
			}

			return results;
		}

		public static List<ItemSale> FindRelevantClassifeids(List<ItemPricing> pricings)
		{
			List<ItemSale> results = new List<ItemSale>();

			foreach (ItemPricing p in pricings)
			{
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
			Filters deals = new Filters()
			{
				Qualities = qualitiesAllowed,
				Slots = slotsAllowed,
				Craftable = craftable,
				Halloween = halloween,
				Botkiller = botkiller,
			};

			return FindPricingsInRange(range, deals);
		}

		public static List<ItemPricing> FindPricingsInRange(PriceRange range, Filters filters)
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
			foreach (ItemInstance i in backpack.Items)
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

		public static List<ItemSale> PickOutDeals(List<ItemSale> relevant, Price? minProfit)
		{
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
					continue;
				}

				if (minProfit != null && profit < minProfit.Value)
				{
					VersatileIO.Warning("  Profit ({0}) does not meet specified threshold of {1}. Excluded.",
						profit, minProfit.Value);
					continue;
				}

				results.Add(sale);
			}

			return results;
		}
	}
}
