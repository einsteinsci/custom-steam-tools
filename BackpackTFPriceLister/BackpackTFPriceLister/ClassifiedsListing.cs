using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public enum OrderType
	{
		Buy,
		Sell
	}

	public class ClassifiedsListing
	{
		public OrderType OrderType
		{ get; set; }

		public ItemInstance ItemInstance
		{ get; set; }

		public string Comment
		{ get; set; }

		public Price Price
		{ get; set; }

		public string SellerSteamID64
		{ get; set; }

		public string SellerNickname
		{ get; set; }

		public string OfferURL
		{ get; set; }

		public ClassifiedsListing(ItemInstance inst, Price price, string steamID, string nickname,
			string url, string comment = "", OrderType order = OrderType.Sell)
		{
			ItemInstance = inst;
			Price = price;
			SellerSteamID64 = steamID;
			SellerNickname = nickname;
			Comment = comment;
			OrderType = order;
			OfferURL = url;
		}
	}
}
