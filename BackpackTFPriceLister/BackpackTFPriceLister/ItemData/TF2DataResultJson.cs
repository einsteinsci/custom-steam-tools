using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TF2DataResultJson
	{
		// should always be 1
		public int status
		{ get; set; }

		public string items_game_url
		{ get; set; }

		public QualitiesListJson qualities
		{ get; set; }

		public QualitiesNameListJson qualityNames
		{ get; set; }

		public List<OriginJson> originNames
		{ get; set; }

		// HERE IT IS
		public List<ItemJson> items
		{ get; set; }

		public List<AttributeJson> attributes
		{ get; set; }

		public List<ItemSetJson> item_sets
		{ get; set; }

		// AKA Unusual FX
		public List<UnusualEffectJson> attribute_controlled_attached_particles
		{ get; set; }

		public List<StrangeLevelingSystemJson> item_levels
		{ get; set; }

		public List<StrangePartJson> kill_eater_score_types
		{ get; set; }

		public List<SpellStringLookupJson> string_lookups
		{ get; set; }

		public override string ToString()
		{
			return "TF2 Item Data Results";
		}
	}
}
