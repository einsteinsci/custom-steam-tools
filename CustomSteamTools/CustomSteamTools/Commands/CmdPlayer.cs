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
	public class CmdPlayer : ITradeCommand
	{
		public string[] Aliases => new string[] { "player", "playerinfo" };

		public string Description => "Gets information about a player.";

		public string RegistryName => "player";

		public string Syntax => "player [steamiID64] [/force]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			bool force = false;
			string steamid = Settings.Instance.HomeSteamID64;
			foreach (string a in args)
			{
				if (!a.StartsWith("/"))
				{
					steamid = a;
				}

				if (a.EqualsIgnoreCase("/force"))
				{
					force = true;
				}
			}

			Player p = GetPlayerInfo(steamid, force);
			if (p == null)
			{
				VersatileIO.Error("  Could not retrieve player data.");
				return;
			}

			VersatileIO.WriteLine();
			VersatileIO.WriteComplex(p.ToComplexString(), '\u00a7');
			VersatileIO.WriteComplex("  &fProfile URL: &7" + p.ProfileURL);
			if (!p.RealName.IsNullOrEmpty())
			{
				VersatileIO.WriteComplex("  &fReal Name: &7" + p.RealName);
			}
			VersatileIO.WriteComplex("  &fPrimary Clan ID: &7" + p.PrimaryClanID);
			VersatileIO.WriteComplex("  &fAvatar URL: &7" + p.AvatarLargeURL);
			if (p.CurrentGameServer != null && p.CurrentGameServer != "0.0.0.0:0")
			{
				VersatileIO.WriteComplex("  &fCurrent Server: &7" + p.CurrentGameServer);
			}
		}

		public static Player GetPlayerInfo(string steamid, bool force)
		{
			PlayerList l = DataManager.GetPlayerInfos(steamid.Once<string>().ToList(), force);
			if (l.IsNullOrEmpty())
			{
				return null;
			}

			return l.First();
		}
	}
}
