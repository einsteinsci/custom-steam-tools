using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public struct Price : IEquatable<Price>
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

		public static Item RefinedMetal => DataManager.ItemData.GetItem(REF_DEFINDEX);
		public static Item ReclaimedMetal => DataManager.ItemData.GetItem(REC_DEFINDEX);
		public static Item ScrapMetal => DataManager.ItemData.GetItem(SCRAP_DEFINDEX);

		public static double RefinedPerKey
		{
			get
			{
				if (_refinedPerKey == -1)
				{
					DataManager.AutoSetup(true, false);
				}

				return _refinedPerKey;
			}
			set
			{
				_refinedPerKey = value;
			}
		}
		private static double _refinedPerKey = -1;

		// this is what is actually stored
		public double TotalRefined
		{ get; private set; }

		public int Keys => (int)TotalKeys;
		public double Refined => TotalRefined - (Keys * RefinedPerKey);
		public double TotalKeys => TotalRefined / RefinedPerKey;

		public Price(int keys, double refined) : this(keys * RefinedPerKey + refined)
		{ }

		public Price(double refined)
		{
			TotalRefined = refined;
		}

		public Price(double amount, string currency)
		{
			switch (currency)
			{
				case CURRENCY_CASH:
					double keys = amount / KEY_STOREPRICE_USD;
					TotalRefined = keys * RefinedPerKey;
					break;
				case CURRENCY_KEYS:
					TotalRefined = amount * RefinedPerKey;
					break;
				case CURRENCY_REF:
					TotalRefined = amount;
					break;
				default:
					throw new ArgumentException("Invalid currency: " + currency, nameof(currency));
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Price)
			{
				return Equals((Price)obj);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return TotalRefined.GetHashCode();
		}

		public override string ToString()
		{
			return Keys > 2.0 ? (TotalKeys.ToString("F2") + " keys") : (TotalRefined.ToString() + " ref");
		}

		public bool Equals(Price other)
		{
			return TotalRefined == other.TotalRefined;
		}

		// for scraping backpack.tf classifieds
		public static Price ParseFancy(string s)
		{
			string buf = s.Replace("keys", "k").Replace("ref", "").Replace(" ", "").Replace("key", "k");
			string[] split = buf.Split(',');

			string sKeysWithK = null, sRef = null;
			if (split.Length > 1)
			{
				sKeysWithK = split[0];
				sRef = split[1];
			}
			else
			{
				if (split[0].EndsWith("k"))
				{
					sKeysWithK = split[0];
					sRef = "0";
				}
				else
				{
					sKeysWithK = "0k";
					sRef = split[0];
				}
			}

			string sKeys = sKeysWithK.TrimEnd('k');

			double keys = double.Parse(sKeys);
			double refined = double.Parse(sRef);

			return new Price(keys * RefinedPerKey + refined);
		}

		public static Price FromKeys(double keys)
		{
			return new Price(keys * RefinedPerKey);
		}
		public static Price FromMetal(double refined)
		{
			return new Price(refined);
		}
		public static Price FromUSD(double usd)
		{
			return new Price(usd, CURRENCY_CASH);
		}

		public string ToStringUnitless()
		{
			return Keys > 0 ? TotalKeys.ToString() : TotalRefined.ToString();
		}

		public static bool operator==(Price a, Price b)
		{
			return a.Equals(b);
		}
		public static bool operator!=(Price a, Price b)
		{
			return !a.Equals(b);
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
