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
		public double value
		{ get; set; }

		public override string ToString()
		{
			return @class + ": " + value.ToString();
		}
	}
}
