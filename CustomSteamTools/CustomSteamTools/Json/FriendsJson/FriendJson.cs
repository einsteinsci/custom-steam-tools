using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.FriendsJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class FriendJson
	{
		public string steamid
		{ get; set; }

		public string relationship
		{ get; set; }

		public long friend_since
		{ get; set; }

		public override string ToString()
		{
			return "#" + steamid + " (" + relationship + ")";
		}
	}
}
