using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackpackTFPriceLister;
using SteamKit2;

namespace SealedTradeBot
{
	public class AssetDescription
	{
		public int AppID
		{ get; private set; }

		public long ClassID
		{ get; private set; }

		public long InstanceID
		{ get; private set; }

		public bool Currency
		{ get; private set; }

		public string BackgroundColor
		{ get; private set; }

		public string IconUrl
		{ get; private set; }

		public string IconUrlLarge
		{ get; private set; }

		public List<DescriptionLine> Descriptions
		{ get; private set; }

		public bool Tradable
		{ get; private set; }

		public List<ItemAction> Actions
		{ get; private set; }

		public string Name
		{ get; private set; }

		public string NameColor
		{ get; private set; }

		public string TypeLine
		{ get; private set; }

		public string MarketName
		{ get; private set; }

		public string MarketHashName // what's the difference?
		{ get; private set; }

		public List<ItemAction> MarketActions
		{ get; private set; }

		public bool Commodity
		{ get; private set; }

		public int MarketTradableRestriction
		{ get; private set; }

		public int MarketMarketableRestriction // no idea what this means
		{ get; private set; }

		public AssetDescription(KeyValue kv)
		{
			AppID = int.Parse(kv["appid"].Value);
			ClassID = long.Parse(kv["classid"].Value);
			InstanceID = long.Parse(kv["instanceid"].Value);
			Currency = Util.ParseWebBool(kv["currency"].Value);
			BackgroundColor = kv["background_color"].Value;
			IconUrl = kv["icon_url"].Value;
			IconUrlLarge = kv["icon_url_large"].Value;

			KeyValue descs = kv["descriptions"];
			Descriptions = new List<DescriptionLine>();
			foreach (KeyValue d in descs.Children)
			{
				string type = d["type"].Value;
				string value = d["value"].Value;
				string color = d["color"].Value;

				Descriptions.Add(new DescriptionLine(type, value, color));
			}

			Tradable = Util.ParseWebBool(kv["tradable"].Value);

			KeyValue actions = kv["actions"];
			Actions = new List<ItemAction>();
			foreach (KeyValue a in actions.Children)
			{
				string link = a["link"].Value;
				string name = a["name"].Value;

				Actions.Add(new ItemAction(link, name));
			}

			Name = kv["name"].Value;
			NameColor = kv["name_color"].Value;
			TypeLine = kv["type"].Value;
			MarketName = kv["market_name"].Value;
			MarketHashName = kv["market_hash_name"].Value;
			Commodity = Util.ParseWebBool(kv["commodity"].Value);
			MarketTradableRestriction = int.Parse(kv["market_tradable_restriction"].Value);
			MarketMarketableRestriction = int.Parse(kv["market_marketable_restriction"].Value);

			actions = kv["market_actions"];
			MarketActions = new List<ItemAction>();
			foreach (KeyValue a in actions.Children)
			{
				string link = a["link"].Value;
				string name = a["name"].Value;

				MarketActions.Add(new ItemAction(link, name));
			}
		}

		public Item GetItem(TF2Data data)
		{
			ItemAction action = Actions.FirstOrDefault((a) => a.Name == "Item Wiki Page...");
			if (action == null)
			{
				return null;
			}

			int index = action.Link.IndexOf("id=") + 3;
			int endIndex = action.Link.IndexOf("&", index);
			string substr = "";
			if (endIndex == -1)
			{
				substr = action.Link.Substring(index);
			}
			else
			{
				substr = action.Link.Substring(index, endIndex - index);
			}

			long id = long.Parse(substr);
			return data.GetItem(id);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class DescriptionLine
	{
		public string Type
		{ get; private set; }

		public string Value
		{ get; private set; }

		public string Color
		{ get; private set; }

		public DescriptionLine(string type, string value, string color)
		{
			Type = type;
			Value = value;
			Color = color;
		}
	}

	public class ItemAction
	{
		public string Link
		{ get; private set; }

		public string Name
		{ get; private set; }

		public ItemAction(string link, string name)
		{
			Link = link;
			Name = name;
		}
	}
}
