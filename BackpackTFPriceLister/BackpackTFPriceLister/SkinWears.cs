using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public enum SkinWear
	{
		FactoryNew = 0,
		MinimalWear,
		FieldTested,
		WellWorn,
		BattleScarred
	}

	public static class SkinWears
	{
		public static string ToReadableString(this SkinWear skin)
		{
			switch (skin)
			{
				case SkinWear.FactoryNew:
					return "Factory New";
				case SkinWear.MinimalWear:
					return "Minimal Wear";
				case SkinWear.FieldTested:
					return "Field-Tested";
				case SkinWear.WellWorn:
					return "Well-Worn";
				case SkinWear.BattleScarred:
					return "Battle Scarred";
				default:
					return "WEAR.ERR";
			}
		}

		public static string WithParentheses(this SkinWear skin)
		{
			return "(" + skin.ToReadableString() + ")";
		}

		public static SkinWear? ParseNullable(string s)
		{
			for (int i = 0; i < 5; i++)
			{
				SkinWear w = (SkinWear)i;
				if (w.ToString().ToLower() == s.ToLower() ||
					w.ToReadableString().ToLower() == s.ToLower())
				{
					return w;
				}
			}

			return null;
		}

		public static SkinWear Parse(string s)
		{
			return ParseNullable(s) ?? SkinWear.FactoryNew;
		}
	}
}
