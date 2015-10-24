using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
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
		public const string HEAVY = "Heavy";
		public const string ENGINEER = "Engineer";

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
			if (s.ToLower() == SCOUT.ToLower())
			{
				return PlayerClass.Scout;
			}
			if (s.ToLower() == SOLDIER.ToLower())
			{
				return PlayerClass.Soldier;
			}
			if (s.ToLower() == PYRO.ToLower())
			{
				return PlayerClass.Pyro;
			}
			if (s.ToLower() == DEMOMAN.ToLower())
			{
				return PlayerClass.Demoman;
			}
			if (s.ToLower() == HEAVY.ToLower())
			{
				return PlayerClass.Heavy;
			}
			if (s.ToLower() == ENGINEER.ToLower())
			{
				return PlayerClass.Engineer;
			}
			if (s.ToLower() == MEDIC.ToLower())
			{
				return PlayerClass.Medic;
			}
			if (s.ToLower() == SNIPER.ToLower())
			{
				return PlayerClass.Sniper;
			}
			if (s.ToLower() == SPY.ToLower())
			{
				return PlayerClass.Spy;
			}

			return 0;
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
