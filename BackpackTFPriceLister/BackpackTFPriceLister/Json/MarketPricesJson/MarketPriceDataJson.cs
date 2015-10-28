using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BackpackTFPriceLister.Json.MarketPricesJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class MarketPriceDataJson
	{
		[JsonProperty]
		public MarketPricesResponseJson response
		{ get; set; }
	}
}
