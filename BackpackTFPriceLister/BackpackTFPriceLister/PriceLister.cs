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
		public static string BackpackDataUrl
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
		public static string BackpackCache
		{ get; private set; }

		public static TF2DataJson ItemDataRaw
		{ get; private set; }
		public static TF2Data ItemData
		{ get; private set; }

		public static BpTfPriceDataJson PriceDataRaw
		{ get; private set; }
		public static BpTfPriceData PriceData
		{ get; private set; }

		public static TF2BackpackJson BackpackDataRaw
		{ get; private set; }
		public static TF2BackpackData BackpackData
		{ get; private set; }

		public static void Initialize(bool fancyJson)
		{
			string _cachelocation = Environment.GetEnvironmentVariable("TEMP") + "\\BACKPACK.TF-PRICELIST\\";
			string _bptffilename = "bptf-pricedata.json";
			string _itemfilename = "tf2-itemdata.json";
			string _bpdatafilename = "steam-backpackdata-sealedinterface.json";
			Initialize(_cachelocation, _bptffilename, _itemfilename, _bpdatafilename, fancyJson);
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

			BackpackDataUrl = "http://api.steampowered.com/IEconItems_" + TF2_APP_ID +
				"/GetPlayerItems/v0001/?key=" + STEAM_API_KEY + "&steamid=" + SEALEDINTERFACE_STEAMID;

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

		public static void LoadData(bool bptfOffline = false, bool steamOffline = false)
		{
			DateTime lastAccessPrices = File.GetLastWriteTime(CacheLocation + PriceDataFilename);
			DateTime lastAccessItem = File.GetLastWriteTime(CacheLocation + ItemDataFilename);

			bool backpackOffline = bptfOffline || DateTime.Now.Subtract(lastAccessPrices).TotalMinutes <= 5.00;
			bool itemsOffline = steamOffline || DateTime.Now.Subtract(lastAccessItem).TotalMinutes <= 5.00;

			// backpack.tf
			bool priceDlFailed = false;
			if (!backpackOffline)
			{
				Logger.Log("Downloading data from backpack.tf...");
				WebClient client = new WebClient();
				try
				{
					PricesCache = client.DownloadString(PriceDataUrl);
					PricesCache = PricesCache.Replace("    ", "\t");
					Logger.Log("  Download complete.");

					File.WriteAllText(CacheLocation + PriceDataFilename, PricesCache);
					Logger.Log("  Saved bp.tf cache.");
				}
				catch (Exception e)
				{
					Logger.Log("  Download failed: " + e.Message, MessageType.Error);
					priceDlFailed = true;
				}
			}

			if (backpackOffline || priceDlFailed)
			{
				Logger.Log("Retrieving bp.tf cache...");
				try
				{
					PricesCache = File.ReadAllText(CacheLocation + PriceDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the bp.tf cache: " + e.Message);
					return;
				}
			}

			// Steam
			bool itemDlFailed = false, backpackDlFailed = false;
			if (!itemsOffline)
			{
				Logger.Log("Downloading TF2 data from Steam...");
				WebClient client = new WebClient();
				try
				{
					ItemCache = client.DownloadString(ItemDataUrl);
					Logger.Log("  Download complete.");

					File.WriteAllText(CacheLocation + ItemDataFilename, ItemCache);
					Logger.Log("  Saved TF2 item cache.");
				}
				catch (Exception e)
				{
					Logger.Log("  Download failed: " + e.Message, MessageType.Error);
					itemDlFailed = true;
				}

				Logger.Log("Downloading backpack data from Steam...");
				try
				{
					BackpackCache = client.DownloadString(BackpackDataUrl);
					Logger.Log("  Download complete.");

					File.WriteAllText(CacheLocation + BackpackDataFilename, BackpackCache);
				}
				catch (Exception e)
				{
					Logger.Log("  Download failed: " + e.Message, MessageType.Error);
					backpackDlFailed = true;
				}
			}

			if (itemsOffline || itemDlFailed)
			{
				Logger.Log("Retrieving TF2 item cache...");
				try
				{
					ItemCache = File.ReadAllText(CacheLocation + ItemDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the TF2 item cache: " + e.Message, MessageType.Error);
				}
			}

			if (itemsOffline || backpackDlFailed)
			{
				Logger.Log("Retrieving backpack data cache...");
				try
				{
					BackpackCache = File.ReadAllText(CacheLocation + BackpackDataFilename);
				}
				catch (Exception e)
				{
					Logger.Log("  An error occurred reading the steam backpack cache: " + e.Message, MessageType.Error);
				}
			}
		}

		public static TF2DataJson ParseItemsJson()
		{
			if (ItemCache == null)
			{
				LoadData();

				if (PricesCache == null)
				{
					Logger.Log("  Could not parse item data as it was never loaded.", MessageType.Error);
					return null;
				}
			}

			Logger.Log("Parsing TF2 item data...");
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
			if (PricesCache == null)
			{
				LoadData();

				if (PricesCache == null)
				{
					Logger.Log("  Could not parse price data as it was never loaded.", MessageType.Error);
					return null;
				}
			}

			Logger.Log("Parsing backpack.tf price data...");
			PriceDataRaw = JsonConvert.DeserializeObject<BpTfPriceDataJson>(PricesCache);
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

		public static TF2BackpackJson ParseBackpackJson()
		{
			if (BackpackCache == null)
			{
				LoadData();

				if (BackpackCache == null)
				{
					Logger.Log("  Could not parse backpack data as it was never loaded.", MessageType.Error);
					return null;
				}
			}

			Logger.Log("Parsing steam backpack data for 'sealed interface'...");
			BackpackDataRaw = JsonConvert.DeserializeObject<TF2BackpackJson>(BackpackCache);
			Logger.Log("  Parse complete.");

			return BackpackDataRaw;
		}

		public static TF2BackpackData TranslateBackpackData()
		{
			if (BackpackDataRaw == null)
			{
				ParseBackpackJson();
			}
			if (ItemData == null)
			{
				TranslateItemsData();
			}

			BackpackData = new TF2BackpackData(BackpackDataRaw, ItemData);

			return BackpackData;
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
