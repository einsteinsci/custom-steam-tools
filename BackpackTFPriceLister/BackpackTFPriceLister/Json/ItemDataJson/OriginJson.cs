using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class OriginJson
	{
		[JsonProperty]
		public int origin
		{ get; set; }

		[JsonProperty]
		public string name
		{ get; set; }

		public OriginJson(int id, string _name)
		{
			origin = id;
			name = _name;
		}

		public OriginJson() : this(0, "Timed Drop")
		{ }

		public override string ToString()
		{
			return "#" + origin + ": " + name;
		}
	}
}
