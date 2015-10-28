using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemToolDataJson
	{
		public string type
		{ get; set; }

		public ItemToolUsageCapabilitiesJson usage_capabilities
		{ get; set; }

		public override string ToString()
		{
			return type + " (" + usage_capabilities.ToString() + ")";
		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemToolUsageCapabilitiesJson
	{
		public bool decodable
		{ get; set; }

		public override string ToString()
		{
			return "decodable: " + decodable.ToString();
		}
	}
}
