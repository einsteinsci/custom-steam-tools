using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using UltimateUtil.UserInteraction;
using UltimateUtil;

namespace CustomSteamTools.Utils
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Settings
	{
		public static readonly string LOCATION = Path.Combine(
			Environment.GetEnvironmentVariable("appdata"), "CustomSteamTools");
		public static readonly string FILEPATH = Path.Combine(LOCATION, "settings.json");

		public static Settings Instance
		{ get; private set; }

		[JsonProperty]
		public long PriceListLastAccess
		{ get; set; }

		[JsonProperty]
		public long MarketPricesLastAccess
		{ get; set; }

		[JsonProperty]
		public long SchemaLastAccess
		{ get; set; }

		[JsonProperty]
		public long BackpackLastAccess
		{ get; set; }

		[JsonProperty]
		public int DownloadTimeoutSeconds
		{ get; set; }

		[JsonProperty]
		public string HomeSteamID64
		{ get; set; }

		[JsonProperty]
		public string SteamPersonaName
		{ get; set; }

		[JsonProperty]
		public string BackpackTFAPIKey
		{ get; set; }

		[JsonProperty]
		public string SteamAPIKey
		{ get; set; }

		[JsonProperty]
		public int DealsPriceDropThresholdListingCount
		{ get; set; }

		[JsonProperty]
		public double DealsPriceDropThresholdPriceBelow
		{ get; set; }

		public TimeSpan DownloadTimeout => TimeSpan.FromSeconds(DownloadTimeoutSeconds);

		public static Settings Load(bool affectInstance = true)
		{
			Settings res = null;

			VersatileIO.Debug("Loading settings");

			if (!Directory.Exists(LOCATION))
			{
				Directory.CreateDirectory(LOCATION);
			}

			if (File.Exists(FILEPATH))
			{
				string contents = File.ReadAllText(FILEPATH);
				res = JsonConvert.DeserializeObject<Settings>(contents);
			}
			else
			{
				res = new Settings();
			}

			VersatileIO.Verbose("  Settings loaded.");

			if (affectInstance)
			{
				Instance = res;
			}

			return res;
		}

		public void GetNecessaryInfo()
		{
			bool save = false;

			if (HomeSteamID64.IsNullOrEmpty())
			{
				HomeSteamID64 = VersatileIO.GetString("Enter your SteamID64: ");
				SteamPersonaName = VersatileIO.GetString("Enter your Steam persona name: ");
				save = true;
			}

			if (BackpackTFAPIKey.IsNullOrEmpty())
			{
				BackpackTFAPIKey = VersatileIO.GetString("Enter your backpack.tf API key: ");
				save = true;
			}

			if (SteamAPIKey.IsNullOrEmpty())
			{
				SteamAPIKey = VersatileIO.GetString("Enter your steam API key: ");
				save = true;
			}

			if (save)
			{
				Save();
			}
		}

		public void Save()
		{
			VersatileIO.Debug("Saving settings");
			
			if (!Directory.Exists(LOCATION))
			{
				Directory.CreateDirectory(LOCATION);
			}

			string contents = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(FILEPATH, contents);

			VersatileIO.Verbose("  Settings saved.");
		}

		public Settings()
		{
			DownloadTimeoutSeconds = 20;
			DealsPriceDropThresholdListingCount = 3;
			DealsPriceDropThresholdPriceBelow = 0.97;
		}
	}
}
