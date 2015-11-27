using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Json.ItemDataJson;

namespace CustomSteamTools.Schema
{
	public class ItemAttribute
	{
		public const int UNUSUAL_ID = 134;
		public const int CRATESERIES_ID = 187;
		public const int KILLSTREAK_ID = 2025; // "Killstreak Tier"
		public const int SKINWEAR_ID = 725;
		public const int AUSTRALIUM_ID = 2027;

		public string Name
		{ get; set; }

		public string AttributeType
		{ get; set; }

		public int ID
		{ get; set; }

		public string FormatString
		{ get; set; }

		public AttributeEffectType EffectType
		{ get; set; }

		public ItemAttribute(AttributeJson json)
		{
			Name = json.name;
			AttributeType = json.attribute_class;
			ID = json.defindex;
			FormatString = json.description_string;
			EffectType = AttributeEffectTypes.Parse(json.effect_type);
		}

		public string GetDescription(string value)
		{
			return FormatString.Replace("%s1", value);
		}
		public string GetDescription(double value)
		{
			return GetDescription(value.ToString());
		}

		public override string ToString()
		{
			if (FormatString == null)
			{
				return "#" + ID + ": " + AttributeType;
			}

			return "#" + ID + ": " + GetDescription("X") + " [" + AttributeType + "]";
		}
	}
}
