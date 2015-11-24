using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CustomSteamTools.Classifieds;
using CustomSteamTools.Commands;
using CustomSteamTools.Items;
using CustomSteamTools.Json.BackpackDataJson;
using CustomSteamTools.Json.ItemDataJson;
using CustomSteamTools.Json.MarketPricesJson;
using CustomSteamTools.Json.PriceDataJson;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;

using Newtonsoft.Json;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools
{
	// the "main" class in this DLL
	public static class DataManager
	{
		public static string PriceDataUrl
		{ get; set; }
		public static string SchemaUrl
		{ get; set; }
		public static string MyBackpackDataUrl
		{ get; set; }
		public static string MarketPricesUrl
		{ get; set; }

		public static string PriceDataFilename
		{ get; set; }
		public static string SchemaFilename
		{ get; set; }
		public static string MyBackpackDataFilename
		{ get; set; }
		public static string MarketPricesFilename	// Downloaded from an official bp.tf API, not scraped off
		{ get; set; }								//   steam community market

		public static string CacheLocation
		{ get; set; }

		public static string PricesCache
		{ get; private set; }
		public static string SchemaCache
		{ get; private set; }
		public static string MyBackpackCache
		{ get; private set; }
		public static Dictionary<string, string> BackpackCaches
		{ get; private set; }
		public static string MarketPricesCache
		{ get; private set; }

		public static TF2DataJson SchemaRaw
		{ get; private set; }
		public static GameSchema Schema
		{ get; private set; }

		public static BpTfPriceDataJson PriceDataRaw
		{ get; private set; }
		public static PriceReference PriceData
		{ get; private set; }

		public static TF2BackpackJson MyBackpackDataRaw
		{ get; private set; }
		public static Backpack MyBackpackData
		{ get; private set; }

		public static Dictionary<string, TF2BackpackJson> BackpackDataRaw
		{ get; private set; }
		public static Dictionary<string, Backpack> BackpackData
		{ get; private set; }

		public static MarketPriceDataJson MarketPricesRaw
		{ get; private set; }
		public static MarketReference MarketPrices
		{ get; private set; }

		public static void Initialize()
		{
			string _cachelocation = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "CUSTOM-STEAM-TOOLS");
			string _bptffilename = "bptf-pricedata.json";
			string _itemfilename = "tf2-itemdata.json";
			string _bpdatafilename = "steam-backpackdata-homeuser.json";
			string _marketfilename = "bptf-marketdata.json";
			Initialize(_cachelocation, _bptffilename, _itemfilename, _bpdatafilename, _marketfilename);

			BackpackCaches = new Dictionary<string, string>();
			BackpackDataRaw = new Dictionary<string, TF2BackpackJson>();
			BackpackData = new Dictionary<string, Backpack>();
		}
		
		public static void Initialize(string cacheLocation, string bptfFilename, string itemFilename, 
			string backpackFilename, string marketFilename)
		{
			PriceDataUrl = "http://backpack.tf/api/IGetPrices/v4/?key=" + 
				Settings.Instance.BackpackTFAPIKey + "&format=pretty";

			SchemaUrl = "http://api.steampowered.com/IEconItems_440/GetSchema/v0001/?key=" + 
				Settings.Instance.SteamAPIKey + "&language=en_US";

			MyBackpackDataUrl = GetBackbackUrl(Settings.Instance.HomeSteamID64);

			MarketPricesUrl = "http://backpack.tf/api/IGetMarketPrices/v1/?key=" + 
				Settings.Instance.BackpackTFAPIKey + "&format=pretty";

			if (!Directory.Exists(cacheLocation))
			{
				Directory.CreateDirectory(cacheLocation);
				VersatileIO.Debug("Created Cache at " + cacheLocation);
			}

			CacheLocation = cacheLocation;
			PriceDataFilename = bptfFilename;
			SchemaFilename = itemFilename;
			MyBackpackDataFilename = backpackFilename;
			MarketPricesFilename = marketFilename;
		}

		public static string GetBackbackUrl(string steamID64)
		{
			return "http://api.steampowered.com/IEconItems_440/GetPlayerItems/v0001/?key=" + 
				Settings.Instance.SteamAPIKey + "&steamid=" + steamID64;
		}

		public static void LoadItemSchema(bool forceOffline)
		{
			bool offline = forceOffline;

			DateTime lastAccess = new DateTime(Settings.Instance.SchemaLastAccess);
			if (DateTime.Now.Subtract(lastAccess).TotalMinutes < 15)
			{
				offline = true;
			}

			if (!File.Exists(CacheLocation + SchemaFilename))
			{
				offline = false;
			}

			DateTime currentAccess = DateTime.Now;
			bool failed = false;
			if (!offline)
			{
				VersatileIO.Debug("Downloading item schema...");
				try
				{
					SchemaCache = Util.DownloadString(SchemaUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					VersatileIO.Error("  Download failed.");
					failed = true;
				}
			}

			if (SchemaCache == null)
			{
				VersatileIO.Debug("Retrieving item schema cache...");
				try
				{
					SchemaCache = File.ReadAllText(CacheLocation + SchemaFilename, Encoding.UTF8);
					VersatileIO.Verbose("  Retrieval complete.");
				}
				catch (Exception e)
				{
					VersatileIO.Fatal("  Schema retrieval failed: " + e.Message);
					throw new RetrievalFailedException(RetrievalType.Schema);
				}
			}

			SchemaCache = Util.Asciify(SchemaCache);

			ParseItemsJson();
			TranslateItemsData();

			if (!failed && !offline) // don't bother writing again if it's the same thing
			{
				File.WriteAllText(CacheLocation + SchemaFilename, SchemaCache, Encoding.UTF8);
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
				VersatileIO.Debug("Downloading backpack data for '{0}'...", Settings.Instance.SteamPersonaName);
				try
				{
					MyBackpackCache = Util.DownloadString(GetBackbackUrl(Settings.Instance.HomeSteamID64), 
						Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					VersatileIO.Error("  Download failed.");
					failed = true;
				}
			}

			if (MyBackpackCache == null)
			{
				VersatileIO.Debug("Retrieving backpack data cache...");
				try
				{
					MyBackpackCache = File.ReadAllText(CacheLocation + MyBackpackDataFilename, Encoding.UTF8);
					VersatileIO.Verbose("  Retrieval complete.");
				}
				catch (Exception e)
				{
					VersatileIO.Fatal("  Backpack retrieval failed: " + e.Message);
					throw new RetrievalFailedException(RetrievalType.BackpackContents);
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
				VersatileIO.Debug("Downloading price list...");
				try
				{
					PricesCache = Util.DownloadString(PriceDataUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					VersatileIO.Error("  Download failed.");
					failed = true;
				}
			}

			if (PricesCache == null)
			{
				VersatileIO.Debug("Retrieving price list cache...");
				try
				{
					PricesCache = File.ReadAllText(CacheLocation + PriceDataFilename, Encoding.UTF8);
					VersatileIO.Verbose("  Retrieval complete.");
				}
				catch (Exception e)
				{
					VersatileIO.Fatal("  Price data retrieval failed: " + e.Message);
					throw new RetrievalFailedException(RetrievalType.PriceData);
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
				VersatileIO.Debug("Downloading market price list...");
				try
				{
					MarketPricesCache = Util.DownloadString(MarketPricesUrl, Settings.Instance.DownloadTimeout);
				}
				catch (WebException)
				{
					VersatileIO.Error("  Download failed.");
					failed = true;
				}
			}

			if (MarketPricesCache == null)
			{
				VersatileIO.Debug("Retrieving market prices cache...");
				try
				{
					MarketPricesCache = File.ReadAllText(CacheLocation + MarketPricesFilename, Encoding.UTF8);
					VersatileIO.Verbose("  Retrieval complete.");
				}
				catch (Exception e)
				{
					VersatileIO.Fatal("  Market price retrieval failed: " + e.Message);
					throw new RetrievalFailedException(RetrievalType.MarketData);
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

			VersatileIO.Debug("Requesting backpack data for user #{0} from Steam...", steamID64);
			string result = Util.DownloadString(GetBackbackUrl(steamID64), TIMEOUT);
			if (result == null)
			{
				return false;
			}

			BackpackCaches.Add(steamID64, result);

			VersatileIO.Verbose("  Parsing backpack data...");
			TF2BackpackJson json = JsonConvert.DeserializeObject<TF2BackpackJson>(BackpackCaches[steamID64]);
			BackpackDataRaw.Add(steamID64, json);

			if (json.result.status != TF2BackpackResultJson.Status.SUCCESS)
			{
				VersatileIO.Fatal("  Error parsing backpack data: " + TF2BackpackResultJson.Status
					.ErrorMessages[json.result.status]);
				return false;
			}

			if (Schema == null)
			{
				TranslateItemsData();
			}
			Backpack data = new Backpack(json, Schema);
			BackpackData.Add(steamID64, data);
			VersatileIO.Verbose("  Parse complete.");

			return true;
		}

		#region JSON stuff

		public static TF2DataJson ParseItemsJson()
		{
			VersatileIO.Verbose("Parsing TF2 item data...");
			SchemaRaw = JsonConvert.DeserializeObject<TF2DataJson>(SchemaCache);
			VersatileIO.Verbose("  Parse complete.");

			return SchemaRaw;
		}

		public static GameSchema TranslateItemsData()
		{
			if (SchemaRaw == null)
			{
				ParseItemsJson();
			}

			Schema = new GameSchema(SchemaRaw);

			return Schema;
		}

		public static BpTfPriceDataJson ParsePricesJson()
		{
			VersatileIO.Verbose("Parsing backpack.tf price data...");
			PriceDataRaw = JsonConvert.DeserializeObject<BpTfPriceDataJson>(PricesCache);
			VersatileIO.Verbose("  Parse complete.");

			Price.RefinedPerKey = PriceDataRaw.response.GetDataFromID(Price.KEY_DEFINDEX)
				.prices["6"].Tradable.Craftable["0"].value;

			return PriceDataRaw;
		}

		public static PriceReference TranslatePricingData()
		{
			if (PriceDataRaw == null)
			{
				ParsePricesJson();
			}
			if (Schema == null)
			{
				TranslateItemsData();
			}

			PriceData = new PriceReference(PriceDataRaw, Schema);

			return PriceData;
		}

		public static TF2BackpackJson ParseBackpackJson()
		{
			VersatileIO.Verbose("Parsing steam backpack data for '{0}'...", Settings.Instance.SteamPersonaName);
			MyBackpackDataRaw = JsonConvert.DeserializeObject<TF2BackpackJson>(MyBackpackCache);
			VersatileIO.Verbose("  Parse complete.");

			return MyBackpackDataRaw;
		}

		public static Backpack TranslateBackpackData()
		{
			if (MyBackpackDataRaw == null)
			{
				ParseBackpackJson();
			}
			if (Schema == null)
			{
				TranslateItemsData();
			}

			MyBackpackData = new Backpack(MyBackpackDataRaw, Schema);

			return MyBackpackData;
		}

		public static MarketPriceDataJson ParseMarketJson()
		{
			VersatileIO.Verbose("Parsing market price data...");
			MarketPricesRaw = JsonConvert.DeserializeObject<MarketPriceDataJson>(MarketPricesCache);
			VersatileIO.Verbose("  Parse complete.");

			return MarketPricesRaw;
		}

		public static MarketReference TranslateMarketPrices()
		{
			if (MarketPricesRaw == null)
			{
				ParseMarketJson();
			}
			if (Schema == null)
			{
				TranslateItemsData();
			}

			MarketPrices = new MarketReference(MarketPricesRaw, Schema);

			return MarketPrices;
		}

		#endregion JSON stuff

		public static void AutoSetup(bool force = true)
		{
			Settings.Load();
			Settings.Instance.GetNecessaryInfo();

			Initialize();

			bool success = false;
			while (!success)
			{
				try
				{
					LoadItemSchema(!force);
					LoadMyBackpackData(!force);
					LoadPriceData(!force);
					LoadMarketData(!force);

					success = true;
				}
				catch (RetrievalFailedException e)
				{
					VersatileIO.Fatal("Details: " + e.ToString());
					VersatileIO.Warning("Retrieval failed. Attempting again in 10 seconds.");

					Thread.Sleep(10000);
				}
			}

			Settings.Instance.Save();

			FixHauntedItems();
		}

		public static void FixHauntedItems()
		{
			if (Schema == null)
			{
				ParseItemsJson();
			}

			if (PriceData == null)
			{
				ParsePricesJson();
			}

			VersatileIO.Verbose("Fixing haunted items...");
			foreach (Item i in Schema.Items)
			{
				List<ItemPricing> prices = PriceData.GetAllPriceData(i);

				if (prices.Exists((p) => p.Quality == Quality.Haunted))
				{
					i.HasHauntedVersion = true;
				}
			}
			VersatileIO.Verbose("  Fix complete.");
		}

		public static void RunCommand(string cmdName, params string[] args)
		{
			CommandHandler.Instance.RunCommand(cmdName, args);
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
