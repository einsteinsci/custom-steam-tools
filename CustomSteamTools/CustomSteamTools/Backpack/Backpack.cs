﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;
using CustomSteamTools.Json.BackpackDataJson;
using CustomSteamTools.Lookup;

namespace CustomSteamTools
{
	public class Backpack
	{
		public int SlotCount
		{ get; private set; }

		public List<ItemInstance> Items
		{ get; private set; }

		public Backpack(TF2BackpackJson json, GameSchema reference) : this(json.result, reference)
		{ }

		public Backpack(TF2BackpackResultJson json, GameSchema reference)
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