using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Json.FriendsJson;
using UltimateUtil;

namespace CustomSteamTools.Friends
{
	public sealed class Friend
	{
		public string SteamID64
		{ get; private set; }

		public string Name
		{ get; private set; }

		public PersonaState PersonaState
		{ get; private set; }

		public int CurrentGame
		{ get; set; }

		public string AvatarSmallURL
		{ get; private set; }

		public string AvatarMedURL
		{ get; private set; }

		public string AvatarLargeURL
		{ get; private set; }

		public Friend(string steamid, string name, byte personaState, string smallAvatar,
			string medAvatar, string largeAvatar)
		{
			SteamID64 = steamid;
			Name = name;
			PersonaState = (PersonaState)personaState;
			AvatarSmallURL = smallAvatar;
			AvatarMedURL = medAvatar;
			AvatarLargeURL = largeAvatar;
		}

		public Friend(PlayerJson j) : 
			this(j.steamid, j.personaname, j.personastate, j.avatar, j.avatarmedium, j.avatarfull)
		{ }

		public override string ToString()
		{
			if (Name == null)
			{
				return "#" + SteamID64;
			}

			return "{0} (#{1})".Fmt(Name, SteamID64);
		}
	}
}
