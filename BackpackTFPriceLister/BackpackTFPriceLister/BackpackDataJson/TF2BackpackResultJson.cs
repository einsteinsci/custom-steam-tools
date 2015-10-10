using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.BackpackDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TF2BackpackResultJson
	{
		public static class Status
		{
			public const int SUCCESS = 1;
			public const int INVALID_STEAMID = 8;
			public const int BACKPACK_PRIVATE = 15;
			public const int NO_SUCH_STEAMID = 18;
		}

		public int status
		{ get; set; }

		public int num_backpack_slots
		{ get; set; }

		public List<ItemInstanceJson> items
		{ get; set; }
    }
}
