using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class StyleJson
	{
		[JsonProperty]
		public string name
		{ get; set; }

		public StyleJson(string _name)
		{
			name = _name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
