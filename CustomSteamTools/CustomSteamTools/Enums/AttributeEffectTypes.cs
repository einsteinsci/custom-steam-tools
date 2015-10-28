using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;

namespace CustomSteamTools
{
	public enum AttributeEffectType
	{
		Negative = -1,
		Neutral = 0,
		Positive = 1
	}

	public static class AttributeEffectTypes
	{
		public const string POSITIVE = "positive";
		public const string NEUTRAL = "neutral";
		public const string NEGATIVE = "negative";

		public static AttributeEffectType Parse(string aet)
		{
			if (aet == POSITIVE)
			{
				return AttributeEffectType.Positive;
			}
			if (aet == NEUTRAL)
			{
				return AttributeEffectType.Neutral;
			}

			return AttributeEffectType.Negative;
		}

		public static string GetEffectType(this AttributeEffectType t)
		{
			switch (t)
			{
				case AttributeEffectType.Negative:
					return NEGATIVE;
				case AttributeEffectType.Neutral:
					return NEUTRAL;
				case AttributeEffectType.Positive:
					return POSITIVE;
				default:
					return "ERR";
			}
		}
	}
}
