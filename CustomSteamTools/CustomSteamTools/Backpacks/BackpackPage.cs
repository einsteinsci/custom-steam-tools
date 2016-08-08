using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools.Backpacks
{
	public sealed class BackpackPage
	{
		public int PageID
		{ get; private set; }

		public ushort MinSlotID => (ushort)(PageID * 50);
		public ushort MaxSlotID => (ushort)((PageID + 1) * 50 - 1);

		public ItemInstance[] Items
		{ get; private set; }

		public bool IsEmpty => Items.Count() == 0;

		public BackpackPage(List<ItemInstance> items, int pageid)
		{
			PageID = pageid;

			Items = new ItemInstance[50];

			foreach (ItemInstance i in items)
			{
				if ((i.BackpackSlot - 1).IsBetween(MinSlotID, MaxSlotID) && !i.IsNewToBackpack)
				{
					Items[i.BackpackSlot - MinSlotID - 1] = i;
				}
			}
		}

		public ItemInstance GetItemFromFullSlot(ushort slot)
		{
			if (!slot.IsBetween(MinSlotID, MaxSlotID))
			{
				return null;
			}

			return Items.FirstOrDefault((i) => i.BackpackSlot == slot);
		}

		public bool SetItemFromFullSlot(ItemInstance inst)
		{
			if (!inst.BackpackSlot.IsBetween(MinSlotID, MaxSlotID))
			{
				return false;
			}

			int index = inst.BackpackSlot - MinSlotID;
			Items[index] = inst;
			return true;
		}
		public bool SetItemFromFullSlot(ushort slot, ItemInstance inst)
		{
			inst.BackpackSlot = slot;
			return SetItemFromFullSlot(inst);
		}

		public override string ToString()
		{
			int notNull = 0;
			for (int i = 0; i < 50; i++)
			{
				if (Items[i] != null)
				{
					notNull++;
				}
			}

			return "Page " + PageID + ": " + notNull + " items";
		}
	}
}
