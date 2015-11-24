using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Items;
using UltimateUtil;

namespace CustomSteamTools.Classifieds
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

		public string ListerSteamID64
		{ get; set; }

		public string ListerNickname
		{ get; set; }

		public string OfferURL
		{ get; set; }

		public ClassifiedsListing(ItemInstance inst, Price price, string steamID, string nickname,
			string url, string comment = "", OrderType order = OrderType.Sell)
		{
			ItemInstance = inst;
			Price = price;
			ListerSteamID64 = steamID;
			ListerNickname = nickname;

			Comment = comment;
			if (!Comment.IsNullOrWhitespace())
			{
				Comment = WebUtility.HtmlDecode(Comment);
			}

			OrderType = order;
			OfferURL = url;
		}

		public string ToString(bool complex, int commentLength, char esc)
		{
			string res = "";
			if (complex)
			{
				res = "&2[{0}] &f{1} &7at &2{2} &7from &f{3}".Replace('&', esc).Fmt(OrderType.ToString().ToUpper(),
					ItemInstance.ToString(), Price.ToString(), ListerNickname ?? ListerSteamID64);
			}
			else
			{
				res = "[" + OrderType.ToString().ToUpper() + "] ";
				res += ItemInstance.ToString() + " at " + Price.ToString() + " from " + (ListerNickname ?? ListerSteamID64);
			}

			if (commentLength != -1 && Comment != null)
			{
				if (complex)
				{
					res += ": " + esc + "7" + Comment.NewlinesToSpaces().Shorten(commentLength);
				}
				else
				{
					res += ": " + Comment.NewlinesToSpaces().Shorten(commentLength);
				}
			}

			return res;
		}

		public override string ToString()
		{
			return ToString(false, -1, '\\');
		}
	}
}
