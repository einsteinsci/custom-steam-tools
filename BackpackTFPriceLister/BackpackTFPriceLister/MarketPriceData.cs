using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class MarketPriceData
	{
		public List<MarketPricing> Pricings
		{ get; private set; }

		public MarketPriceData(MarketPricesJson.MarketPriceDataJson json, TF2Data schema)
		{
			Pricings = new List<MarketPricing>();

			foreach (KeyValuePair<string, MarketPricesJson.MarketPricingJson> kvp in json.response.items)
			{
				MarketPricing p = new MarketPricing(kvp.Key, kvp.Value, schema);

				if (!p.Failed)
				{
					Pricings.Add(p);
				}
			}
		}
	}
}
