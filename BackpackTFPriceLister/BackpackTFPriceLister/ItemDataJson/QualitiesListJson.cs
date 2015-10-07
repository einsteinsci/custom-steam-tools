using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class QualitiesListJson
	{
		public int Normal // 0
		{ get; set; }

		public int rarity1 // 1
		{ get; set; }

		public int rarity2 // 2
		{ get; set; }

		public int vintage // 3
		{ get; set; }

		public int rarity3 // 4
		{ get; set; }

		public int rarity4 // 5 (unusual)
		{ get; set; }

		public int Unique // 6
		{ get; set; }

		public int community // 7
		{ get; set; }

		public int developer // 8
		{ get; set; }

		public int selfmade // 9
		{ get; set; }

		public int customized // 10
		{ get; set; }

		public int strange // 11
		{ get; set; }

		public int completed // 12
		{ get; set; }

		public int haunted // 13
		{ get; set; }

		public int collectors // 14
		{ get; set; }

		public int paintkitweapon // 15
		{ get; set; }

		public QualitiesListJson()
		{
			Normal = 0;
			rarity1 = 1;
			rarity2 = 2;
			vintage = 3;
			rarity3 = 4;
			rarity4 = 5;
			Unique = 6;
			community = 7;
			developer = 8;
			selfmade = 9;
			customized = 10;
			strange = 11;
			completed = 12;
			haunted = 13;
			collectors = 14;
			paintkitweapon = 15;
		}

		public override string ToString()
		{
			return "Quality ID List";
		}
	}
}
