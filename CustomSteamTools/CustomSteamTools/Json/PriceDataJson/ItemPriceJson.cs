using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemPriceJson
	{
		public List<long> defindex
		{ get; set; }

		// qualities = key
		public Dictionary<string, TradabilityJson> prices
		{ get; set; }

		public override string ToString()
		{
			return "ID x" + (defindex?.Count ?? 0).ToString();
		}
	}
}
