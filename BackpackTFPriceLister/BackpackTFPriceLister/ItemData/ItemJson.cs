using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class ItemJson
	{
		// Localized (or Unlocalized) name
		[JsonProperty]
		public string name
		{ get; set; }

		// Item ID
		[JsonProperty]
		public int defindex
		{ get; set; }

		[JsonProperty]
		public string item_class
		{ get; set; }

		// Localized Item "Type"
		[JsonProperty]
		public string item_type_name
		{ get; set; }

		// Localized name
		[JsonProperty]
		public string item_name
		{ get; set; }

		// Prepend "The"
		[JsonProperty]
		public bool proper_name
		{ get; set; }

		[JsonProperty]
		public string item_slot
		{ get; set; }

		[JsonProperty]
		public string model_player
		{ get; set; }

		// default quality
		[JsonProperty]
		public int item_quality
		{ get; set; }

		[JsonProperty]
		public string image_inventory
		{ get; set; }

		// min level
		[JsonProperty]
		public int min_ilevel
		{ get; set; }

		// max level
		[JsonProperty]
		public int max_ilevel
		{ get; set; }

		[JsonProperty]
		public string image_url
		{ get; set; }

		[JsonProperty]
		public string image_url_large
		{ get; set; }

		[JsonProperty]
		public string item_set
		{ get; set; }

		[JsonProperty]
		public string craft_class
		{ get; set; }

		public string craft_material_type
		{ get; set; }
	}
}
