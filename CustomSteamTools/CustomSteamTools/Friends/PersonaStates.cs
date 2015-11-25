using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools.Friends
{
	public enum PersonaState : byte
	{
		Offline = 0,
		Online,
		Busy,
		Away,
		Snooze,
		LookingToTrade,
		LookingToPlay,
	}

	public static class PersonaStates
	{
		public static string ToReadableString(this PersonaState state)
		{
			switch (state)
			{
			case PersonaState.LookingToTrade:
				return "Looking to Trade";
			case PersonaState.LookingToPlay:
				return "Looking to Play";
			default:
				return state.ToString();
			}
		}

		public static PersonaState? ParseNullable(string s)
		{
			for (PersonaState ps = PersonaState.Offline; ps <= PersonaState.LookingToPlay; ps++)
			{
				if (s.EqualsIgnoreCase(ps.ToReadableString()) ||
					s.EqualsIgnoreCase(ps.ToString()))
				{
					return ps;
				}
			}

			return null;
		}
		public static PersonaState Parse(string s)
		{
			PersonaState? n = ParseNullable(s);
			if (n == null)
			{
				return PersonaState.Offline;
			}
			
			return n.Value;
		}
		public static bool TryParse(string str, out PersonaState result)
		{
			PersonaState? n = ParseNullable(str);
			if (n == null)
			{
				result = PersonaState.Offline;
				return false;
			}
			else
			{
				result = n.Value;
				return true;
			}
		}

		public static bool IsPresent(this PersonaState state)
		{
			switch (state)
			{
			case PersonaState.Offline:
				return false;
			case PersonaState.Online:
				return true;
			case PersonaState.Busy:
			case PersonaState.Away:
			case PersonaState.Snooze:
				return false;
			case PersonaState.LookingToTrade:
			case PersonaState.LookingToPlay:
				return true;
			default:
				return false;
			}
		}

		public static int GetTier(this PersonaState state)
		{
			switch (state)
			{
			case PersonaState.Offline:
				return 0;
			case PersonaState.Online:
				return 3;
			case PersonaState.Busy:
				return 1;
			case PersonaState.Away:
				return 1;
			case PersonaState.Snooze:
				return 2;
			case PersonaState.LookingToTrade:
				return 3;
			case PersonaState.LookingToPlay:
				return 4;
			default:
				return 0;
			}
		}
	}
}
