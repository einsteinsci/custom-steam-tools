using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class AppliedAttributeJson
	{
		[JsonProperty]
		public string name
		{ get; set; }

		[JsonProperty]
		public string @class
        { get; set; }

		[JsonProperty]
		public int value
		{ get; set; }

		public AppliedAttributeJson(string _name, string _class, int val)
		{
			name = _name;
			@class = _class;
			value = val;
		}
	}
}
