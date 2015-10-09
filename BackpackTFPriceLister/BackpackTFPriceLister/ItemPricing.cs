using BackpackTFPriceLister.PriceDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class ItemPricing
	{
		public Item Item
		{ get; set; }

		public Quality Quality
		{ get; set; }

		public bool Craftable
		{ get; set; }

		// Data means nothing if this is false
		public bool Tradable
		{ get; set; }

		// 0 for almost everything except unusuals and a few other things.
		// On chem sets this is filled by the target item's defindex
		public int PriceIndex
		{ get; set; }

		public Price PriceLow
		{ get; set; }

		public Price PriceHigh
		{ get; set; }

		public DateTime LastUpdate
		{ get; set; }

		public Price Difference
		{ get; set; }

		public Quality? ChemistrySetQuality
		{ get; set; }

		public bool IsChemistrySet => ChemistrySetQuality != null;

		public string CompiledTitleName
		{
			get
			{
				string res = Item.ImproperName;
				if (Quality.ToReadableString() != "")
				{
					res = Quality.ToReadableString() + " " + res;
				}

				if (!Tradable)
				{
					res = "Non-Tradable " + res;
				}
				else if (!Craftable)
				{
					res = "Non-Craftable " + res;
				}

				
				if ((Quality == Quality.Unique || Quality == Quality.Stock) && 
					Craftable && Tradable && Item.IsProper)
				{
					res = "The " + res;
				}

				return res;
			}
		}

		public ItemPricing(Item item, Quality quality, string currency, 
			double price, double priceHigh, string priceIndex = "0", bool craftable = true, bool tradable = true)
		{
			this.Item = item;
			Quality = quality;
			Craftable = craftable;
			Tradable = tradable;
			
			// hyphen in middle
			if (!priceIndex.StartsWith("-") && priceIndex.Contains("-"))
			{
				string[] pair = priceIndex.Split('-');
				PriceIndex = int.Parse(pair[0]);
				ChemistrySetQuality = (Quality)int.Parse(pair[1]);
			}
			else
			{
				PriceIndex = int.Parse(priceIndex);
			}

			PriceLow = new Price(price, currency);
			if (priceHigh == 0)
			{
				PriceHigh = PriceLow;
			}
			else
			{
				PriceHigh = new Price(priceHigh, currency);
			}
		}

		public string GetPriceString()
		{
			if (!Tradable)
			{
				return "n/a";
			}

			if (PriceHigh != PriceLow && PriceHigh.MatchesUnit(PriceLow))
			{
				return PriceLow.ToStringUnitless() + " - " + PriceHigh.ToString();
			}

			if (PriceHigh != PriceLow)
			{
				return PriceLow.ToString() + " - " + PriceHigh.ToString();
			}

			return "~" + PriceLow.ToString();
		}

		public override string ToString()
		{
			string res = Item.Name + " [" + PriceIndex.ToString() + "]: " + GetPriceString();
			if (Quality.ToReadableString() != "")
			{
				res = Quality.ToReadableString() + " " + res;
			}

			if (!Craftable)
			{
				res = "Non-Craftable " + res;
			}

			return res;
		}
	}
}
