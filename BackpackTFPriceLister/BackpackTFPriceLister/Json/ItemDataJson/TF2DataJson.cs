using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class TF2DataJson
	{
		[JsonProperty]
		public TF2DataResultJson result
		{ get; set; }
	}
}
