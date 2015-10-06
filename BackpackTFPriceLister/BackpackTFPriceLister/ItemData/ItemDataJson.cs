﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.ItemData
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class ItemDataJson
	{
		[JsonProperty]
		public ItemDataResultJson result
		{ get; set; }
	}
}
