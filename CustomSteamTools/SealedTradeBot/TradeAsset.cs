using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SealedTradeBot
{
	public class TradeAsset
	{
		public const int TF2_APPID = 440;

		public int AppID
		{ get; set; }

		public byte ContextID
		{ get; set; }

		public long AssetID
		{ get; set; }

		public long ClassID
		{ get; set; }

		public long InstanceID
		{ get; set; }

		public int Amount
		{ get; set; }

		public bool Missing
		{ get; set; }

		public TradeAsset()
		{ }

		public TradeAsset(KeyValue kv)
		{
			AppID = int.Parse(kv["appid"].Value);
			ContextID = byte.Parse(kv["contextid"].Value);
			AssetID = long.Parse(kv["assetid"].Value);
			ClassID = long.Parse(kv["classid"].Value);
			InstanceID = long.Parse(kv["instanceid"].Value);
			Amount = int.Parse(kv["amount"].Value);
			Missing = Util.ParseWebBool(kv["missing"].Value);
		}
	}
}
