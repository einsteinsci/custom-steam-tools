using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class UnusualEffectJson
	{
		public string system
		{ get; set; }

		public int id
		{ get; set; }

		public bool attach_to_rootbone
		{ get; set; }

		public string attachment
		{ get; set; }

		public string name
		{ get; set; }

		public override string ToString()
		{
			return "#" + id + ": " + name + "(" + system + ")";
		}
	}
}
