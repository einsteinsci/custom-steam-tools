using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.FriendsJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class PlayerSummaryJson
	{
		public string steamid
		{ get; set; }

		public string personaname
		{ get; set; }

		public string profileurl
		{ get; set; }

		public string avatar
		{ get; set; }

		public string avatarmedium
		{ get; set; }

		public string avatarfull
		{ get; set; }

		public byte personastate
		{ get; set; }

		public byte communityvisibilitystate
		{ get; set; }

		public byte profilestate
		{ get; set; }

		public long lastlogoff
		{ get; set; }

		public byte commentpermission
		{ get; set; }

		public string realname
		{ get; set; }

		public string primaryclanid
		{ get; set; }

		public long timecreated
		{ get; set; }

		public int gameid
		{ get; set; }

		public string gameserverip
		{ get; set; } = "0.0.0.0:0";

		public string gameextrainfo
		{ get; set; }

		public string loccountrycode
		{ get; set; }

		public string locstatecode
		{ get; set; }

		public string loccityid
		{ get; set; }
	}
}
