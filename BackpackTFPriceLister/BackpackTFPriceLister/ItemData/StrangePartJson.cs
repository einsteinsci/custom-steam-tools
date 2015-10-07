using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
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
	}
}
