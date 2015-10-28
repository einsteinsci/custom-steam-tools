using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.ItemDataJson
{
	public static class AttributeDescriptionFormats
	{
		public const string PERCENT = "value_is_percentage";
		public const string PERCENT_INVERTED = "value_is_inverted_percentage";
		public const string ADDITIVE = "value_is_additive";
		public const string ADDITIVE_PERCENT = "value_is_additive_percentage";
		public const string DATE = "value_is_date";
		public const string PARTICLE = "value_is_particle_index";
		public const string STEAMID = "value_is_account_id";
		public const string OR = "value_is_or"; // ???
		public const string ITEM_DEF = "value_is_item_def"; // Item ID
	}
}
