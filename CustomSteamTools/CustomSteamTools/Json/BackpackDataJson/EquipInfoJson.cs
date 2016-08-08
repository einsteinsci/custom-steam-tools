using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.BackpackDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class EquipInfoJson
	{
		public uint @class
		{ get; set; }

		public uint slot
		{ get; set; }

		public override string ToString()
		{
			return "Class " + @class + ": slot " + slot;
		}
	}
}
