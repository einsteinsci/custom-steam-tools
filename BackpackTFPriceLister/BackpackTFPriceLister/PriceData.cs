using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	 public static class PriceData
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
					Logger.Log("An error occurred reading the bp.tf cache: " + e.Message);
					return;
				}
			}
			else
			{
				Logger.Log("Connecting to backpack.tf...");
				WebClient client = new WebClient();
				BpTfCache = client.DownloadString(BpTfUrl);
				Logger.Log("Download complete.");
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
					Logger.Log("An error occurred reading the TF2 item cache: " + e.Message);
				}
			}
			else
			{
				Logger.Log("Connecting to Steam...");
				WebClient client = new WebClient();
				ItemCache = client.DownloadString(SteamUrl);
				Logger.Log("Download complete.");
			}

			Logger.Log("Caching data...");
			WriteCache(BpTfCache, ItemCache);
		}

		private static void WriteCache(string bptfCache, string tf2Cache)
		{
			File.WriteAllText(CacheLocation + BpTfFilename, bptfCache);
			Logger.Log("Saved bp.tf cache.");

			File.WriteAllText(CacheLocation + ItemDataFilename, tf2Cache);
			Logger.Log("Saved TF2 item cache.");
		}
	}
}
