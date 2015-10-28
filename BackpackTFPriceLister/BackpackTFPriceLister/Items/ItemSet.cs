using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Json.ItemDataJson;

namespace CustomSteamTools.Items
{
	public class ItemSet
	{
		public string Name
		{ get; set; }

		public string UnlocalizedName
		{ get; set; }

		public List<Item> SetItems
		{ get; private set; }

		public List<ItemAttribute> Attributes
		{ get; private set; }

		public ItemSet(ItemSetJson json, List<Item> itemsRef, List<ItemAttribute> attsRef)
		{
			Name = json.name;
			UnlocalizedName = json.item_set;
			SetItems = json.items.ConvertAll((s) => itemsRef.First((i) => i.UnlocalizedName == s));
			Attributes = json.attributes?.ConvertAll((aaj) => attsRef.First((a) => a.Name == aaj.name)) ?? new List<ItemAttribute>();
		}
	}
}
