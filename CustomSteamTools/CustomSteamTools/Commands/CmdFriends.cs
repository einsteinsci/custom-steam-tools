using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Friends;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public sealed class CmdFriends : ITradeCommand
	{
		public string[] Aliases => new string[] { "friends", "friendslist" };

		public string Description => "Lists all friends of a player.";

		public string RegistryName => "friends";

		public string Syntax => "friends [steamID64] [/force] [/online]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			string steamid = Settings.Instance.HomeSteamID64;
			bool force = false, online = false;
			foreach (string a in args)
			{
				if (!a.StartsWith("/"))
				{
					steamid = a;
					continue;
				}

				if (a.EqualsIgnoreCase("/force"))
				{
					force = true;
				}
				else if (a.EqualsIgnoreCase("/online"))
				{
					online = true;
				}
			}

			PlayerList list = GetFriendsList(steamid, force);
			if (list == null)
			{
				return;
			}

			list.Sort(new PlayerList.PlayerGameStateComparer());

			VersatileIO.Info("{0} friends ({1} online):", list.Count, list.GetOnline().Count());
			foreach (Player p in list)
			{
				if (online && !p.IsOnline)
				{
					continue;
				}

				VersatileIO.WriteComplex(p.ToComplexString(), '\u00a7');
			}
		}

		public PlayerList GetFriendsList(string steamid, bool force)
		{
			bool success = DataManager.LoadOtherFriendsList(steamid, force);
			if (!success)
			{
				VersatileIO.Error("Failed to load friends list.");
				return null;
			}

			return DataManager.FriendsLists[steamid];
		}
	}
}
