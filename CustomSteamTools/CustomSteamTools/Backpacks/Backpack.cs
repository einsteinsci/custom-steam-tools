using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Json.BackpackDataJson;
using CustomSteamTools.Lookup;

namespace CustomSteamTools.Backpacks
{
	public class Backpack
	{
		public int SlotCount
		{ get; private set; }

		public List<ItemInstance> ItemsOld
		{ get; private set; }

		public BackpackPage[] Pages
		{ get; private set; }

		public List<ItemInstance> NewItems
		{ get; private set; }

		public ItemInstance this[ushort slot]
		{
			get
			{
				foreach (BackpackPage page in Pages)
				{
					ItemInstance res = page.GetItemFromFullSlot(slot);
					if (res != null)
					{
						return res;
					}
				}

				return null;
			}
			set
			{
				foreach (BackpackPage page in Pages)
				{
					if (page.SetItemFromFullSlot(value))
					{
						break;
					}
				}
			}
		}

		public Backpack(TF2BackpackJson json, GameSchema reference) : this(json.result, reference)
		{ }

		public Backpack(TF2BackpackResultJson json, GameSchema reference)
		{
			SlotCount = json.num_backpack_slots;

			List<ItemInstance> all = new List<ItemInstance>();
			foreach (ItemInstanceJson iij in json.items)
			{
				all.Add(new ItemInstance(iij, reference));
			}

			NewItems = all.FindAll((i) => i.IsNewToBackpack);
			
			int pageCount = SlotCount / 50;
			Pages = new BackpackPage[pageCount];
			for (int p = 0; p < pageCount; p++)
			{
				BackpackPage page = new BackpackPage(all, p);
				Pages[p] = page;
			}

			/*
			ItemsOld = new List<ItemInstance>();
			foreach (ItemInstanceJson iij in json.items)
			{
				ItemsOld.Add(new ItemInstance(iij, reference));
			}

			ItemsOld.Sort((a, b) => a.BackpackPos - b.BackpackPos);
			*/
		}

		public List<ItemInstance> GetAllItems()
		{
			List<ItemInstance> res = new List<ItemInstance>();
			res.AddRange(NewItems);

			foreach (BackpackPage p in Pages)
			{
				foreach (ItemInstance i in p.Items)
				{
					if (i != null)
					{
						res.Add(i);
					}
				}
			}

			return res;
		}
	}
}
