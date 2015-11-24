using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;

namespace CustomSteamTools.Classifieds
{
	public class ItemPricing
	{
		public List<Item> Items
		{ get; set; }

		public Item Item => Items.FirstOrDefault();

		public Quality Quality
		{ get; set; }

		public bool Craftable
		{ get; set; }

		// Data means nothing if this is false
		public bool Tradable
		{ get; set; }

		public bool Australium
		{ get; set; }

		// 0 for almost everything except unusuals and a few other things.
		// On chem sets this is filled by the target item's defindex
		public int PriceIndex
		{ get; set; }

		public PriceRange Pricing
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

				if (Australium)
				{
					res = "Australium " + res;
				}

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

		public ItemPricing(List<Item> items, Quality quality, string currency, 
			double price, double priceHigh, string priceIndex = "0", 
			bool craftable = true, bool tradable = true, bool australium = false)
		{
			Items = items;
			Quality = quality;
			Craftable = craftable;
			Tradable = tradable;
			Australium = australium;
			
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

			if (items.Contains(Price.RefinedMetal) && quality == Quality.Unique)
			{
				Pricing = new PriceRange(Price.OneRef);
			}
			else
			{
				Pricing = new PriceRange(new Price(price, currency));

				if (priceHigh != 0)
				{
					Pricing = Pricing.SetHigh(new Price(priceHigh, currency));
				}
			}
		}

		public string GetPriceString()
		{
			if (!Tradable)
			{
				return "Not Tradable";
			}

			return (Pricing.IsOnePrice ? "~" : "") + Pricing.ToString();
		}

		public override string ToString()
		{
			string res = Item.Name;

			if (PriceIndex != 0)
			{
				res += " [" + PriceIndex.ToString() + "]";
			}

			res += ": " + GetPriceString();
			if (Australium)
			{
				res = "Australium " + res;
			}

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

		public string ToUnpricedString()
		{
			string res = Item.ImproperName;

			if (PriceIndex != 0)
			{
				res += " [" + PriceIndex.ToString() + "]";
			}

			if (Australium)
			{
				res = "Australium " + res;
			}

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
