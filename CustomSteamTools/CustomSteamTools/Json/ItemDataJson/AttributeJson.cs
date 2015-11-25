using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.ItemDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class AttributeJson
	{
		// Localized name
		public string name
		{ get; set; }

		// Attribute ID
		public int defindex
		{ get; set; }

		// Unlocalized name
		public string attribute_class
		{ get; set; }

		public double minvalue
		{ get; set; }

		public double maxvalue
		{ get; set; }

		// Format string for description
		public string description_string
		{ get; set; }

		public string description_format
		{ get; set; }

		// Positive or negative (or neutral)
		public string effect_type
		{ get; set; }

		public bool hidden
		{ get; set; }

		public bool stored_as_integer
		{ get; set; }

		public override string ToString()
		{
			if (description_string == null)
			{
				return "#" + defindex + ": " + attribute_class;
			}

			string exampleDesc = description_string.Replace("%s1", "X");

			return "#" + defindex + ": " + exampleDesc + " [" + attribute_class + "]";
		}
	}
}
