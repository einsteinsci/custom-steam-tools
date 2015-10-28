using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Items;

namespace CustomSteamTools.Classifieds
{
    public class ItemSale
    {
        public ItemPricing Pricing
        { get; private set; }

        public Item Item => Pricing.Item;

        public List<ClassifiedsListing> Sellers
        { get; private set; }

		public List<ClassifiedsListing> Buyers
		{ get; private set; }

        public bool HasQuickDeal
        { get; set; }

		public Price Profit
		{
			get
			{
				return Pricing.PriceMid - CheapestSeller.Price;
			}
		}

		public ClassifiedsListing CheapestSeller
		{
			get
			{
				Price? low = null;
				ClassifiedsListing res = null;
				foreach (ClassifiedsListing c in Sellers)
				{
					if (low == null || c.Price < low.Value)
					{
						low = c.Price;
						res = c;
					}
				}

				return res;
			}
		}

		public ClassifiedsListing HighestBuyer
		{
			get
			{
				Price? high = null;
				ClassifiedsListing res = null;
				foreach (ClassifiedsListing c in Buyers)
				{
					if (high == null || c.Price > high.Value)
					{
						high = c.Price;
						res = c;
					}
				}

				return res;
			}
		}

        public ItemSale(ItemPricing pricing)
        {
            Pricing = pricing;

			HasQuickDeal = false;

			Sellers = new List<ClassifiedsListing>();
			Buyers = new List<ClassifiedsListing>();
        }

		public override string ToString()
		{
			string res = "";
			if (HasQuickDeal)
			{
				res += "[QUICK] ";
			}

			ClassifiedsListing cheapest = CheapestSeller;

			res += Pricing.ToUnpricedString();
			res += " > Starting at " + cheapest.Price.ToString();
			res += " (" + Profit.ToString() + " profit)";
			res += " from " + (cheapest.ListerNickname ?? cheapest.ListerSteamID64);

			return res;
		}

		public object[] ToComplexOutput(string prefix = "", string suffix = "")
		{
			List<object> res = new List<object>();
			res.Add(prefix);

			if (HasQuickDeal)
			{
				res.Add(ConsoleColor.Green);
				res.Add("[QUICK] ");
			}

			ClassifiedsListing cheapest = CheapestSeller;

			res.Add(Pricing.Quality.GetColor());
			res.Add(Pricing.ToUnpricedString());

			res.Add(ConsoleColor.DarkGray);
			res.Add(" > Starting at ");

			res.Add(ConsoleColor.Gray);
			res.Add(cheapest.Price.ToString() + " (");
			res.Add(ConsoleColor.White);
			res.Add(Profit.ToString() + " profit");

			res.Add(ConsoleColor.Gray);
			res.Add(") from ");

			res.Add(ConsoleColor.Gray);
			res.Add(cheapest.ListerNickname ?? cheapest.ListerSteamID64);

			res.Add(suffix);

			return res.ToArray();
		}
	}
}
