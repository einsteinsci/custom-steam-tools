using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;

namespace CustomSteamTools.Json.BackpackDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemInstanceJson
	{
		public ulong id
		{ get; set; }

		public ulong original_id
		{ get; set; }

		public long defindex
		{ get; set; }

		public int level
		{ get; set; }

		// 1 for most items except certain consumables
		public int quantity
		{ get; set; }

		// no idea what this means
		public int origin
		{ get; set; }

		public bool flag_cannot_trade
		{ get; set; }

		public bool flag_cannot_craft
		{ get; set; }

		public uint inventory // bit flags
		{ get; set; }

		public int quality // I value this over quantity
		{ get; set; }

		public string custom_name
		{ get; set; }

		public string custom_desc
		{ get; set; }

		public ItemInstanceJson contained_item
		{ get; set; }

		public int style // e.g., "Thirsty" vs "Thirstier"
		{ get; set; }

		public List<AppliedItemInstanceAttributeJson> attributes
		{ get; set; }

		public List<EquipInfoJson> equipped
		{ get; set; }

		public override string ToString()
		{
			string result = "#" + defindex;
			string s_quality = ((Quality)quality).ToReadableString();
			if (custom_name != null)
			{
				result = custom_name + " (" + result + ")";
			}

			if (s_quality != "")
			{
				result = s_quality + " " + result;
			}

			if (flag_cannot_craft)
			{
				result = "Non-Craftable " + result;
			}

			if (flag_cannot_trade)
			{
				result = "Non-Tradable " + result;
			}

			return result;
		}
	}
}
