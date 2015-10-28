using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackpackTFPriceLister.Json.ItemDataJson;

namespace BackpackTFPriceLister.Items
{
	public class StrangePart
	{
		public int ID
		{ get; set; }

		public string Name
		{ get; set; }

		public string LevelingSystem
		{ get; set; }

		public StrangePart(StrangePartJson json)
		{
			ID = json.type;
			Name = json.type_name;
			LevelingSystem = json.level_data;
		}
	}
}
