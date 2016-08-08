using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.PriceDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TypeIndexPricingJson
	{
		public string currency
		{ get; set; }

		public double value
		{ get; set; }
		
		public double value_high
		{ get; set; }

		public long last_update
		{ get; set; }

		public double difference
		{ get; set; }

		public override string ToString()
		{
			return $"Currency: {currency}, Value: {value}, Value (High): {value_high}, " + 
				$"Last Update: {last_update}, Difference: {difference}";
		}
	}
}
