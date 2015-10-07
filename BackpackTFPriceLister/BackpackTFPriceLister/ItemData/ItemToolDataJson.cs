using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemToolDataJson
	{
		public string type
		{ get; set; }

		public ItemToolUsageCapabilitiesJson usage_capabilities
		{ get; set; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemToolUsageCapabilitiesJson
	{
		public bool decodable
		{ get; set; }
	}
}
