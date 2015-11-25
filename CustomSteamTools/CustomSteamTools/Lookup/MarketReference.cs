using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Utils;
using CustomSteamTools.Json.MarketPricesJson;
using CustomSteamTools.Market;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Schema;

namespace CustomSteamTools.Lookup
{
	public class MarketReference
	{
		public List<MarketPricing> Pricings
		{ get; private set; }

		public MarketReference(MarketPriceDataJson json, GameSchema schema)
		{
			Pricings = new List<MarketPricing>();

			if (json.response.success == 0)
			{
				VersatileIO.Error("Market price data failed: " + json.response.message);
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
