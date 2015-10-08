using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class BPTFPriceDataResponseJson
	{
		// almost always 1
		public int success
		{ get; set; }

		public int current_time
		{ get; set; }

		public double raw_usd_value
		{ get; set; }

		public string usd_currency
		{ get; set; }

		public int usd_currency_index
		{ get; set; }

		public Dictionary<string, ItemPriceJson> items
		{ get; set; }
	}
}
