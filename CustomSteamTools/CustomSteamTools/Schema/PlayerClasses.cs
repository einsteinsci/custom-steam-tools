using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools.Schema
{
	public enum PlayerClass
	{
		Scout = 1,
		Soldier = 2,
		Pyro = 3,
		Demoman = 4,
		Heavy = 5,
		Engineer = 6,
		Medic = 7,
		Sniper = 8,
		Spy = 9
	}

	public static class PlayerClasses
	{
		public const string SCOUT = "Scout";
		public const string SOLDIER = "Soldier";
		public const string PYRO = "Pyro";

		public const string DEMOMAN = "Demoman";
		public const string DEMOMAN_ALT = "Demo";
		public const string HEAVY = "Heavy";
		public const string HEAVY_ALT = "HWG";
		public const string ENGINEER = "Engineer";
		public const string ENGINEER_ALT = "Engie";

		public const string MEDIC = "Medic";
		public const string SNIPER = "Sniper";
		public const string SPY = "Spy";

		public static List<PlayerClass> All
		{
			get
			{
				List<PlayerClass> res = new List<PlayerClass>();
				for (int i = 1; i <= 9; i++)
				{
					res.Add((PlayerClass)i);
				}
				return res;
			}
		}

		public static string GetClassString(PlayerClass c)
		{
			switch (c)
			{
				case PlayerClass.Scout:
					return SCOUT;
				case PlayerClass.Soldier:
					return SOLDIER;
				case PlayerClass.Pyro:
					return PYRO;
				case PlayerClass.Demoman:
					return DEMOMAN;
				case PlayerClass.Heavy:
					return HEAVY;
				case PlayerClass.Engineer:
					return ENGINEER;
				case PlayerClass.Medic:
					return MEDIC;
				case PlayerClass.Sniper:
					return SNIPER;
				case PlayerClass.Spy:
					return SPY;
				default:
					return "ERR";
			}
		}

		public static string GetClassString(int id)
		{
			return GetClassString((PlayerClass)id);
		}

		public static PlayerClass Parse(string s)
		{
			if (s.EqualsIgnoreCase(SCOUT))
			{
				return PlayerClass.Scout;
			}
			if (s.EqualsIgnoreCase(SOLDIER))
			{
				return PlayerClass.Soldier;
			}
			if (s.EqualsIgnoreCase(PYRO))
			{
				return PlayerClass.Pyro;
			}
			if (s.EqualsIgnoreCase(DEMOMAN) || s.EqualsIgnoreCase(DEMOMAN_ALT))
			{
				return PlayerClass.Demoman;
			}
			if (s.EqualsIgnoreCase(HEAVY) || s.EqualsIgnoreCase(HEAVY_ALT))
			{
				return PlayerClass.Heavy;
			}
			if (s.EqualsIgnoreCase(ENGINEER) || s.EqualsIgnoreCase(ENGINEER_ALT))
			{
				return PlayerClass.Engineer;
			}
			if (s.EqualsIgnoreCase(MEDIC))
			{
				return PlayerClass.Medic;
			}
			if (s.EqualsIgnoreCase(SNIPER))
			{
				return PlayerClass.Sniper;
			}
			if (s.EqualsIgnoreCase(SPY))
			{
				return PlayerClass.Spy;
			}

			throw new FormatException("Invalid PlayerClass: " + s);
		}
		public static PlayerClass? ParseNullable(string s)
		{
			try
			{
				return Parse(s);
			}
			catch (FormatException)
			{
				return null;
			}
		}
		public static bool TryParse(string s, out PlayerClass result)
		{
			try
			{
				result = Parse(s);
				return true;
			}
			catch (FormatException)
			{
				result = default(PlayerClass);
				return false;
			}
		}

		public static bool IsAllClass(this IEnumerable<PlayerClass> list)
		{
			if (!list.Contains(PlayerClass.Scout))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Soldier))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Pyro))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Demoman))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Heavy))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Engineer))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Medic))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Sniper))
			{
				return false;
			}
			if (!list.Contains(PlayerClass.Spy))
			{
				return false;
			}

			return true;
		}
	}
}
