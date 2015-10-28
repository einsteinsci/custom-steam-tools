using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class CraftibilityJson
	{
		[JsonProperty]
		public Dictionary<string, TypeIndexPricingJson> Craftable
		{ get; private set; }

		[JsonProperty("Non-Craftable")]
		public Dictionary<string, TypeIndexPricingJson> NonCraftable
		{ get; set; }
	}
}
