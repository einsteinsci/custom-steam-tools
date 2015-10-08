using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class StrangePartJson
	{
		public int type
		{ get; set; }

		public string type_name
		{ get; set; }

		public string level_data
		{ get; set; }

		public override string ToString()
		{
			return "#" + type + ": " + type_name + " (" + level_data + ")";
		}
	}
}
