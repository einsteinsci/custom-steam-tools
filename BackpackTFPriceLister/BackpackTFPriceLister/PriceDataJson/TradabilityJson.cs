using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TradabilityJson
	{
		public CraftibilityJson Tradable
		{ get; set; }

		[JsonProperty("Non-Tradable")]
		public CraftibilityJson NonTradable
		{ get; set; }
	}
}
