using BackpackTFPriceLister.BackpackDataJson;
using BackpackTFPriceLister.ItemDataJson;
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
		public const string SEALEDINTERFACE_STEAMID = "76561198111510726";

		public static string PriceDataUrl
		{ get; set; }
		public static string ItemDataUrl
		{ get; set; }
		public static string MyBackpackDataUrl
		{ get; set; }

		public static string PriceDataFilename
		{ get; set; }
		public static string ItemDataFilename
		{ get; set; }
		public static string BackpackDataFilename
		{ get; set; }

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

		public static void Initialize(bool fancyJson)
		{
			string _cachelocation = Environment.GetEnvironmentVariable("TEMP") + "\\BACKPACK.TF-PRICELIST\\";
			string _bptffilename = "bptf-pricedata.json";
			string _itemfilename = "tf2-itemdata.json";
			string _bpdatafilename = "steam-backpackdata-sealedinterface.json";
			Initialize(_cachelocation, _bptffilename, _itemfilename, _bpdatafilename, fancyJson);

			BackpackCaches = new Dictionary<string, string>();
			BackpackDataRaw = new Dictionary<string, TF2BackpackJson>();
			BackpackData = new Dictionary<string, TF2BackpackData>();
		}
		
		public static void Initialize(string cacheLocation, string bptfFilename, string itemFilename, 
			string backpackFilename, bool fancyJson)
		{
			PriceDataUrl = "http://backpack.tf/api/IGetPrices/v4/?key=" + BPTF_API_KEY;

			if (fancyJson)
			{
				PriceDataUrl += "&format=pretty";
			}

			ItemDataUrl = "http://api.steampowered.com/IEconItems_" + TF2_APP_ID + 
				"/GetSchema/v0001/?key=" + STEAM_API_KEY + "&language=en_US";

			MyBackpackDataUrl = GetBackbackUrl(SEALEDINTERFACE_STEAMID);

			if (!Directory.Exists(cacheLocation))
			{
				Directory.CreateDirectory(cacheLocation);
				Logger.Log("Created Cache at " + cacheLocation);
			}

			CacheLocation = cacheLocation;
			PriceDataFilename = bptfFilename;
			ItemDataFilename = itemFilename;
			BackpackDataFilename = backpackFilename;
		}

		public static string GetBackbackUrl(string steamID64)
		{
			return "http://api.steampowered.com/IEconItems_" + TF2_APP_ID +
				"/GetPlayerItems/v0001/?key=" + STEAM_API_KEY + "&steamid=" + steamID64;
		}

		public static void LoadData(bool bptfOffline = false, bool steamOffline = false)
		{
			TimeSpan TIMEOUT = TimeSpan.FromSeconds(15);

			DateTime lastAccessPrices = File.GetLastWriteTime(CacheLocation + PriceDataFilename);
			DateTime lastAccessItem = File.GetLastWriteTime(CacheLocation + ItemDataFilename);

			bool backpackOffline = bptfOffline || DateTime.Now.Subtract(lastAccessPrices).TotalMinutes <= 5.00;
			bool itemsOffline = steamOffline || DateTime.Now.Subtract(lastAccessItem).TotalMinutes <= 5.00;

			// backpack.tf
			bool priceDlFailed = false;
			if (!backpackOffline)
			{
				Logger.Log("Downloading data from backpack.tf...", ConsoleColor.DarkGray);
				PricesCache = Util.DownloadString(PriceDataUrl, TIMEOUT);
				if (PricesCache == null)
				{
					priceDlFailed = true;
				}
				else
				{
					File.WriteAllText(CacheLocation + PriceDataFilename, PricesCache);
					Logger.Log("  Saved bp.tf cache.", ConsoleColor.DarkGray);
				}
			}

			if (backpackOffline || priceDlFailed)
			{
				Logger.Log("Retrieving bp.tf cache...", ConsoleColor.DarkGray);
				try
				{
					PricesCache = File.ReadAllText(CacheLocation + PriceDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the bp.tf cache: " + e.Message, ConsoleColor.Red);
					return;
				}
			}

			// Steam
			bool itemDlFailed = false, backpackDlFailed = false;
			if (!itemsOffline)
			{
				Logger.Log("Downloading TF2 data from Steam...", ConsoleColor.DarkGray);
				ItemCache = Util.DownloadString(ItemDataUrl, TIMEOUT);
				if (ItemCache == null)
				{
					itemDlFailed = true;
				}
				else
				{
					File.WriteAllText(CacheLocation + ItemDataFilename, ItemCache);
					Logger.Log("  Saved TF2 item cache.", ConsoleColor.DarkGray);
				}

				Logger.Log("Downloading backpack data from Steam...", ConsoleColor.DarkGray);
				MyBackpackCache = Util.DownloadString(MyBackpackDataUrl, TIMEOUT);
				if (MyBackpackCache == null)
				{
					backpackDlFailed = true;
				}
				else
				{
					File.WriteAllText(CacheLocation + BackpackDataFilename, MyBackpackCache);
					Logger.Log("  saved Steam backpack cache.", ConsoleColor.DarkGray);
				}
			}

			if (itemsOffline || itemDlFailed)
			{
				Logger.Log("Retrieving TF2 item cache...", ConsoleColor.DarkGray);
				try
				{
					ItemCache = File.ReadAllText(CacheLocation + ItemDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the TF2 item cache: " + e.Message, ConsoleColor.Red);
				}
			}

			if (itemsOffline || backpackDlFailed)
			{
				Logger.Log("Retrieving backpack data cache...", ConsoleColor.DarkGray);
				try
				{
					MyBackpackCache = File.ReadAllText(CacheLocation + BackpackDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the steam backpack cache: " + e.Message, ConsoleColor.Red);
				}
			}
		}

		public static bool LoadOtherBackpack(string steamID64)
		{
			TimeSpan TIMEOUT = TimeSpan.FromSeconds(45);

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

		public static TF2DataJson ParseItemsJson()
		{
			if (ItemCache == null)
			{
				LoadData();

				if (PricesCache == null)
				{
					Logger.Log("  Could not parse item data as it was never loaded.", ConsoleColor.Red);
					return null;
				}
			}

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
			if (PricesCache == null)
			{
				LoadData();

				if (PricesCache == null)
				{
					Logger.Log("  Could not parse price data as it was never loaded.", ConsoleColor.Red);
					return null;
				}
			}

			Logger.Log("Parsing backpack.tf price data...", ConsoleColor.DarkGray);
			PriceDataRaw = JsonConvert.DeserializeObject<BpTfPriceDataJson>(PricesCache);
			Logger.Log("  Parse complete.", ConsoleColor.DarkGray);

			Price.RefinedPerKey = PriceDataRaw.response.GetDataFromID(Price.KEY_DEFINDEX)
				.prices["6"].Tradable.Craftable["0"].value;

			return PriceDataRaw;
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
			if (MyBackpackCache == null)
			{
				LoadData();

				if (MyBackpackCache == null)
				{
					Logger.Log("  Could not parse backpack data as it was never loaded.", ConsoleColor.Red);
					return null;
				}
			}

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

		public static void AutoSetup(bool fancy = false, bool force = true)
		{
			Initialize(fancy);
			LoadData(!force, !force);

			ParseItemsJson();
			TranslateItemsData();

			ParsePricesJson();
			TranslatePricingData();

			ParseBackpackJson();
			TranslateBackpackData();

			FixHauntedItems();
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
