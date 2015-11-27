using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Json.ItemDataJson;

namespace CustomSteamTools.Schema
{
	public class GameSchema
	{
		const string ACCOUNT_FLAGGED_ITEM_SUBSTRING = "Your account has been flagged";
		const string ACCOUNT_BONUS_ITEM_SUBSTRING = "Congratulations! Your Honesty has been rewarded";

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

		public GameSchema(TF2DataResultJson json)
		{
			Attributes = json.attributes.ConvertAll((j) => new ItemAttribute(j));

			Items = new List<Item>();
			foreach (var j in json.items)
			{
				if (j.name.Contains(ACCOUNT_FLAGGED_ITEM_SUBSTRING) ||
					j.item_name.Contains(ACCOUNT_FLAGGED_ITEM_SUBSTRING) ||
					j.name.Contains(ACCOUNT_BONUS_ITEM_SUBSTRING) ||
					j.item_name.Contains(ACCOUNT_BONUS_ITEM_SUBSTRING))
				{
					continue;
				}

				Items.Add(new Item(j, Attributes));
			}
			
			ItemSets = json.item_sets.ConvertAll((j) => new ItemSet(j, Items, Attributes));
			Unusuals = json.attribute_controlled_attached_particles.ConvertAll((j) => new UnusualEffect(j));
			StrangeParts = json.kill_eater_score_types.ConvertAll((j) => new StrangePart(j));
		}

		public GameSchema(TF2DataJson json) : this(json.result)
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

		public UnusualEffect GetUnusual(int id)
		{
			foreach (UnusualEffect fx in Unusuals)
			{
				if (fx.ID == id)
				{
					return fx;
				}
			}

			return null;
		}
	}
}
