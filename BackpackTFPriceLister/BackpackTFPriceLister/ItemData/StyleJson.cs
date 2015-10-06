using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
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
	}
}
