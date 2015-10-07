using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class ItemJson
	{
		public static class DeathDropTypes
		{
			public const string NONE = "none";
			public const string DROP_DETACH = "drop";
		}

		public static class HolidayRestrictionTypes
		{
			public const string YEAR_ROUND = null;
			public const string HALLOWEEN = "halloween";
		}

		// Localized (or Unlocalized) name
		public string name
		{ get; set; }

		// Item ID
		public int defindex
		{ get; set; }
		
		// Unlocalized Item "Type"
		public string item_class
		{ get; set; }

		// Localized Item "Type"
		public string item_type_name
		{ get; set; }

		// Localized name
		public string item_name
		{ get; set; }

		public string item_description
		{ get; set; }

		// Prepend "The"
		public bool proper_name
		{ get; set; }
		
		public string item_slot
		{ get; set; }

		// default quality
		public int item_quality
		{ get; set; }
		
		public string image_inventory
		{ get; set; }
		
		public string image_url
		{ get; set; }
		
		public string image_url_large
		{ get; set; }

		public string drop_type
		{ get; set; }
		
		public string item_set
		{ get; set; }

		public string holiday_restriction
		{ get; set; }
		
		public string model_player
		{ get; set; }

		// min level
		public int min_ilevel
		{ get; set; }

		// max level
		public int max_ilevel
		{ get; set; }
		
		public string craft_class
		{ get; set; }
		
		public string craft_material_type
		{ get; set; }

		public CapabilitiesJson capabilities
		{ get; set; }

		public ItemToolDataJson tool
		{ get; set; }

		public ItemPerClassLoadoutSlotJson per_class_loadout_slots
		{ get; set; }

		public List<string> used_by_classes
		{ get; set; }

		public List<StyleJson> styles
		{ get; set; }

		public List<AppliedAttributeJson> attributes
		{ get; set; }

		public string Subtext
		{
			get
			{
				// Level 3 Chin
				return "Level " + max_ilevel.ToString() + " " + item_type_name;
			}
		}

		public override string ToString()
		{
			return "#" + defindex + ": " + name;
		}
	}
}
