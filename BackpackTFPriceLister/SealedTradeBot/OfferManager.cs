using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using static SteamKit2.WebAPI;
using SteamWebAPI;
using static SteamWebAPI.SteamAPISession;
using CustomSteamTools;
using System.Net;

namespace SealedTradeBot
{
	public static class OfferManager
	{
		public const string BOT_API_KEY = "6EC366E3D20B931D81378B625575CFF3";
		public const string BOT_STEAMID = "";

		public const string SEALED_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";
		public const string SEALED_STEAMID = "76561198111510726";

		public static List<TradeOffer> SentOffers
		{ get; private set; }
		public static List<TradeOffer> ReceivedOffers
		{ get; private set; }
		public static List<AssetDescription> DescriptionCache
		{ get; private set; }

		public static void LoadOffers()
		{
			SentOffers = new List<TradeOffer>();
			ReceivedOffers = new List<TradeOffer>();
			DescriptionCache = new List<AssetDescription>();

			BotLogger.LogDebug("Connecting to Steam...");
			using (Interface econService = GetInterface("IEconService", SEALED_API_KEY))
			{
				Dictionary<string, string> tradeOffersArgs = new Dictionary<string, string>();
				tradeOffersArgs["get_sent_offers"] = "1";
				tradeOffersArgs["get_received_offers"] = "1";
				tradeOffersArgs["get_descriptions"] = "1";
				tradeOffersArgs["language"] = "en_US";

				BotLogger.LogDebug("Downloading Trade Offer data...");

				KeyValue response = null;
				try
				{
					response = econService.Call("GetTradeOffers", 1, tradeOffersArgs);
				}
				catch (WebException e)
				{
					BotLogger.LogErr("Could not retrieve GetTradeOffers API: " + e.Message);
					return;
				}

				BotLogger.LogDebug("Parsing Data...");

				KeyValue offersKV = response["trade_offers_sent"];
				if (offersKV != null)
				{
					foreach (KeyValue kv in offersKV.Children)
					{
						SentOffers.Add(new TradeOffer(kv));
					}
				}

				offersKV = response["trade_offers_received"];
				if (offersKV != null)
				{
					foreach (KeyValue kv in offersKV.Children)
					{
						SentOffers.Add(new TradeOffer(kv));
					}
				}

				offersKV = response["descriptions"];
				if (offersKV != null)
				{
					foreach (KeyValue kv in offersKV.Children)
					{
						DescriptionCache.Add(new AssetDescription(kv));
					}
				}

				BotLogger.LogDebug("Parse Complete.");
				BotLogger.LogLine();

			}
		}

		public static List<TradeOffer> GetAllOpenOffers()
		{
			if (SentOffers == null || ReceivedOffers == null || DescriptionCache == null)
			{
				LoadOffers();
			}

			List<TradeOffer> results = new List<TradeOffer>();
			foreach (TradeOffer to in SentOffers)
			{
				if (to.State == TradeOfferState.Active)
				{
					results.Add(to);
				}
			}
			foreach (TradeOffer to in ReceivedOffers)
			{
				if (to.State == TradeOfferState.Active)
				{
					results.Add(to);
				}
			}

			return results;
		}
	}
}
