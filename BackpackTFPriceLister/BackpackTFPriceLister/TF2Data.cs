using BackpackTFPriceLister.ItemDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class TF2Data
	{
		// the big one
		public List<Item> Items
		{ get; set; }

		public List<ItemAttribute> Attributes
		{ get; set; }

		public List<ItemSet> ItemSets
		{ get; set; }

		public List<UnusualEffect> Unusuals
		{ get; set; }

		public List<StrangePart> StrangeParts
		{ get; set; }

		public TF2Data(TF2DataResultJson json)
		{
			Attributes = json.attributes.ConvertAll((j) => new ItemAttribute(j));
			Items = json.items.ConvertAll((j) => new Item(j, Attributes));
			ItemSets = json.item_sets.ConvertAll((j) => new ItemSet(j, Items, Attributes));
			Unusuals = json.attribute_controlled_attached_particles.ConvertAll((j) => new UnusualEffect(j));
			StrangeParts = json.kill_eater_score_types.ConvertAll((j) => new StrangePart(j));
		}

		public TF2Data(TF2DataJson json) : this(json.result)
		{ }

		public Item GetItem(long id)
		{
			foreach (Item i in Items)
			{
				if (i.ID == id)
				{
					return i;
				}
			}

			return null;
		}
	}
}
