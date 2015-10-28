using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TypeIndexPricingJson
	{
		public string currency
		{ get; set; }

		public double value
		{ get; set; }
		
		public double value_high
		{ get; set; }

		public int last_update
		{ get; set; }

		public double difference
		{ get; set; }
	}
}
