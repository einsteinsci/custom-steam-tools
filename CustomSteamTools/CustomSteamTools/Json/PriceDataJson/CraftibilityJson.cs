using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CustomSteamTools.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class CraftibilityJson
	{
		[JsonProperty]
		public JToken Craftable
		{ get; private set; }

		[JsonProperty("Non-Craftable")]
		public JToken NonCraftable
		{ get; set; }

		public override string ToString()
		{
			string res = "";

			if (Craftable != null)
			{
				res += "Craftable";
			}

			if (NonCraftable != null)
			{
				if (res != "")
				{
					res += ", ";
				}

				res += "Non-Craftable";
			}

			return res == "" ? "???" : res;
		}
	}
}
