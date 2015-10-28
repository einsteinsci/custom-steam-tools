using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Json.BackpackDataJson
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class TF2BackpackJson
	{
		[JsonProperty]
		public TF2BackpackResultJson result
		{ get; set; }
	}
}
