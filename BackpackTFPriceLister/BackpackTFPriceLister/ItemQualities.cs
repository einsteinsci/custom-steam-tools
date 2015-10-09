using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public enum Quality
	{
		Stock = 0,
		Genuine = 1,
		Vintage = 3,
		Unusual = 5,
		Unique = 6,
		Community = 7,
		Valve = 8,
		SelfMade = 9,
		Strange = 11,
		Haunted = 13,
		Collectors = 14,
	}

	public static class ItemQualities
	{
		public const int NORMAL = 0; // stock
		public const int GENUINE = 1;
		public const int VINTAGE = 3;
		public const int UNUSUAL = 5;
		public const int UNIQUE = 6;
		public const int COMMUNITY = 7;
		public const int VALVE = 8;
		public const int SELFMADE = 9;
		public const int STRANGE = 11;
		public const int HAUNTED = 13;
		public const int COLLECTORS = 14;

		public static string ToReadableString(this Quality q)
		{
			if (q == Quality.Stock || q == Quality.Unique)
			{
				return "";
			}

			if (q == Quality.SelfMade)
			{
				return "Self-Made";
			}

			if (q == Quality.Collectors)
			{
				return "Collector's";
			}

			return q.ToString();
		}

		public static Quality Parse(string s)
		{
			for (int i = 0; i <= 14; i++)
			{
				Quality q = (Quality)i;
				if (q.ToReadableString().ToLower() == s || q.ToString().ToLower() == s)
				{
					return q;
				}
			}

			return Quality.Unique;
		}
	}
}
