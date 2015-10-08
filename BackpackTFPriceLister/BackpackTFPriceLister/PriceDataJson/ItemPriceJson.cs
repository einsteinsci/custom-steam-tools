using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemPriceJson
	{
		public List<int> defindex
		{ get; private set; }

		public Dictionary<string, TradabilityJson>
	}
}
