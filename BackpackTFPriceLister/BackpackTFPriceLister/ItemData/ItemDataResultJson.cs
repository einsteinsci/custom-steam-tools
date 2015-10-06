using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemDataResultJson
	{
		// should always be 1
		public int status
		{ get; set; }

		public string items_game_url
		{ get; set; }
	}
}
