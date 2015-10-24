using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class ClassifiedsScraper
	{
		public static bool SteamBackpackDown
		{ get; set; }

		public static List<ClassifiedsListing> GetClassifieds(Item item, Quality quality, bool verifySellers = true,
			bool craftable = true, bool tradable = true, bool australium = false)
		{
			string url = "http://backpack.tf/classifieds?item=";
			url += item.ImproperName;
			url += "&quality=" + ((int)quality).ToString();
			url += "&tradable=" + (tradable ? 1 : -1).ToString();
			url += "&craftable=" + (craftable ? 1 : -1).ToString();
			url += "&australium=" + (australium ? 1 : -1).ToString();
			url += "&killstreak_tier=0";
			
			Logger.Log("Downloading listings from " + url.SubstringMax(100) + "...", ConsoleColor.DarkGray);
			WebClient client = new WebClient();
			string html = client.DownloadString(url);
			//Logger.Log("  Download complete.", ConsoleColor.DarkGray);

			Logger.Log("  Scraping listings from HTML...", ConsoleColor.DarkGray);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			List<ClassifiedsListing> results = new List<ClassifiedsListing>();

			HtmlNode root = doc.DocumentNode;

			#region sells
			HtmlNode sellOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") && 
				n.Attributes["class"].Value == "media-list").FirstOrDefault();

			if (sellOrderRoot == null)
			{
				Logger.Log("  No sell orders found.", ConsoleColor.DarkGray);
			}
			else
			{
				List<HtmlNode> sellOrderBases = sellOrderRoot.Descendants("li").ToList();

				foreach (HtmlNode sob in sellOrderBases)
				{
					HtmlNode sellData = sob.Descendants("li").FirstOrDefault();

					if (sellData == null)
					{
						continue;
					}

					string sellItemName = sellData.Attributes["data-name"].Value; // not really necessary
					string sellTradable = sellData.Attributes["data-tradable"]?.Value; // or this
					string sellCraftable = sellData.Attributes["data-craftable"]?.Value; // or this
					string sellQuality = sellData.Attributes["data-quality"].Value; // or even this
					string sellComment = sellData.Attributes["data-listing-comment"]?.Value;
					string sellPrice = sellData.Attributes["data-listing-price"].Value;
					string sellLevel = sellData.Attributes["data-level"].Value;
					string sellID = sellData.Attributes["data-id"].Value;
					string sellerSteamID64 = sellData.Attributes["data-listing-steamid"].Value;
					string sellerNickname = sellData.Attributes["data-listing-name"]?.Value;
					string sellOfferUrl = sellData.Attributes["data-listing-offers-url"]?.Value;
					string sellOriginalID = sellData.Attributes["data-original-id"]?.Value;
					string sellCustomName = sellData.Attributes["data-custom-name"]?.Value;
					string sellCustomDesc = sellData.Attributes["data-custom-desc"]?.Value;

					ulong id = ulong.Parse(sellID); // really funky syntax down here -v
					ulong? originalID = sellOriginalID != null ? new ulong?(ulong.Parse(sellOriginalID)) : null;
					Price price = Price.ParseFancy(sellPrice);
					int level = int.Parse(sellLevel);

					ItemInstance instance = new ItemInstance(item, id, level, quality, craftable,
						sellCustomName, sellCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, sellerSteamID64, 
						sellerNickname, sellOfferUrl, sellComment, OrderType.Sell);

					if (string.IsNullOrWhiteSpace(listing.OfferURL))
					{
						continue;
					}

					if (verifySellers)
					{
						if (!UserHasItem(listing.ListerSteamID64, instance))
						{
							Logger.Log("  Dead listing. Skipping.", ConsoleColor.DarkGray);
						}
					}

					results.Add(listing);
				}
				Logger.Log("  Sell order scrape complete.", ConsoleColor.DarkGray);
			}
			#endregion sells

			#region buys
			HtmlNode buyOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") &&
				n.Attributes["class"].Value == "media-list").LastOrDefault();

			if (buyOrderRoot == null)
			{
				Logger.Log("  No buy orders found.", ConsoleColor.DarkGray);
			}
			else
			{
				List<HtmlNode> buyOrderBases = buyOrderRoot.Descendants("li").ToList();

				foreach (HtmlNode bob in buyOrderBases)
				{
					HtmlNode buyData = bob.Descendants("li").FirstOrDefault();

					if (buyData == null)
					{
						continue;
					}

					string buyItemName = buyData.Attributes["data-name"].Value; // not really necessary
					string buyTradable = buyData.Attributes["data-tradable"]?.Value; // or this
					string buyCraftable = buyData.Attributes["data-craftable"]?.Value; // or this
					string buyQuality = buyData.Attributes["data-quality"].Value; // or even this
					string buyComment = buyData.Attributes["data-listing-comment"]?.Value;
					string buyPrice = buyData.Attributes["data-listing-price"].Value;
					string buyLevel = buyData.Attributes["data-level"]?.Value ?? "0";
					string buyID = buyData.Attributes["data-id"]?.Value ?? "0";
					string buyerSteamID64 = buyData.Attributes["data-listing-steamid"].Value;
					string buyerSteamNickname = buyData.Attributes["data-listing-name"]?.Value;
					string buyOfferUrl = buyData.Attributes["data-listing-offers-url"]?.Value;
					string buyOriginalID = buyData.Attributes["data-original-id"]?.Value;
					string buyCustomName = buyData.Attributes["data-custom-name"]?.Value;
					string buyCustomDesc = buyData.Attributes["data-custom-desc"]?.Value;

					ulong id = ulong.Parse(buyID == "" ? "0" : buyID); // really funky syntax down here -v
					ulong? originalID = buyOriginalID != null ? new ulong?(ulong.Parse(buyOriginalID == "" ? "0" : buyOriginalID)) : null;
					Price price = Price.ParseFancy(buyPrice);
					int level = int.Parse(buyLevel);

					ItemInstance instance = new ItemInstance(item, id, level, quality, craftable,
						buyCustomName, buyCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, buyerSteamID64, buyerSteamNickname, 
						buyOfferUrl, buyComment, OrderType.Buy);

					if (string.IsNullOrWhiteSpace(listing.OfferURL))
					{
						continue;
					}

					results.Add(listing);
				}

				//Logger.Log("  Buy order scrape complete.", ConsoleColor.DarkGray);
			}
			#endregion buys

			return results;
		}

		public static bool UserHasItem(string steamID, ItemInstance inst)
		{
			if (SteamBackpackDown)
			{
				Logger.Log("  Steam API is down; skipping verify.", ConsoleColor.DarkGray);
				return true;
			}

			TF2BackpackData bp = null;
			if (steamID == DataManager.SEALEDINTERFACE_STEAMID)
			{
				bp = DataManager.MyBackpackData;
			}
			else
			{
				if (!DataManager.BackpackData.ContainsKey(steamID))
				{
					if (!DataManager.LoadOtherBackpack(steamID))
					{
						Logger.Log("  Steam API is down. Disabling verify until refresh command.", ConsoleColor.Red);
						SteamBackpackDown = true;
						return true;
					}
				}

				bp = DataManager.BackpackData[steamID];
			}
			
			foreach (ItemInstance i in bp.Items)
			{
				// now check a bunch of stuff. If anything fails, off to next item.

				if (!i.Tradable)
					continue;

				if (i.Item != inst.Item)
					continue;

				if (i.Quality != inst.Quality)
					continue;

				if (i.Craftable != inst.Craftable)
					continue;

				if (i.Level != inst.Level)
					continue;

				if (i.GetKillstreak() != inst.GetKillstreak())
					continue;

				if (i.GetUnusual() != inst.GetUnusual())
					continue;

				if (i.GetCrateSeries() != inst.GetCrateSeries())
					continue;

				if ((i.CustomName == null) != (inst.CustomName == null))
					continue;

				if (i.CustomName != null && inst.CustomName != null && 
					i.CustomName.ToLower() != inst.CustomName.ToLower())
					continue;

				Logger.Log("  Item verified!", ConsoleColor.DarkGray);
				return true;
			}

			return false;
		}
	}
}
