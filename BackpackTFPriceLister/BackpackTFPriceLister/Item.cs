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

		public string ImproperName
		{ get; set; }

		public bool IsProper
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

		public bool? HasHauntedVersion
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
			ImproperName = json.item_name;
			IsProper = json.proper_name;
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

		public string GetSubtext()
		{
			if (MinLevel == MaxLevel)
			{
				return "Level " + MinLevel.ToString() + " " + Type;
			}

			return "Level " + MinLevel.ToString() + "-" + MaxLevel.ToString() + " " + Type;
		}

		public bool IsCurrency()
		{
			return ID == Price.REF_DEFINDEX || ID == Price.REC_DEFINDEX || 
				ID == Price.SCRAP_DEFINDEX || ID == Price.KEY_DEFINDEX;
		}

		public Price GetCurrencyPrice()
		{
			if (!IsCurrency())
			{
				throw new ArgumentException("Item must be currency.");
			}

			switch (ID)
			{
				case Price.REF_DEFINDEX:
					return Price.OneRef;
				case Price.REC_DEFINDEX:
					return new Price(0, 0.33);
				case Price.SCRAP_DEFINDEX:
					return new Price(0, 0.11);
				case Price.KEY_DEFINDEX:
					return Price.OneKey;
			}

			return Price.Zero;
		}

		public bool CanBeAustralium()
		{
			if (PriceLister.PriceData == null)
			{
				PriceLister.TranslatePricingData();
			}

			List<ItemPricing> pricings = PriceLister.PriceData.GetAllPriceData(this);
			foreach (ItemPricing p in pricings)
			{
				if (p.Australium)
				{
					return true;
				}
			}

			return false;
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

		public bool IsBotkiller()
		{
			return ImproperName.Contains("Botkiller");
		}
	}
}
