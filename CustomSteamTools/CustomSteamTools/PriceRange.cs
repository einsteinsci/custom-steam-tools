using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools
{
	public struct PriceRange
	{
		public Price Low
		{ get; private set; }

		public Price High
		{ get; private set; }

		public PriceRange(Price low, Price high)
		{
			Low = low;
			High = high;
		}

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

		public override bool Equals(object obj)
		{
			if (obj is PriceRange)
			{
				PriceRange other = (PriceRange)obj;
				return other.Low == Low && other.High == High;
			}

			return false;
		}
		public override int GetHashCode()
		{
			return Low.GetHashCode() + High.GetHashCode();
		}

		public override string ToString()
		{
			if (Low == High)
			{
				return Low.ToString();
			}

			return Low.ToString() + " - " + High.ToString();
		}
	}
}
