using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools
{
	public struct PriceRange : IEquatable<PriceRange>
	{
		public static PriceRange Zero => new PriceRange(Price.Zero);

		public Price Low
		{ get; private set; }

		public Price High
		{ get; private set; }

		public Price Mid => new Price((High.TotalRefined + Low.TotalRefined) / 2.0);

		public bool IsOnePrice => High == Low;

		public PriceRange(Price low, Price high)
		{
			Low = low;
			High = high;
		}

		public PriceRange(Price both) : this(both, both)
		{ }

		public PriceRange(double lowRef, double highRef) : 
			this(Price.FromMetal(lowRef), Price.FromMetal(highRef))
		{ }

		public PriceRange SetLow(Price low)
		{
			return new PriceRange(low, High);
		}
		public PriceRange SetHigh(Price high)
		{
			return new PriceRange(Low, high);
		}

		public bool Contains(Price price)
		{
			return Low <= price && High >= price;
		}
		public bool Contains(PriceRange range)
		{
			return Low <= range.Low && High >= range.High;
		}

		public bool ContainsExclusive(Price price)
		{
			return Low < price && High > price;
		}
		public bool ContainsExclusive(PriceRange range)
		{
			return Low < range.Low && High > range.High;
		}

		public bool Equals(PriceRange other)
		{
			return other.Low == Low && other.High == High;
		}

		public override bool Equals(object obj)
		{
			if (obj is PriceRange)
			{
				return Equals((PriceRange)obj);
			}

			return false;
		}
		public override int GetHashCode()
		{
			return Low.GetHashCode() + High.GetHashCode();
		}

		public override string ToString()
		{
			if (IsOnePrice)
			{
				return Low.ToString();
			}

			return Low + " - " + High;
		}

		public string ToStringUSD()
		{
			if (IsOnePrice)
			{
				return Low.TotalUSD.ToCurrency();
			}

			return Low.TotalUSD.ToCurrency() + " - " + High.TotalUSD.ToCurrency();
		}

		#region operator overloads

		public static bool operator==(PriceRange a, PriceRange b)
		{
			return a.Equals(b);
		}
		public static bool operator!=(PriceRange a, PriceRange b)
		{
			return !a.Equals(b);
		}

		public static PriceRange operator+(PriceRange a, PriceRange b)
		{
			return new PriceRange(a.Low + b.Low, a.High + b.High);
		}
		public static PriceRange operator-(PriceRange a, PriceRange b)
		{
			return new PriceRange(a.Low - b.Low, a.High - b.High);
		}

		public static PriceRange operator*(PriceRange a, double b)
		{
			return new PriceRange(a.Low * b, a.High * b);
		}
		public static PriceRange operator/(PriceRange a, double b)
		{
			return new PriceRange(a.Low / b, a.High / b);
		}

		#endregion operator overloads
	}
}
