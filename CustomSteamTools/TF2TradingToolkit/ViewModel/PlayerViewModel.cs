using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CustomSteamTools.Friends;

namespace TF2TradingToolkit.ViewModel
{
	public class PlayerViewModel
	{
		public Player Player
		{ get; private set; }

		public string ProfilePicURL => Player.AvatarSmallURL;
		public string PersonaName => Player.Name;
		public string CurrentState => GetCurrentStateString();
		public string SteamID => Player.SteamID64;

		public SolidColorBrush StateColor => new SolidColorBrush(GetStateColor());

		public PlayerViewModel(Player p)
		{
			Player = p;
		}

		public string GetCurrentStateString()
		{
			if (Player.IsInGame)
			{
				return "In-Game #" + Player.CurrentGame.ToString();
			}
			else
			{
				return Player.PersonaState.ToReadableString();
			}
		}

		public Color GetStateColor()
		{
			if (Player.PersonaState == PersonaState.Offline)
			{
				return Colors.DarkGray;
			}
			else if (Player.IsInGame)
			{
				return Colors.Green;
			}
			else
			{
				return Colors.DodgerBlue;
			}
		}
	}
}
