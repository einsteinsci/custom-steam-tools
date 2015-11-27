using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;

namespace CustomSteamTools
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

		public static Item RefinedMetal => DataManager.Schema.GetItem(REF_DEFINDEX);
		public static Item ReclaimedMetal => DataManager.Schema.GetItem(REC_DEFINDEX);
		public static Item ScrapMetal => DataManager.Schema.GetItem(SCRAP_DEFINDEX);

		public static double RefinedPerKey
		{
			get
			{
				if (double.IsNaN(_refinedPerKey))
				{
					DataManager.AutoSetup(false);
				}

				return _refinedPerKey;
			}
			set
			{
				_refinedPerKey = value;
			}
		}
		private static double _refinedPerKey = double.NaN;

		// this is what is actually stored
		public double TotalRefined
		{ get; private set; }

		public int Keys => (int)TotalKeys;
		public double Refined => TotalRefined - (Keys * RefinedPerKey);
		public double TotalKeys => TotalRefined / RefinedPerKey;
		public double TotalUSD => TotalKeys * KEY_STOREPRICE_USD;

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
			return Math.Abs(Keys) > 2.0 ? (TotalKeys.ToString("F2") + " keys") : (TotalRefined.ToString("F2") + " ref");
		}

		public PriceRange ToPriceRange()
		{
			return new PriceRange(this, this);
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
		public static Price Parse(string input)
		{
			string s = input.ToLower();

			bool isKey = s.EndsWith("k");

			s = s.TrimEnd('k');

			double d = -1;
			if (!double.TryParse(s, out d))
			{
				throw new FormatException("Argument invalid: " + input);
			}

			return new Price(d, isKey ? Price.CURRENCY_KEYS : Price.CURRENCY_REF);
		}
		public static bool TryParse(string input, out Price result)
		{
			bool succeeded = false;
			try
			{
				result = Parse(input);
				succeeded = true;
			}
			catch (FormatException)
			{
				result = Zero;
			}

			return succeeded;
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

		#region operator overloading
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

		public static Price operator*(Price a, double b)
		{
			return new Price(0, a.TotalRefined * b);
		}

		public static Price operator/(Price a, double b)
		{
			return new Price(0, a.TotalRefined / b);
		}
		#endregion

		[Obsolete("Don't use this. Not entirely functioning correctly")]
		public bool MatchesUnit(Price other)
		{
			return (other.Keys > 0) == (Keys > 0);
		}
	}
}
