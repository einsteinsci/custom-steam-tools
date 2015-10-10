using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public struct Price
	{
		public const string CURRENCY_KEYS = "keys";
		public const string CURRENCY_REF = "metal";
		public const string CURRENCY_CASH = "usd";

		public const int KEY_DEFINDEX = 5021;

		public const int SCRAP_DEFINDEX = 5000;
		public const int REC_DEFINDEX = 5001;
		public const int REF_DEFINDEX = 5002;

		public const double KEY_STOREPRICE_USD = 2.49;

		public static Price OneKey => new Price(1, 0);
		public static Price OneRef => new Price(0, 1);
		public static Price Zero => new Price(0, 0);

		public static Item RefinedMetal => PriceLister.ItemData.GetItem(REF_DEFINDEX);
		public static Item ReclaimedMetal => PriceLister.ItemData.GetItem(REC_DEFINDEX);
		public static Item ScrapMetal => PriceLister.ItemData.GetItem(SCRAP_DEFINDEX);

		public static double RefinedPerKey
		{
			get
			{
				if (_refinedPerKey == -1)
				{
					PriceLister.Initialize(false);
					PriceLister.LoadData();
					PriceLister.ParseItemsJson();
					PriceLister.ParsePricesJson();
				}

				return _refinedPerKey;
			}
			set
			{
				_refinedPerKey = value;
			}
		}
		private static double _refinedPerKey = -1;

		public double Keys
		{ get; private set; }

		public double Ref
		{ get; private set; }

		public double TotalKeys => Keys + (Ref / RefinedPerKey);
		public double TotalRefined => Ref + (Keys * RefinedPerKey);
		
		public Price(double keys, double @ref)
		{
			Keys = keys;
			Ref = @ref;

			Correct();
		}

		public Price(double value, string currency)
		{
			if (currency == CURRENCY_KEYS)
			{
				Keys = value;
				Ref = 0;
			}
			else if (currency == CURRENCY_REF)
			{
				Keys = 0;
				Ref = value;
				Correct();
			}
			else if (currency == CURRENCY_CASH)
			{
				Keys = value * KEY_STOREPRICE_USD;
				Ref = 0;
				Correct();
			}
			else
			{
				Keys = 0;
				Ref = 0;
			}
		}

		public void Correct()
		{
			if (Keys - (int)Keys != 0)
			{
				double dif = Keys - (int)Keys;
				Ref += dif * RefinedPerKey;
				Keys = (int)Keys;
			}

			while (Ref > RefinedPerKey)
			{
				Ref -= RefinedPerKey;
				Keys++;
			}
		}

		public override string ToString()
		{
			return Keys > 2.0 ? (TotalKeys.ToString("F2") + " keys") : (TotalRefined.ToString() + " ref");
		}

		public override bool Equals(object obj)
		{
			if (obj is Price)
			{
				Price other = (Price)obj;
				return other.TotalRefined == TotalRefined;
			}

			return false;
		}

		public string ToStringUnitless()
		{
			return Keys > 0 ? TotalKeys.ToString() : TotalRefined.ToString();
		}

		public static bool operator==(Price a, Price b)
		{
			return a.TotalRefined == b.TotalRefined;
		}
		public static bool operator!=(Price a, Price b)
		{
			return a.TotalRefined != b.TotalRefined;
		}

		public static bool operator>(Price a, Price b)
		{
			return a.TotalRefined > b.TotalRefined;
		}

		public static bool operator<(Price a, Price b)
		{
			return a.TotalRefined < b.TotalRefined;
		}

		public static bool operator>=(Price a, Price b)
		{
			return a.TotalRefined >= b.TotalRefined;
		}

		public static bool operator<=(Price a, Price b)
		{
			return a.TotalRefined <= b.TotalRefined;
		}

		public static Price operator+(Price a, Price b)
		{
			return new Price(0, a.TotalRefined + b.TotalRefined);
		}

		public static Price operator-(Price a, Price b)
		{
			return new Price(0, a.TotalRefined - b.TotalRefined);
		}

		public bool MatchesUnit(Price other)
		{
			return (other.Keys > 0) == (Keys > 0);
		}
	}
}
