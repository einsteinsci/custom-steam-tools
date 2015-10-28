using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.MarketPricesJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MarketPricesResponseJson
	{
		public int success
		{ get; set; }

		public string message
		{ get; set; }

		public ulong current_time
		{ get; set; }

		public Dictionary<string, MarketPricingJson> items
		{ get; set; }
	}
}
