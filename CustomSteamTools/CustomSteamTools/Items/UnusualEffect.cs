using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Json.ItemDataJson;

namespace CustomSteamTools.Items
{
	public class UnusualEffect
	{
		public int ID
		{ get; set; }

		public string Name
		{ get; set; }

		public UnusualEffect(UnusualEffectJson json)
		{
			ID = json.id;
			Name = json.name;
		}
	}
}
