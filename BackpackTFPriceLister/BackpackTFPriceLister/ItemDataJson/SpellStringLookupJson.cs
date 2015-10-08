using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class SpellStringLookupJson
	{
		public string table_name
		{ get; set; }

		public List<SpellStringJson> strings
		{ get; set; }

		public override string ToString()
		{
			return table_name + ": " + strings.Count.ToString() + " spell strings";
		}
	}
}
