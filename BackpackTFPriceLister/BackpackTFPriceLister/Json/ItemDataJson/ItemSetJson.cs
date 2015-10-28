using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemSetJson
	{
		public string item_set
		{ get; set; }

		public string name
		{ get; set; }

		public string store_bundle
		{ get; set; }

		public List<string> items
		{ get; set; }

		public List<AppliedAttributeJson> attributes
		{ get; set; }

		public override string ToString()
		{
			return name + " [" + item_set + "]";
		}
	}
}
