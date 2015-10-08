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
			if (s == SCOUT)
			{
				return PlayerClass.Scout;
			}
			if (s == SOLDIER)
			{
				return PlayerClass.Soldier;
			}
			if (s == PYRO)
			{
				return PlayerClass.Pyro;
			}
			if (s == DEMOMAN)
			{
				return PlayerClass.Demoman;
			}
			if (s == HEAVY)
			{
				return PlayerClass.Heavy;
			}
			if (s == ENGINEER)
			{
				return PlayerClass.Engineer;
			}
			if (s == MEDIC)
			{
				return PlayerClass.Medic;
			}
			if (s == SNIPER)
			{
				return PlayerClass.Sniper;
			}
			if (s == SPY)
			{
				return PlayerClass.Spy;
			}

			return 0;
		}
	}
}
