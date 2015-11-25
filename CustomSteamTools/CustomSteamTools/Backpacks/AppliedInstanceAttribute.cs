using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Json.BackpackDataJson;

namespace CustomSteamTools.Backpacks
{
	public class AppliedInstanceAttribute
	{
		public int ID
		{ get; private set; }

		public object ValueObj
		{ get; private set; }

		public double Value
		{ get; private set; }

		public ulong? SteamID
		{ get; private set; }

		public string SteamNickname
		{ get; private set; }

		public AppliedInstanceAttribute(AppliedItemInstanceAttributeJson json)
		{
			ID = json.defindex;
			ValueObj = json.value;
			Value = json.float_value;

			if (json.account_info != null)
			{
				SteamID = json.account_info.steamid;
				SteamNickname = json.account_info.personaname;
			}
		}
	}
}
