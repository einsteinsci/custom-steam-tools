using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class QualitiesNameListJson
	{
		public string Normal // 0
		{ get; set; }

		public string rarity1 // 1 (genuine)
		{ get; set; }

		public string rarity2 // 2
		{ get; set; }

		public string vintage // 3
		{ get; set; }

		public string rarity3 // 4
		{ get; set; }

		public string rarity4 // 5 (unusual)
		{ get; set; }

		public string Unique // 6
		{ get; set; }

		public string community // 7
		{ get; set; }

		public string developer // 8
		{ get; set; }

		public string selfmade // 9
		{ get; set; }

		public string customized // 10
		{ get; set; }

		public string strange // 11
		{ get; set; }

		public string completed // 12
		{ get; set; }

		public string haunted // 13
		{ get; set; }

		public string collectors // 14
		{ get; set; }

		public string paintkitweapon // 15
		{ get; set; }

		public QualitiesNameListJson()
		{
			Normal = "Normal";
		}
	}
}
