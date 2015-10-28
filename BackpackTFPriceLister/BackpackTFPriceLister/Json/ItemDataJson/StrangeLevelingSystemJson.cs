using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class StrangeLevelingSystemJson
	{
		public string name
		{ get; set; }

		public List<StrangeLevelJson> levels
		{ get; set; }

		public override string ToString()
		{
			return name;
		}
	}
}
