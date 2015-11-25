using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;

namespace CustomSteamTools.Classifieds
{
	public class ListingProperties
	{
		public Quality Quality
		{ get; set; }

		public bool Craftable
		{ get; set; }

		public bool Tradable
		{ get; set; }

		public bool Australium
		{ get; set; }

		public ListingProperties()
		{
			Craftable = true;
			Tradable = true;
			Australium = false;
			Quality = Quality.Unique;
		}

		public string ToString(Item item)
		{
			string res = Quality.ToReadableString() + " " + item.Name;

			if (Australium)
			{
				res = "Australium " + res;
			}

			if (!Craftable)
			{
				res = "Non-Craftable " + res;
			}

			if (!Tradable)
			{
				res = "Non-Tradable " + res;
			}

			return res;
		}
	}
}
