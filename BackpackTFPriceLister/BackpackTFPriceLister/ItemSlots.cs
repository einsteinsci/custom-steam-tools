using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public enum ItemSlot
	{
		Primary,
		Secondary,
		Melee,
		Hat,
		Misc,
		PDA1,
		PDA2,
		Action,
		Taunt,
		_Building,
		_Grenade,
	}

	public enum ItemSlotPlain
	{
		Weapon,
		Cosmetic,
		Action,
		Taunt,
		Unused
	}

	public static class ItemSlots
	{
		public const string WEAP_PRIMARY = "primary";
		public const string WEAP_SECONDARY = "secondary";
		public const string WEAP_MELEE = "melee";
		public const string HAT = "head";
		public const string MISC = "misc";
		public const string PDA_BUILD_DISGUISE = "pda";
		public const string PDA_DESTROY_CLOAK = "pda2";
		public const string ACTION = "action";
		public const string TAUNT = "taunt";

		public static class Plain
		{
			public const string WEAPON = "weapon";
			public const string HAT = "cosmetic";
			public const string ACTION = "action";
			public const string TAUNT = "taunt";
			public const string UNUSED = "unused";

			public static ItemSlotPlain Parse(string s)
			{
				if (s == null)
				{
					return ItemSlotPlain.Unused;
				}

				string sl = s.ToLower();

				if (sl == WEAPON)
				{
					return ItemSlotPlain.Weapon;
				}
				if (sl == HAT || sl == "hat")
				{
					return ItemSlotPlain.Cosmetic;
				}
				if (sl == ACTION)
				{
					return ItemSlotPlain.Action;
				}
				if (sl == TAUNT)
				{
					return ItemSlotPlain.Taunt;
				}

				return ItemSlotPlain.Unused;
			}
		}

		public const string UNUSED_BUILDING = "building";
		public const string UNUSED_GRENADE = "grenade";

		public static ItemSlot Parse(string s)
		{
			if (s == null)
			{
				return ItemSlot._Grenade;
			}

			string sl = s.ToLower();

			if (sl == WEAP_PRIMARY)
			{
				return ItemSlot.Primary;
			}
			if (sl == WEAP_SECONDARY)
			{
				return ItemSlot.Secondary;
			}
			if (sl == WEAP_MELEE)
			{
				return ItemSlot.Melee;
			}
			if (sl == HAT)
			{
				return ItemSlot.Hat;
			}
			if (sl == MISC)
			{
				return ItemSlot.Misc;
			}
			if (sl == PDA_BUILD_DISGUISE)
			{
				return ItemSlot.PDA1;
			}
			if (sl == PDA_DESTROY_CLOAK)
			{
				return ItemSlot.PDA2;
			}
			if (sl == ACTION)
			{
				return ItemSlot.Action;
			}
			if (sl == TAUNT)
			{
				return ItemSlot.Taunt;
			}
			if (sl == UNUSED_BUILDING)
			{
				return ItemSlot._Building;
			}

			return ItemSlot._Grenade;
		}

		public static string GetString(this ItemSlot slot)
		{
			switch (slot)
			{
				case ItemSlot.Primary:
					return WEAP_PRIMARY;
				case ItemSlot.Secondary:
					return WEAP_SECONDARY;
				case ItemSlot.Melee:
					return WEAP_MELEE;
				case ItemSlot.Hat:
					return HAT;
				case ItemSlot.Misc:
					return MISC;
				case ItemSlot.PDA1:
					return PDA_BUILD_DISGUISE;
				case ItemSlot.PDA2:
					return PDA_DESTROY_CLOAK;
				case ItemSlot.Action:
					return ACTION;
				case ItemSlot.Taunt:
					return TAUNT;
				case ItemSlot._Building:
					return UNUSED_BUILDING;
				case ItemSlot._Grenade:
					return UNUSED_GRENADE;
				default:
					return "ERR";
			}
		}

		public static ItemSlotPlain GetPlain(this ItemSlot slot)
		{
			if (slot == ItemSlot.Primary || slot == ItemSlot.Secondary || slot == ItemSlot.Melee ||
				slot == ItemSlot.PDA1 || slot == ItemSlot.PDA2)
			{
				return ItemSlotPlain.Weapon;
			}

			if (slot == ItemSlot.Hat || slot == ItemSlot.Misc)
			{
				return ItemSlotPlain.Cosmetic;
			}

			if (slot == ItemSlot.Action)
			{
				return ItemSlotPlain.Action;
			}

			if (slot == ItemSlot.Taunt)
			{
				return ItemSlotPlain.Taunt;
			}

			return ItemSlotPlain.Unused;
		}
	}
}
