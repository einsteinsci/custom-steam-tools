using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemPerClassLoadoutSlotJson
	{
		public string Scout
		{ get; set; }

		public string Soldier
		{ get; set; }

		public string Pyro
		{ get; set; }

		public string Demoman
		{ get; set; }

		public string Heavy
		{ get; set; }

		public string Engineer
		{ get; set; }

		public string Medic
		{ get; set; }

		public string Sniper
		{ get; set; }

		public string Spy
		{ get; set; }
	}
}
