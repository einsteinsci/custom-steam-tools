using BackpackTFPriceLister.Json.BackpackDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class TF2BackpackData
	{
		public int SlotCount
		{ get; private set; }

		public List<ItemInstance> Items
		{ get; private set; }

		public TF2BackpackData(TF2BackpackJson json, TF2Data reference) : this(json.result, reference)
		{ }

		public TF2BackpackData(TF2BackpackResultJson json, TF2Data reference)
		{
			SlotCount = json.num_backpack_slots;

			Items = new List<ItemInstance>();
			foreach (ItemInstanceJson iij in json.items)
			{
				Items.Add(new ItemInstance(iij, reference));
			}
		}
	}
}
