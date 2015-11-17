using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Classifieds
{
	public struct ListingProperties
	{
		public Quality Quality
		{ get; set; }

		public bool Craftable
		{ get; set; }

		public bool Tradable
		{ get; set; }

		public bool Australium
		{ get; set; }
	}
}
