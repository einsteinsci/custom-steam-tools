using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Json.FriendsJson;
using CustomSteamTools.Utils;
using UltimateUtil;

namespace CustomSteamTools.Friends
{
	public sealed class Player
	{
		public string SteamID64
		{ get; private set; }

		public string Name
		{ get; private set; }

		public PersonaState PersonaState
		{ get; private set; }

		public bool IsOnline => PersonaState != PersonaState.Offline || IsInGame;

		public int CurrentGame
		{ get; set; }

		public bool IsInGame => CurrentGame != 0;

		public string AvatarSmallURL
		{ get; private set; }

		public string AvatarMedURL
		{ get; private set; }

		public string AvatarLargeURL
		{ get; private set; }

		// minor
		public string ProfileURL
		{ get; private set; }

		public string RealName
		{ get; private set; }

		public string PrimaryClanID
		{ get; private set; }

		public string CurrentGameServer
		{ get; private set; }

		public Player(string steamid, string name, byte personaState, string smallAvatar,
			string medAvatar, string largeAvatar)
		{
			SteamID64 = steamid;
			Name = name;
			PersonaState = (PersonaState)personaState;
			AvatarSmallURL = smallAvatar;
			AvatarMedURL = medAvatar;
			AvatarLargeURL = largeAvatar;
		}

		public Player(PlayerSummaryJson j) : 
			this(j.steamid, j.personaname, j.personastate, j.avatar, j.avatarmedium, j.avatarfull)
		{
			ProfileURL = j.profileurl;
			RealName = j.realname;
			PrimaryClanID = j.primaryclanid;
			CurrentGameServer = j.gameserverip;
		}

		public override string ToString()
		{
			if (Name == null)
			{
				return "#" + SteamID64;
			}

			return "{0} (#{1})".Fmt(Name, SteamID64);
		}

		public string ToComplexString(char esc = '\u00a7')
		{
			ConsoleColor stateColor = ConsoleColor.Gray;

			if (CurrentGame != 0)
			{
				stateColor = ConsoleColor.Green;
			}
			else if (PersonaState != PersonaState.Offline)
			{
				stateColor = ConsoleColor.Blue;
			}
			string stateCode = stateColor.ToCode(esc);

			string res = stateCode + "[";
			if (CurrentGame != 0)
			{
				res += "In-Game: " + CurrentGame.ToString() + "] " + esc.ToString() + "f";
			}
			else
			{
				res += PersonaState.ToReadableString() + "] " + esc.ToString() + "f";
			}

			res += Name + " " + esc.ToString() + "7(#" + SteamID64 + ")";

			return res;
		}
	}
}
