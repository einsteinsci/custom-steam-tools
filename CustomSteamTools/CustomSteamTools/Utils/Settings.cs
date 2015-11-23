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

		#region settings

		[Setting("Last access time (in ticks) of bp.tf price list.")]
		[JsonProperty]
		public long PriceListLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of marketplace price list.")]
		[JsonProperty]
		public long MarketPricesLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of TF2 item schema.")]
		[JsonProperty]
		public long SchemaLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of home user's backpack.")]
		[JsonProperty]
		public long BackpackLastAccess
		{ get; set; }

		[Setting("Lenth of time to wait during download before timing out and giving up.")]
		[JsonProperty]
		public int DownloadTimeoutSeconds
		{ get; set; }

		[Setting("SteamID64 of main user. Used for 'home' backpack caching.")]
		[JsonProperty]
		public string HomeSteamID64
		{ get; set; }

		[Setting("Steam persona name of main user. Used to denote home user apart from others.")]
		[JsonProperty]
		public string SteamPersonaName
		{ get; set; }

		[Setting("API key supplied by backpack.tf.")]
		[JsonProperty]
		public string BackpackTFAPIKey
		{ get; set; }

		[Setting("API key supplied by Steam dev.")]
		[JsonProperty]
		public string SteamAPIKey
		{ get; set; }

		[Setting("Number of listings during deal-finding that are below threshold before marking as 'falling price'.")]
		[JsonProperty]
		public int DealsPriceDropThresholdListingCount
		{ get; set; }

		[Setting("Amount below bp.tf price a classified listing must be before contributing to 'falling price' threshold.")]
		[JsonProperty]
		public double DealsPriceDropThresholdPriceBelow
		{ get; set; }

		#endregion settings

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

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class SettingAttribute : Attribute
	{
		public string Meaning
		{ get; private set; }

		public SettingAttribute(string meaning)
		{
			Meaning = meaning;
		}
	}
}
