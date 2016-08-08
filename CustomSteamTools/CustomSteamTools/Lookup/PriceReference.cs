using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Utils;
using CustomSteamTools.Schema;
using CustomSteamTools.Json.PriceDataJson;
using CustomSteamTools.Classifieds;

using Newtonsoft.Json.Linq;

using UltimateUtil;
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
				if (p.Items.Exists((i) => i.Defindex == item.Defindex))
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

				if (items.Count == 0 && ipj.defindex.HasItems())
				{
					string s_ids = "[ ";
					foreach (long l in ipj.defindex)
					{
						s_ids += l + " ";
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
							AddPricings(MakePricings(cfj.Craftable), items, quality, true, true, australium);
						}
						if (cfj.NonCraftable != null)
						{
							AddPricings(MakePricings(cfj.NonCraftable), items, quality, true, false, australium);
						}
					}

					if (kvp1.Value.NonTradable != null)
					{
						CraftibilityJson cfj = kvp1.Value.NonTradable;

						if (cfj.Craftable != null)
						{
							AddPricings(MakePricings(cfj.Craftable), items, quality, false, true, australium);
						}
						if (cfj.NonCraftable != null)
						{
							AddPricings(MakePricings(cfj.NonCraftable), items, quality, false, false, australium);
						}
					}
				}
			}
		} // Sheesh...Thanks a ton backpack.tf...

		public static Dictionary<string, TypeIndexPricingJson> MakePricings(JToken json)
		{
			Dictionary<string, TypeIndexPricingJson> res = new Dictionary<string, TypeIndexPricingJson>();

			JArray arr = json as JArray;
			if (arr != null)
			{
				dynamic dyn = json;

				for (int i = 0; i < arr.Count; i++)
				{
					if (dyn[i].currency == null)
					{
						continue;
					}

					TypeIndexPricingJson tipj = new TypeIndexPricingJson
					{
						currency = dyn[i].currency,
						last_update = (long)(dyn[i].last_update ?? 0),
						difference = dyn[i].difference ?? 0,
						value = dyn[i].value ?? 0,
						value_high = dyn[i].value_high ?? (dyn[i].value ?? 0)
					};

					res.Add(i.ToString(), tipj);
				}

				return res;
			}

			JObject obj = json as JObject;
			if (obj != null)
			{
				foreach (var kvp in obj)
				{
					string idx = kvp.Key;

					dynamic dyn = kvp.Value;

					TypeIndexPricingJson tipj = new TypeIndexPricingJson
					{
						currency = dyn.currency,
						last_update = dyn.last_update,
						difference = dyn.difference,
						value = dyn.value,
						value_high = dyn.value_high ?? dyn.value
					};

					res.Add(idx, tipj);
				}

				return res;
			}

			return res;
		}

		public void AddPricings(Dictionary<string, TypeIndexPricingJson> dict, List<Item> items, Quality quality,
			bool tradable, bool craftable, bool australium)
		{
			foreach (KeyValuePair<string, TypeIndexPricingJson> kvp in dict)
			{
				TypeIndexPricingJson p = kvp.Value;
				string idx = kvp.Key;

				Prices.Add(new ItemPricing(items, quality, p.currency, p.value, p.value_high, idx, craftable, tradable, australium));
			}
		}
	}
}
