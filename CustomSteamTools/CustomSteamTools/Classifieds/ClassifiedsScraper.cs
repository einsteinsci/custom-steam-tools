﻿using System;
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
			string url = "http://backpack.tf/classifieds?item=";
			url += item.ImproperName;
			url += "&quality=" + ((int)quality).ToString();
			url += "&tradable=" + (tradable ? 1 : -1).ToString();
			url += "&craftable=" + (craftable ? 1 : -1).ToString();
			url += "&australium=" + (australium ? 1 : -1).ToString();
			url += "&killstreak_tier=0";
			
			VersatileIO.Verbose("Downloading listings from {0}...", url.Shorten(100, ""));
			WebClient client = new WebClient();
			string html = client.DownloadString(url);
			//Logger.Log("  Download complete.", ConsoleColor.DarkGray);

			VersatileIO.Verbose("  Scraping listings from HTML...");
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			List<ClassifiedsListing> results = new List<ClassifiedsListing>();

			HtmlNode root = doc.DocumentNode;

			#region sells
			HtmlNode sellOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") && 
				n.Attributes["class"].Value == "media-list").FirstOrDefault();

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

					results.Add(listing);
				}
				VersatileIO.Verbose("  Sell order scrape complete.");
			}
			#endregion sells

			#region buys
			HtmlNode buyOrderRoot = root.Descendants("ul").Where((n) => n.Attributes.Contains("class") &&
				n.Attributes["class"].Value == "media-list").LastOrDefault();

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

				VersatileIO.Verbose("  Buy order scrape complete.");
			}
			#endregion buys

			return results;
		}
	}
}
