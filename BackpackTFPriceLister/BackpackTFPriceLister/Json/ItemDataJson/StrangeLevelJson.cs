using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class StrangeLevelJson
	{
		public int level
		{ get; set; }

		public int required_score
		{ get; set; }

		public string name
		{ get; set; }

		public override string ToString()
		{
			return "#" + level + ": " + name + " (" + required_score + " count)";
		}
	}
}
