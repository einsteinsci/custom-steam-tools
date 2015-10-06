using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CapabilitiesJson
	{
		public bool paintable
		{ get; set; }

		public bool nameable
		{ get; set; }

		public bool can_craft_if_purchased
		{ get; set; }

		public bool can_gift_wrap
		{ get; set; }

		public bool can_craft_count
		{ get; set; }

		public bool can_craft_mark
		{ get; set; }

		public bool can_be_restored
		{ get; set; }

		public bool strange_parts
		{ get; set; }

		public bool can_card_upgrade
		{ get; set; }

		public bool can_strangify
		{ get; set; }

		public bool can_killstreakify
		{ get; set; }

		public bool can_consume
		{ get; set; }
	}
}
