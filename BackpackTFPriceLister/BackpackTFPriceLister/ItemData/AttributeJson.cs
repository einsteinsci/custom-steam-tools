using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class AttributeJson
	{
		public string name
		{ get; set; }

		public int defindex
		{ get; set; }

		public string attribute_class
		{ get; set; }

		public double minvalue
		{ get; set; }

		public double maxvalue
		{ get; set; }

		public string description_string
		{ get; set; }

		public string description_format
		{ get; set; }

		public string effect_type
		{ get; set; }

		public bool hidden
		{ get; set; }

		public bool stored_as_integer
		{ get; set; }
	}

	public static class AttributeEffectTypes
	{
		public const string POSITIVE = "positive";
		public const string NEUTRAL = "neutral";
		public const string NEGATIVE = "negative";
	}
}
