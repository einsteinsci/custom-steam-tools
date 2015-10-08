﻿using BackpackTFPriceLister.ItemDataJson;
using BackpackTFPriceLister.PriceDataJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	// the "main" class in this DLL
	public static class PriceLister
    {
		public const string BPTF_API_KEY = "5612f911ba8d880424a41d01";
		public const string STEAM_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";
		public const string TF2_APP_ID = "440";

		public static string BpTfUrl
		{ get; set; }
		public static string SteamUrl
		{ get; set; }
		public static string BpTfFilename
		{ get; set; }
		public static string ItemDataFilename
		{ get; set; }
		public static string CacheLocation
		{ get; set; }

		public static string BpTfCache
		{ get; private set; }
		public static string ItemCache
		{ get; private set; }

		public static TF2DataJson ItemDataRaw
		{ get; private set; }
		public static TF2Data ItemData
		{ get; private set; }

		public static BpTfPriceDataJson PriceDataRaw
		{ get; private set; }
		public static BpTfPriceData PriceData
		{ get; private set; }

		public static void Initialize(bool fancyJson)
		{
			string _cachelocation = Environment.GetEnvironmentVariable("TEMP") + "\\BACKPACK.TF-PRICELIST\\";
			string _bptffilename = "bptf-pricedata.json";
			string _itemfilename = "tf2-itemdata.json";
			Initialize(_cachelocation, _bptffilename, _itemfilename, fancyJson);
		}
		
		public static void Initialize(string cacheLocation, string bptfFilename, string itemFilename, bool fancyJson)
		{
			BpTfUrl = "http://backpack.tf/api/IGetPrices/v4/?key=" + BPTF_API_KEY;

			if (fancyJson)
			{
				BpTfUrl += "&format=pretty";
			}

			SteamUrl = "http://api.steampowered.com/IEconItems_" + TF2_APP_ID + 
				"/GetSchema/v0001/?key=" + STEAM_API_KEY + "&language=en_US";

			if (!Directory.Exists(cacheLocation))
			{
				Directory.CreateDirectory(cacheLocation);
				Logger.Log("Created Cache at " + cacheLocation);
			}

			CacheLocation = cacheLocation;
			BpTfFilename = bptfFilename;
			ItemDataFilename = itemFilename;
		}

		public static void LoadData(bool bptfOffline = false, bool steamOffline = false)
		{
			DateTime lastAccess = File.GetLastWriteTime(CacheLocation + BpTfFilename);

			if (DateTime.Now.Subtract(lastAccess).TotalMinutes <= 5.00 || bptfOffline)
			{
				Logger.Log("Retrieving bp.tf cache...");
				try
				{
					BpTfCache = File.ReadAllText(CacheLocation + BpTfFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the bp.tf cache: " + e.Message);
					return;
				}
			}
			else
			{
				Logger.Log("Downloading data from backpack.tf...");
				WebClient client = new WebClient();
				BpTfCache = client.DownloadString(BpTfUrl);
				BpTfCache = BpTfCache.Replace("    ", "\t");
				Logger.Log("  Download complete.");

				File.WriteAllText(CacheLocation + BpTfFilename, BpTfCache);
				Logger.Log("  Saved bp.tf cache.");

			}

			if (steamOffline)
			{
				Logger.Log("Retrieving TF2 item cache...");
				try
				{
					ItemCache = File.ReadAllText(CacheLocation + ItemDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the TF2 item cache: " + e.Message);
				}
			}
			else
			{
				Logger.Log("Downloading data from Steam...");
				WebClient client = new WebClient();
				ItemCache = client.DownloadString(SteamUrl);
				Logger.Log("  Download complete.");

				File.WriteAllText(CacheLocation + ItemDataFilename, ItemCache);
				Logger.Log("  Saved TF2 item cache.");
			}
		}

		public static TF2DataJson ParseItemsJson()
		{
			if (ItemCache == null)
			{
				LoadData();
			}

			Logger.Log("Parsing TF2 JSON data...");
			ItemDataRaw = JsonConvert.DeserializeObject<TF2DataJson>(ItemCache);
			Logger.Log("  Parse complete.");

			return ItemDataRaw;
		}

		public static TF2Data TranslateItemsData()
		{
			if (ItemDataRaw == null)
			{
				ParseItemsJson();
			}

			ItemData = new TF2Data(ItemDataRaw);

			return ItemData;
		}

		public static BpTfPriceDataJson ParsePricesJson()
		{
			if (BpTfCache == null)
			{
				LoadData();
			}

			Logger.Log("Parsing backpack.tf JSON data...");
			PriceDataRaw = JsonConvert.DeserializeObject<BpTfPriceDataJson>(BpTfCache);
			Logger.Log("  Parse complete.");

			Price.RefinedPerKey = PriceDataRaw.response.GetDataFromID(Price.KEY_DEFINDEX)
				.prices["6"].Tradable.Craftable["0"].value;

			return PriceDataRaw;
		}

		public static BpTfPriceData TranslatePricingData()
		{
			if (PriceDataRaw == null)
			{
				ParsePricesJson();
			}
			if (ItemData == null)
			{
				TranslateItemsData();
			}

			PriceData = new BpTfPriceData(PriceDataRaw, ItemData);

			return PriceData;
		}

		public static void AutoSetup()
		{
			Initialize(false);
			LoadData(true, true);
			ParseItemsJson();
			TranslateItemsData();
			ParsePricesJson();
			TranslatePricingData();
		}

		public static void RunCommand(string cmdName, params string[] args)
		{
			CommandHandler.RunCommand(cmdName, args);
		}
		public static void RunCommand(string cmdAndArgs)
		{
			string[] split = cmdAndArgs.Split(' ');
			string name = split[0];

			string[] args = new string[split.Length - 1];
			for (int i = 1; i < split.Length; i++)
			{
				args[i - 1] = split[i];
			}

			RunCommand(name, args);
		}
	}
}
