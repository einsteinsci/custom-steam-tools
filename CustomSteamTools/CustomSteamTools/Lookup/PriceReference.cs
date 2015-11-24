using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Utils;
using CustomSteamTools.Items;
using CustomSteamTools.Json.PriceDataJson;
using CustomSteamTools.Classifieds;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Lookup
{
	public class PriceReference
	{
		public List<ItemPricing> Prices
		{ get; private set; }

		public ItemPricing GetPriceData(Item item, Quality quality = Quality.Unique, int? priceIndex = null, 
			bool? craftable = null, bool? tradable = true, bool? australium = false)
		{
			List<ItemPricing> pricings = GetAllPriceData(item);

			foreach (ItemPricing p in GetAllPriceData(item))
			{
				if (quality != p.Quality)
				{
					continue;
				}

				if (priceIndex != null && p.PriceIndex != priceIndex)
				{
					continue;
				}

				if (craftable != null && p.Craftable != craftable)
				{
					continue;
				}

				if (tradable != null && p.Tradable != tradable)
				{
					continue;
				}

				if (australium != null && p.Australium != australium)
				{
					continue;
				}

				return p;
			}

			return null;
		}

		public List<ItemPricing> GetAllPriceData(Item item)
		{
			List<ItemPricing> res = new List<ItemPricing>();
			foreach (ItemPricing p in Prices)
			{
				if (p.Items.Exists((i) => i.ID == item.ID))
				{
					res.Add(p);
				}
			}

			return res;
		}

		public PriceReference(BpTfPriceDataJson json, GameSchema db)
		{
			Prices = new List<ItemPricing>();

			foreach (KeyValuePair<string, ItemPriceJson> kvp0 in json.response.items)
			{
				ItemPriceJson ipj = kvp0.Value;

				// Skip "Random Craft Hat". It's not a real item.
				if (ipj.defindex.FirstOrDefault() == -2)
				{
					continue;
				}

				List<Item> items = new List<Item>();
				foreach (long l in ipj.defindex)
				{
					Item i = db.GetItem(l);
					if (i != null)
					{
						items.Add(i);
					}
				}

				if (items.Count == 0)
				{
					string s_ids = "[ ";
					foreach (long l in ipj.defindex)
					{
						s_ids += l.ToString() + " ";
					}
					s_ids += "]";
					VersatileIO.Error("Could not find item with any ID among " + s_ids);
				}

				bool australium = kvp0.Key.StartsWith("Australium ");

				foreach (KeyValuePair<string, TradabilityJson> kvp1 in ipj.prices)
				{
					int qid = int.Parse(kvp1.Key);
					Quality quality = (Quality)qid;

					if (kvp1.Value.Tradable != null)
					{
						CraftibilityJson cfj = kvp1.Value.Tradable;

						if (cfj.Craftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.Craftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(items, quality, pricing.currency, pricing.value, 
									pricing.value_high, priceIndex, true, true, australium));
							}
						}
						if (cfj.NonCraftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.NonCraftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(items, quality, pricing.currency, pricing.value, 
									pricing.value_high, priceIndex, false, true, australium));
							}
						}
					}

					if (kvp1.Value.NonTradable != null)
					{
						CraftibilityJson cfj = kvp1.Value.NonTradable;

						if (cfj.Craftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.Craftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(items, quality, pricing.currency, pricing.value, 
									pricing.value_high, priceIndex, true, false, australium));
							}
						}
						if (cfj.NonCraftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.NonCraftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(items, quality, pricing.currency, pricing.value, 
									pricing.value_high, priceIndex, false, false, australium));
							}
						}
					}
				}
			}
		} // Sheesh...Thanks a ton backpack.tf...
	}
}
