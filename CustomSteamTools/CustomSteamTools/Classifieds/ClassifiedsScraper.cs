using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;

using UltimateUtil;

using HtmlAgilityPack;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Backpacks;
using CustomSteamTools.Market;

namespace CustomSteamTools.Classifieds
{
	public static class ClassifiedsScraper
	{
		public static bool SteamBackpackDown
		{ get; set; }

		public static List<ClassifiedsListing> GetClassifieds(Item item, ListingProperties props)
		{
			return GetClassifieds(item, props.Quality, props.Craftable, props.Tradable, props.Australium);
		}

		public static List<ClassifiedsListing> GetClassifieds(Item item, Quality quality,
			bool craftable = true, bool tradable = true, bool australium = false)
		{
			string url = MakeBpTfClassifiedsUrl(item, quality, craftable, tradable, australium);

			VersatileIO.Verbose("Downloading listings from {0}...", url.Shorten(100, ""));
			WebClient client = new WebClient { Encoding = Encoding.UTF8 };
			string html = client.DownloadString(url);
			//Logger.Log("  Download complete.", ConsoleColor.DarkGray);

			VersatileIO.Verbose("  Scraping listings from HTML...");
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			List<ClassifiedsListing> results = new List<ClassifiedsListing>();

			HtmlNode root = doc.DocumentNode;

			#region sells
			HtmlNode sellOrderRoot = root.Descendants("ul").FirstOrDefault(n => n.Attributes.Contains("class") &&
				n.Attributes["class"].Value == "media-list");

			if (sellOrderRoot == null)
			{
				VersatileIO.Verbose("  No sell orders found.");
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

					Item foundItem = item;

					string sellItemName = sellData.Attributes["data-name"].Value; // not really necessary
					string sellTradable = sellData.Attributes["data-tradable"]?.Value; // or this
					string sellCraftable = sellData.Attributes["data-craftable"]?.Value; // or this
					string sellQuality = sellData.Attributes["data-quality"].Value; // or even this
					string sellComment = sellData.Attributes["data-listing_comment"]?.Value;
					string sellPrice = sellData.Attributes["data-listing_price"].Value;
					string sellLevel = sellData.Attributes["data-level"]?.Value ?? "-1";
					string sellID = sellData.Attributes["data-id"].Value;
					string sellerSteamID64 = sellData.Attributes["data-listing_account_id"].Value;
					string sellerNickname = sellData.Attributes["data-listing_name"]?.Value;
					string sellOfferUrl = sellData.Attributes["data-listing_offers_url"]?.Value;
					string sellOriginalID = sellData.Attributes["data-original_id"]?.Value;
					string sellCustomName = sellData.Attributes["data-custom_name"]?.Value;
					string sellCustomDesc = sellData.Attributes["data-custom_desc"]?.Value;
					string sellBuyoutOnly = sellData.Attributes["data-listing_buyout"]?.Value ?? "0";
					string sellMarketName = sellData.Attributes["data-market-name"]?.Value;

					if (sellMarketName != null)
					{
						MarketPricing mp = DataManager.MarketPrices.GetPricing(sellMarketName);
						if (mp?.GunMettleSkin != null)
						{
							foundItem = mp.GunMettleSkin.GetItemForm(DataManager.Schema);
						}
					}

					ulong id = ulong.Parse(sellID); // really funky syntax down here -v
					ulong? originalID = sellOriginalID != null ? new ulong?(ulong.Parse(sellOriginalID)) : null;
					Price price = Price.ParseFancy(sellPrice);
					int level = int.Parse(sellLevel);
					bool buyoutOnly = BooleanUtil.ParseLoose(sellBuyoutOnly);

					ItemInstance instance = new ItemInstance(foundItem, id, level, quality, craftable,
						sellCustomName, sellCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, sellerSteamID64,
						sellerNickname, sellOfferUrl, sellComment, buyoutOnly, OrderType.Sell);

					if (string.IsNullOrWhiteSpace(listing.OfferURL))
					{
						continue;
					}

					results.Add(listing);
				}
				VersatileIO.Verbose("  Sell order scrape complete.");
			}
			#endregion sells

			#region buys
			HtmlNode buyOrderRoot = root.Descendants("ul").LastOrDefault(n => n.Attributes.Contains("class") &&
				n.Attributes["class"].Value == "media-list");

			if (buyOrderRoot == null)
			{
				VersatileIO.Verbose("  No buy orders found.");
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

					Item foundItem = item;

					string buyItemName = buyData.Attributes["data-name"].Value; // not really necessary
					string buyTradable = buyData.Attributes["data-tradable"]?.Value; // or this
					string buyCraftable = buyData.Attributes["data-craftable"]?.Value; // or this
					string buyQuality = buyData.Attributes["data-quality"].Value; // or even this
					string buyComment = buyData.Attributes["data-listing_comment"]?.Value;
					string buyPrice = buyData.Attributes["data-listing_price"].Value;
					string buyLevel = buyData.Attributes["data-level"]?.Value ?? "-1";
					string buyID = buyData.Attributes["data-id"]?.Value ?? "0";
					string buyerSteamID64 = buyData.Attributes["data-listing_account_id"].Value;
					string buyerSteamNickname = buyData.Attributes["data-listing_name"]?.Value;
					string buyOfferUrl = buyData.Attributes["data-listing_offers_url"]?.Value;
					string buyOriginalID = buyData.Attributes["data-original_id"]?.Value;
					string buyCustomName = buyData.Attributes["data-custom_name"]?.Value;
					string buyCustomDesc = buyData.Attributes["data-custom_desc"]?.Value;
					string buyBuyoutOnly = buyData.Attributes["data-listing_buyout"]?.Value ?? "0";
					string buyMarketName = buyData.Attributes["data-market-name"]?.Value;

					if (buyMarketName != null)
					{
						MarketPricing mp = DataManager.MarketPrices.GetPricing(buyMarketName);
						if (mp != null && mp.GunMettleSkin != null)
						{
							foundItem = mp.GunMettleSkin.GetItemForm(DataManager.Schema);
						}
					}

					ulong id = ulong.Parse(buyID == "" ? "0" : buyID); // really funky syntax down here -v
					ulong? originalID = buyOriginalID != null ? new ulong?(ulong.Parse(buyOriginalID == "" ? "0" : buyOriginalID)) : null;
					Price price = Price.ParseFancy(buyPrice);
					int level = int.Parse(buyLevel);
					bool buyoutOnly = BooleanUtil.ParseLoose(buyBuyoutOnly);

					ItemInstance instance = new ItemInstance(foundItem, id, level, quality, craftable,
						buyCustomName, buyCustomDesc, originalID, tradable);
					ClassifiedsListing listing = new ClassifiedsListing(instance, price, buyerSteamID64, buyerSteamNickname,
						buyOfferUrl, buyComment, buyoutOnly, OrderType.Buy);

					if (string.IsNullOrWhiteSpace(listing.OfferURL))
					{
						continue;
					}

					results.Add(listing);
				}

				VersatileIO.Verbose("  Buy order scrape complete.");
			}
			#endregion buys

			return results;
		}

		public static string MakeBpTfClassifiedsUrl(Item item, Quality quality, bool craftable, bool tradable, bool australium)
		{
			string url = "https://backpack.tf/classifieds?item=";
			url += item.ImproperName;
			url += "&quality=" + (int)quality;
			url += "&tradable=" + (tradable ? 1 : -1);
			url += "&craftable=" + (craftable ? 1 : -1);
			url += "&australium=" + (australium ? 1 : -1);
			url += "&killstreak_tier=0";

			return url;
		}
	}
}
