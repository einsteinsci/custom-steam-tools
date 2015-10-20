using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class ClassifiedsListing
	{
		public bool IsBuyOrder
		{ get; set; }

		public ItemInstance ItemInstance
		{ get; set; }

		public string Comment
		{ get; set; }

		public Price Price
		{ get; set; }

		public string SteamID64
		{ get; set; }

		public string OfferURL
		{ get; set; }

		public ClassifiedsListing(ItemInstance inst, Price price, string steamID, string url,
			string comment = "", bool buying = false)
		{
			ItemInstance = inst;
			Price = price;
			SteamID64 = steamID;
			Comment = comment;
			IsBuyOrder = buying;
			OfferURL = url;
		}
	}
}
