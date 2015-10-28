using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.MarketPricesJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MarketPricingJson
	{
		public ulong last_updated
		{ get; set; }

		public int quantity
		{ get; set; }

		public int value // in cents
		{ get; set; }
	}
}
