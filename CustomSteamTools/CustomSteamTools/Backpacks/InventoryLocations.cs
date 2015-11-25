using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;

namespace CustomSteamTools.Backpacks
{
	[Flags]
	public enum InventoryLocationFlags : uint
	{
		BackpackPos = 0xFFFFu, // well 0xFu too
		Scout = 1u << 16,
		Sniper = 1u << 17,
		Soldier = 1u << 18,
		Demoman = 1u << 19,
		Medic = 1u << 20,
		Heavy = 1u << 21,
		Pyro = 1u << 22,
		Spy = 1u << 23,
		Engineer = 1u << 24,
		NewItemBits = 0x7E000000u,
		AlwaysTrue = 1u << 31
	}

	public static class InventoryLocations
	{
		public static InventoryLocationFlags GetClassFlag(this PlayerClass c)
		{
			switch (c)
			{
			case PlayerClass.Scout:
				return InventoryLocationFlags.Scout;
			case PlayerClass.Soldier:
				return InventoryLocationFlags.Soldier;
			case PlayerClass.Pyro:
				return InventoryLocationFlags.Pyro;
			case PlayerClass.Demoman:
				return InventoryLocationFlags.Demoman;
			case PlayerClass.Heavy:
				return InventoryLocationFlags.Heavy;
			case PlayerClass.Engineer:
				return InventoryLocationFlags.Engineer;
			case PlayerClass.Medic:
				return InventoryLocationFlags.Medic;
			case PlayerClass.Sniper:
				return InventoryLocationFlags.Sniper;
			case PlayerClass.Spy:
				return InventoryLocationFlags.Spy;
			default:
				return 0u;
			}
		}

		public static List<PlayerClass> GetAppliedClasses(this InventoryLocationFlags f)
		{
			List<PlayerClass> res = new List<PlayerClass>();

			if (f.HasFlag(InventoryLocationFlags.Scout))
			{
				res.Add(PlayerClass.Scout);
			}
			if (f.HasFlag(InventoryLocationFlags.Soldier))
			{
				res.Add(PlayerClass.Soldier);
			}
			if (f.HasFlag(InventoryLocationFlags.Pyro))
			{
				res.Add(PlayerClass.Pyro);
			}
			if (f.HasFlag(InventoryLocationFlags.Demoman))
			{
				res.Add(PlayerClass.Demoman);
			}
			if (f.HasFlag(InventoryLocationFlags.Heavy))
			{
				res.Add(PlayerClass.Heavy);
			}
			if (f.HasFlag(InventoryLocationFlags.Engineer))
			{
				res.Add(PlayerClass.Engineer);
			}
			if (f.HasFlag(InventoryLocationFlags.Medic))
			{
				res.Add(PlayerClass.Medic);
			}
			if (f.HasFlag(InventoryLocationFlags.Sniper))
			{
				res.Add(PlayerClass.Sniper);
			}
			if (f.HasFlag(InventoryLocationFlags.Spy))
			{
				res.Add(PlayerClass.Spy);
			}

			return res;
		}

		public static ushort GetBackpackPos(this InventoryLocationFlags f)
		{
			return (ushort)(f & InventoryLocationFlags.BackpackPos);
		}

		public static bool IsNewItem(this InventoryLocationFlags f)
		{
			return (f & InventoryLocationFlags.NewItemBits) != 0u;
		}

		public static ushort GetSlotIndex(int page, int row, int col)
		{
			return GetSlotIndex(page, row * 10 + col);
		}
		public static ushort GetSlotIndex(int page, int slot)
		{
			return (ushort)(page * 50 + slot);
		}
	}
}
