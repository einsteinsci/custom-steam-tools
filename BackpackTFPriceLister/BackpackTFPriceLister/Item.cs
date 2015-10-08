using BackpackTFPriceLister.ItemDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class Item
	{
		public string UnlocalizedName
		{ get; set; }

		public string Name
		{ get; set; }

		public long ID
		{ get; set; }

		public string Description
		{ get; set; }

		public string Subtext
		{ get { return "Level " + MaxLevel.ToString() + " " + Type; } }

		public string Type
		{ get; set; }

		public Quality DefaultQuality
		{ get; set; }

		public int MinLevel
		{ get; set; }

		public int MaxLevel
		{ get; set; }

		public ItemSlot Slot
		{ get; set; }

		public ItemSlotPlain PlainSlot
		{ get { return Slot.GetPlain(); } }

		public string ItemSetLookup
		{ get; set; }

		public bool HalloweenOnly
		{ get; set; }

		public string ImageURL
		{ get; set; }

		public List<string> Styles
		{ get; private set; }

		public List<PlayerClass> ValidClasses
		{ get; private set; }

		public List<ItemAttribute> Attributes
		{ get; private set; }

		public Item(ItemJson json, List<ItemAttribute> attsRef)
		{
			UnlocalizedName = json.name;
			Name = (json.proper_name ? "The " : "") + json.item_name;
			ID = json.defindex;
			Description = json.item_description;
			Type = json.item_type_name;
			DefaultQuality = (Quality)json.item_quality;
			MinLevel = json.min_ilevel;
			MaxLevel = json.max_ilevel;
			Slot = ItemSlots.Parse(json.item_slot);
			ItemSetLookup = json.item_set;
			HalloweenOnly = json.holiday_restriction == ItemJson.HolidayRestrictionTypes.HALLOWEEN;
			ImageURL = json.image_url;
			Styles = json.styles?.ConvertAll((j) => j.name) ?? new List<string>();
			ValidClasses = json.used_by_classes?.ConvertAll((s) => PlayerClasses.Parse(s)) ?? PlayerClasses.All;
			Attributes = json.attributes?.ConvertAll((aas) => attsRef.First((ia) => ia.Name == aas.name)) ?? new List<ItemAttribute>();
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			if (obj is Item)
			{
				return (obj as Item).ID == ID;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}
	}
}
