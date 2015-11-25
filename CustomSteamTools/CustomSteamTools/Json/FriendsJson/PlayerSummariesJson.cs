using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.FriendsJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class PlayerSummariesJson
	{
		public PlayerSummariesResponseJson response
		{ get; set; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class PlayerSummariesResponseJson
	{
		public List<PlayerSummaryJson> players
		{ get; set; }
	}
}
