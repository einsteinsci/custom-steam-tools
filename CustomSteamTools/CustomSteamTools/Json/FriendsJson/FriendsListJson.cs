using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSteamTools.Json.FriendsJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class FriendsListResponseJson
	{
		public List<FriendJson> friends
		{ get; set; }

		public override string ToString()
		{
			return friends.ToString();
		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public sealed class FriendsListJson
	{
		public FriendsListResponseJson friendslist
		{ get; set; }

		public override string ToString()
		{
			return friendslist.ToString();
		}
	}
}
