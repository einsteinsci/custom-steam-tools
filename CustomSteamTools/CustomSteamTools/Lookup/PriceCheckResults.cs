using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Items;

namespace CustomSteamTools.Lookup
{
	public sealed class PriceCheckResults
	{
		public Item Item
		{ get; private set; }

		public Skin Skin
		{ get; private set; }

		public List<CheckedPrice> Uniques
		{ get; private set; }

		public List<CheckedPrice> Unusuals
		{ get; private set; }

		public List<CheckedPrice> Others
		{ get; private set; }

		public bool NotTradable
		{ get; private set; }

		public bool HasResults
		{ get; private set; }

		public bool HasUnusuals => Unusuals.Count > 0;
		public bool HasCrates => Uniques.Count > 2;

		public List<CheckedPrice> All
		{
			get
			{
				List<CheckedPrice> res = new List<CheckedPrice>();
				res.AddRange(Uniques);
				res.AddRange(Unusuals);
				res.AddRange(Others);

				return res;
			}
		}

		public PriceCheckResults(Item item)
		{
			Item = item;
			NotTradable = true;
			HasResults = false;

			if (Item.IsSkin())
			{
				Skin = item.GetSkin();
			}

			Uniques = new List<CheckedPrice>();
			Unusuals = new List<CheckedPrice>();
			Others = new List<CheckedPrice>();
		}

		public void Add(CheckedPrice check)
		{
			if (check.Quality == Quality.Unique)
			{
				Uniques.Add(check);
			}
			else if (check.Quality == Quality.Unusual)
			{
				Unusuals.Add(check);
			}
			else
			{
				Others.Add(check);
			}

			HasResults = true;
		}
		public void Add(ItemPricing pricing)
		{
			Add(new CheckedPrice(pricing));
		}

		public void AddIfTradable(ItemPricing pricing)
		{
			if (pricing.Tradable)
			{
				Add(pricing);
				NotTradable = false;
			}
		}

		public void AddIfTradableNotChemSet(ItemPricing pricing)
		{
			if (!pricing.IsChemistrySet)
			{
				AddIfTradable(pricing);
			}
		}

		public Dictionary<string, object> MakeVersatileOptions()
		{
			Dictionary<string, object> options = new Dictionary<string, object>();
			if (HasCrates)
			{
				foreach (CheckedPrice c in Uniques)
				{
					string str = c.Pricing.PriceIndex.ToString();
					options.Add(str, "Price index " + str);
				}
			}
			if (HasUnusuals)
			{
				options.Add("u", Unusuals.Count.ToString() + " Unusual Effects");
			}
			return options;
		}
	}
}
