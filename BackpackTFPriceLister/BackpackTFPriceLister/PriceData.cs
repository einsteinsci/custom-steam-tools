using BackpackTFPriceLister.PriceDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class BpTfPriceData
	{
		public List<ItemPricing> Prices
		{ get; private set; }

		public ItemPricing GetPriceData(Item item, Quality quality = Quality.Unique, int? priceIndex = null, 
			bool? craftable = null, bool? tradable = true)
		{
			// Don't ask...
			foreach (ItemPricing p in Prices)
			{
				if (p.Item == item && p.Quality == quality)
				{
					if (priceIndex == null || priceIndex.Value == p.PriceIndex)
					{
						if (craftable == null || craftable.Value == p.Craftable)
						{
							if (tradable == null || tradable.Value == p.Tradable)
							{
								return p;
							}
						}
					}
				}
			}

			return null;
		}

		public BpTfPriceData(BpTfPriceDataJson json, TF2Data db)
		{
			Prices = new List<ItemPricing>();

			foreach (ItemPriceJson ipj in json.response.items.Values)
			{
				Item item = db.GetItem(ipj.defindex.FirstOrDefault());
				if (item == null)
				{
					Logger.Log("Could not find item with ID " + ipj.defindex.FirstOrDefault().ToString(), true, false, this);
				}

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

								Prices.Add(new ItemPricing(item, quality, pricing.currency, pricing.value, pricing.value_high, priceIndex, true, true));
							}
						}
						if (cfj.NonCraftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.NonCraftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(item, quality, pricing.currency, pricing.value, pricing.value_high, priceIndex, false, true));
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

								Prices.Add(new ItemPricing(item, quality, pricing.currency, pricing.value, pricing.value_high, priceIndex, true, false));
							}
						}
						if (cfj.NonCraftable != null)
						{
							foreach (KeyValuePair<string, TypeIndexPricingJson> kvp2 in cfj.NonCraftable)
							{
								TypeIndexPricingJson pricing = kvp2.Value;
								string priceIndex = kvp2.Key;

								Prices.Add(new ItemPricing(item, quality, pricing.currency, pricing.value, pricing.value_high, priceIndex, false, false));
							}
						}
					}
				}
			}
		} // Sheesh...Thanks a ton backpack.tf...
	}
}
