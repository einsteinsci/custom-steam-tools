using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackpackTFPriceLister.Utils;
using BackpackTFPriceLister.Json.MarketPricesJson;
using BackpackTFPriceLister.Market;

namespace BackpackTFPriceLister.Lookup
{
	public class MarketPriceData
	{
		public List<MarketPricing> Pricings
		{ get; private set; }

		public MarketPriceData(MarketPriceDataJson json, TF2Data schema)
		{
			Pricings = new List<MarketPricing>();

			if (json.response.success == 0)
			{
				Logger.Log("Market price data failed: " + json.response.message, ConsoleColor.Red);
				return;
			}

			foreach (KeyValuePair<string, MarketPricingJson> kvp in json.response.items)
			{
				MarketPricing p = new MarketPricing(kvp.Key, kvp.Value, schema);

				if (!p.Failed)
				{
					Pricings.Add(p);
				}
			}
		}

		public MarketPricing GetPricing(string hash)
		{
			foreach (MarketPricing p in Pricings)
			{
				if (p.MarketHash.ToLower() == hash.ToLower())
				{
					return p;
				}
			}

			return null;
		}
	}
}
