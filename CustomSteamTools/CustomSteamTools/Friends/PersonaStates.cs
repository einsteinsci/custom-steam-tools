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
	}
}
