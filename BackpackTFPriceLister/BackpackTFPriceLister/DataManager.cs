using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using BackpackTFPriceLister.Json.BackpackDataJson;
using BackpackTFPriceLister.Json.ItemDataJson;
using BackpackTFPriceLister.Json.MarketPricesJson;
using BackpackTFPriceLister.Json.PriceDataJson;

using Newtonsoft.Json;

namespace BackpackTFPriceLister
{
	// the "main" class in this DLL
	public static class DataManager
    {
		public const string BPTF_API_KEY = "5612f911ba8d880424a41d01";
		public const string STEAM_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";
		public const string TF2_APP_ID = "440";
		public const string SEALEDINTERFACE_STEAMID = "76561198111510726";

		public static string PriceDataUrl
		{ get; set; }
		public static string ItemDataUrl
		{ get; set; }
		public static string MyBackpackDataUrl
		{ get; set; }
		public static string MarketPricesUrl
		{ get; set; }

		public static string PriceDataFilename
		{ get; set; }
		public static string ItemDataFilename
		{ get; set; }
		public static string MyBackpackDataFilename
		{ get; set; }
		public static string MarketPricesFilename // Downloaded from an official bp.tf API, not scraped off
		{ get; set; }								 //   steam community market

		public static string CacheLocation
		{ get; set; }

		public static string PricesCache
		{ get; private set; }
		public static string ItemCache
		{ get; private set; }
		public static string MyBackpackCache
		{ get; private set; }
		public static Dictionary<string, string> BackpackCaches
		{ get; private set; }
		public static string MarketPricesCache
		{ get; private set; }

		public static TF2DataJson ItemDataRaw
		{ get; private set; }
		public static TF2Data ItemData
		{ get; private set; }

		public static BpTfPriceDataJson PriceDataRaw
		{ get; private set; }
		public static BpTfPriceData PriceData
		{ get; private set; }

		public static TF2BackpackJson MyBackpackDataRaw
		{ get; private set; }
		public static TF2BackpackData MyBackpackData
		{ get; private set; }

		public static Dictionary<string, TF2BackpackJson> BackpackDataRaw
		{ get; private set; }
		public static Dictionary<string, TF2BackpackData> BackpackData
		{ get; private set; }

		public static MarketPriceDataJson MarketPricesRaw
		{ get; private set; }
		public static MarketPriceData MarketPrices
		{ get; private set; }

		public static void Initialize(bool fancyJson)
		{
			string _cachelocation = Environment.GetEnvironmentVariable("TEMP") + "\\BACKPACK.TF-PRICELIST\\";
			string _bptffilename = "bptf-pricedata.json";
			string _itemfilename = "tf2-itemdata.json";
			string _bpdatafilename = "steam-backpackdata-sealedinterface.json";
			string _marketfilename = "bptf-marketdata.json";
			Initialize(_cachelocation, _bptffilename, _itemfilename, _bpdatafilename, _marketfilename, fancyJson);

			BackpackCaches = new Dictionary<string, string>();
			BackpackDataRaw = new Dictionary<string, TF2BackpackJson>();
			BackpackData = new Dictionary<string, TF2BackpackData>();
		}
		
		public static void Initialize(string cacheLocation, string bptfFilename, string itemFilename, 
			string backpackFilename, string marketFilename, bool fancyJson)
		{
			PriceDataUrl = "http://backpack.tf/api/IGetPrices/v4/?key=" + BPTF_API_KEY;

			if (fancyJson)
			{
				PriceDataUrl += "&format=pretty";
			}

			ItemDataUrl = "http://api.steampowered.com/IEconItems_" + TF2_APP_ID + 
				"/GetSchema/v0001/?key=" + STEAM_API_KEY + "&language=en_US";

			MyBackpackDataUrl = GetBackbackUrl(SEALEDINTERFACE_STEAMID);

			MarketPricesUrl = "http://backpack.tf/api/IGetMarketPrices/v1/?key=" + BPTF_API_KEY;

			if (fancyJson)
			{
				MarketPricesUrl += "&format=pretty";
			}

			if (!Directory.Exists(cacheLocation))
			{
				Directory.CreateDirectory(cacheLocation);
				Logger.Log("Created Cache at " + cacheLocation);
			}

			CacheLocation = cacheLocation;
			PriceDataFilename = bptfFilename;
			ItemDataFilename = itemFilename;
			MyBackpackDataFilename = backpackFilename;
			MarketPricesFilename = marketFilename;
		}

		public static string GetBackbackUrl(string steamID64)
		{
			return "http://api.steampowered.com/IEconItems_" + TF2_APP_ID +
				"/GetPlayerItems/v0001/?key=" + STEAM_API_KEY + "&steamid=" + steamID64;
		}

		public static void LoadItemSchema(bool forceOffline)
		{
			bool offline = forceOffline;

			DateTime lastAccess = new DateTime(Settings.Instance.SchemaLastAccess);
			if (DateTime.Now.Subtract(lastAccess).TotalMinutes < 15)
			{
				offline = true;
			}

			if (!File.Exists(CacheLocation + ItemDataFilename))
			{
				offline = false;
			}

			DateTime currentAccess = DateTime.Now;
			bool failed = false;
			if (!offline)
			{
				Logger.Log("Downloading item schema...", ConsoleColor.DarkGray);
				try
				{
					ItemCache = Util.DownloadString(ItemDataUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					Logger.Log("  Download failed.", ConsoleColor.Red);
					failed = true;
				}
			}

			if (ItemCache == null)
			{
				Logger.Log("Retrieving item schema cache...", ConsoleColor.DarkGray);
				try
				{
					ItemCache = File.ReadAllText(CacheLocation + ItemDataFilename, Encoding.UTF8);
					Logger.Log("  Retrieval complete.", ConsoleColor.DarkGray);
				}
				catch (Exception e)
				{
					Logger.Log("  Retrieval failed: " + e.Message, ConsoleColor.Red);
				}
			}

			ItemCache = Util.Asciify(ItemCache);

			ParseItemsJson();
			TranslateItemsData();

			if (!failed && !offline) // don't bother writing again if it's the same thing
			{
				File.WriteAllText(CacheLocation + ItemDataFilename, ItemCache, Encoding.UTF8);
				Settings.Instance.SchemaLastAccess = currentAccess.Ticks;
			}
		}

		public static void LoadMyBackpackData(bool forceOffline)
		{
			bool offline = forceOffline;

			DateTime lastAccess = new DateTime(Settings.Instance.BackpackLastAccess);
			if (DateTime.Now.Subtract(lastAccess).TotalMinutes < 15)
			{
				offline = true;
			}

			if (!File.Exists(CacheLocation + MyBackpackDataFilename))
			{
				offline = false;
			}

			DateTime currentAccess = DateTime.Now;
			bool failed = false;
			if (!offline)
			{
				Logger.Log("Downloading backpack data for 'sealed interface'...", ConsoleColor.DarkGray);
				try
				{
					MyBackpackCache = Util.DownloadString(GetBackbackUrl(SEALEDINTERFACE_STEAMID), 
						Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					Logger.Log("  Download failed.", ConsoleColor.Red);
					failed = true;
				}
			}

			if (MyBackpackCache == null)
			{
				Logger.Log("Retrieving backpack data cache...", ConsoleColor.DarkGray);
				try
				{
					MyBackpackCache = File.ReadAllText(CacheLocation + MyBackpackDataFilename, Encoding.UTF8);
					Logger.Log("  Retrieval complete.", ConsoleColor.DarkGray);
				}
				catch (Exception e)
				{
					Logger.Log("  Retrieval failed: " + e.Message, ConsoleColor.Red);
				}
			}

			MyBackpackCache = Util.Asciify(MyBackpackCache);

			ParseBackpackJson();
			TranslateBackpackData();

			if (!failed && !offline) // don't bother writing again if it's the same thing
			{
				File.WriteAllText(CacheLocation + MyBackpackDataFilename, MyBackpackCache, Encoding.UTF8);
				Settings.Instance.BackpackLastAccess = currentAccess.Ticks;
			}
		}

		public static void LoadPriceData(bool forceOffline)
		{
			bool offline = forceOffline;

			DateTime lastAccess = new DateTime(Settings.Instance.PriceListLastAccess);
			if (DateTime.Now.Subtract(lastAccess).TotalMinutes < 15)
			{
				offline = true;
			}

			if (!File.Exists(CacheLocation + PriceDataFilename))
			{
				offline = false;
			}

			DateTime currentAccess = DateTime.Now;
			bool failed = false;
			if (!offline)
			{
				Logger.Log("Downloading price list...", ConsoleColor.DarkGray);
				try
				{
					PricesCache = Util.DownloadString(PriceDataUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					Logger.Log("  Download failed.", ConsoleColor.Red);
					failed = true;
				}
			}

			if (PricesCache == null)
			{
				Logger.Log("Retrieving price list cache...", ConsoleColor.DarkGray);
				try
				{
					PricesCache = File.ReadAllText(CacheLocation + PriceDataFilename, Encoding.UTF8);
					Logger.Log("  Retrieval complete.", ConsoleColor.DarkGray);
				}
				catch (Exception e)
				{
					Logger.Log("  Retrieval failed: " + e.Message, ConsoleColor.Red);
				}
			}

			PricesCache = PricesCache.Replace("    ", "\t");
			PricesCache = Util.Asciify(PricesCache);

			ParsePricesJson();
			TranslatePricingData();

			if (!failed && !offline) // don't bother writing again if it's the same thing
			{
				File.WriteAllText(CacheLocation + PriceDataFilename, PricesCache, Encoding.UTF8);
				Settings.Instance.PriceListLastAccess = currentAccess.Ticks;
			}
		}

		public static void LoadMarketData(bool forceOffline)
		{
			bool offline = forceOffline;

			DateTime lastAccess = new DateTime(Settings.Instance.MarketPricesLastAccess);
			if (DateTime.Now.Subtract(lastAccess).TotalMinutes < 15)
			{
				offline = true;
			}

			if (!File.Exists(CacheLocation + MarketPricesFilename))
			{
				offline = false;
			}

			DateTime currentAccess = DateTime.Now;
			bool failed = false;
			if (!offline)
			{
				Logger.Log("Downloading market price list...", ConsoleColor.DarkGray);
				try
				{
					MarketPricesCache = Util.DownloadString(MarketPricesUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					Logger.Log("  Download failed.", ConsoleColor.Red);
					failed = true;
				}
			}

			if (MarketPricesCache == null)
			{
				Logger.Log("Retrieving market prices cache...", ConsoleColor.DarkGray);
				try
				{
					MarketPricesCache = File.ReadAllText(CacheLocation + MarketPricesFilename, Encoding.UTF8);
					Logger.Log("  Retrieval complete.", ConsoleColor.DarkGray);
				}
				catch (Exception e)
				{
					Logger.Log("  Retrieval failed: " + e.Message, ConsoleColor.Red);
				}
			}

			MarketPricesCache = MarketPricesCache.Replace("    ", "\t");
			MarketPricesCache = Util.Asciify(MarketPricesCache);

			ParseMarketJson();
			TranslateMarketPrices();

			if (!failed && !offline) // don't bother writing again if it's the same thing
			{
				File.WriteAllText(CacheLocation + MarketPricesFilename, MarketPricesCache, Encoding.UTF8);
				Settings.Instance.MarketPricesLastAccess = currentAccess.Ticks;
			}
		}

		public static bool LoadOtherBackpack(string steamID64)
		{
			TimeSpan TIMEOUT = TimeSpan.FromSeconds(20);

			Logger.Log("Requesting backpack data for user #" + steamID64 + " from Steam...", ConsoleColor.DarkGray);
			string result = Util.DownloadString(GetBackbackUrl(steamID64), TIMEOUT);
			if (result == null)
			{
				return false;
			}

			BackpackCaches.Add(steamID64, result);

			Logger.Log("  Parsing backpack data...", ConsoleColor.DarkGray);
			TF2BackpackJson json = JsonConvert.DeserializeObject<TF2BackpackJson>(BackpackCaches[steamID64]);
			BackpackDataRaw.Add(steamID64, json);

			if (json.result.status != TF2BackpackResultJson.Status.SUCCESS)
			{
				Logger.Log("  Error parsing: " + TF2BackpackResultJson.Status.ErrorMessages[json.result.status], ConsoleColor.Red);
				return false;
			}

			if (ItemData == null)
			{
				TranslateItemsData();
			}
			TF2BackpackData data = new TF2BackpackData(json, ItemData);
			BackpackData.Add(steamID64, data);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

			return true;
		}

		#region JSON stuff

		public static TF2DataJson ParseItemsJson()
		{
			Logger.Log("Parsing TF2 item data...", ConsoleColor.DarkGray);
			ItemDataRaw = JsonConvert.DeserializeObject<TF2DataJson>(ItemCache);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

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
			Logger.Log("Parsing backpack.tf price data...", ConsoleColor.DarkGray);
			PriceDataRaw = JsonConvert.DeserializeObject<BpTfPriceDataJson>(PricesCache);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

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

		public static TF2BackpackJson ParseBackpackJson()
		{
			Logger.Log("Parsing steam backpack data for 'sealed interface'...", ConsoleColor.DarkGray);
			MyBackpackDataRaw = JsonConvert.DeserializeObject<TF2BackpackJson>(MyBackpackCache);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

			return MyBackpackDataRaw;
		}

		public static TF2BackpackData TranslateBackpackData()
		{
			if (MyBackpackDataRaw == null)
			{
				ParseBackpackJson();
			}
			if (ItemData == null)
			{
				TranslateItemsData();
			}

			MyBackpackData = new TF2BackpackData(MyBackpackDataRaw, ItemData);

			return MyBackpackData;
		}

		public static MarketPriceDataJson ParseMarketJson()
		{
			Logger.Log("Parsing market price data...", ConsoleColor.DarkGray);
			MarketPricesRaw = JsonConvert.DeserializeObject<MarketPriceDataJson>(MarketPricesCache);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

			return MarketPricesRaw;
		}

		public static MarketPriceData TranslateMarketPrices()
		{
			if (MarketPricesRaw == null)
			{
				ParseMarketJson();
			}
			if (ItemData == null)
			{
				TranslateItemsData();
			}

			MarketPrices = new MarketPriceData(MarketPricesRaw, ItemData);

			return MarketPrices;
		}

		#endregion JSON stuff

		public static void AutoSetup(bool fancy = false, bool force = true)
		{
			Settings.Load();

			Initialize(fancy);

			LoadItemSchema(!force);
			LoadMyBackpackData(!force);
			LoadPriceData(!force);
			LoadMarketData(!force);

			Settings.Save();

			FixHauntedItems();
		}

		public static void FixHauntedItems()
		{
			if (ItemData == null)
			{
				ParseItemsJson();
			}

			if (PriceData == null)
			{
				ParsePricesJson();
			}

			Logger.Log("Fixing haunted items...", ConsoleColor.DarkGray);
			foreach (Item i in ItemData.Items)
			{
				List<ItemPricing> prices = PriceData.GetAllPriceData(i);

				if (prices.Exists((p) => p.Quality == Quality.Haunted))
				{
					i.HasHauntedVersion = true;
				}
			}
			Logger.Log("  Fix complete.", ConsoleColor.DarkGray);
		}

		public static void RunCommand(string cmdName, params string[] args)
		{
			CommandHandler.RunCommand(cmdName, args);
		}
		public static void RunCommand(string cmdAndArgs)
		{
			if (cmdAndArgs.Trim() == "")
			{
				return;
			}

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
