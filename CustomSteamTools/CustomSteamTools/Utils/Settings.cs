﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using UltimateUtil.UserInteraction;
using UltimateUtil;

namespace CustomSteamTools.Utils
{
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class Settings : ICloneable
	{
		[JsonIgnore]
		public static readonly string LOCATION = Path.Combine(
			Environment.GetEnvironmentVariable("appdata"), "CustomSteamTools");

		[JsonIgnore]
		public static readonly string FILEPATH = Path.Combine(LOCATION, "settings.json");

		[JsonIgnore]
		public static Settings Instance
		{ get; set; }

		[JsonIgnore]
		public bool Initialized
		{ get; private set; }

		#region settings

		[Setting("Last access time (in ticks) of bp.tf price list.")]
		public long PriceListLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of marketplace price list.")]
		public long MarketPricesLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of TF2 item schema.")]
		public long SchemaLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of home user's backpack.")]
		public long BackpackLastAccess
		{ get; set; }

		[Setting("Last access time (in ticks) of home user's friends list.")]
		public long FriendsListLastAccess
		{ get; set; }

		[Setting("Lenth of time to wait (in seconds) during download before timing out and giving up.")]
		public double DownloadTimeoutSeconds
		{ get; set; }

		[Setting("SteamID64 of main user. Used for 'home' backpack caching.")]
		public string HomeSteamID64
		{ get; set; }

		[Setting("Steam persona name of main user. Used to denote home user apart from others.")]
		public string SteamPersonaName
		{ get; set; }

		[Setting("API key supplied by backpack.tf.")]
		public string BackpackTFAPIKey
		{ get; set; }

		[Setting("API key supplied by Steam dev.")]
		public string SteamAPIKey
		{ get; set; }

		[Setting("Number of listings during deal-finding that are below threshold before marking as 'falling price'.")]
		public int DealsPriceDropThresholdListingCount
		{ get; set; }

		[Setting("Amount below bp.tf price a classified listing must be before contributing to 'falling price' threshold.")]
		public double DealsPriceDropThresholdPriceBelow
		{ get; set; }

		[Setting("Last minimum price (in ref) used by range command.")]
		public double RangeLastMinPrice
		{ get; set; }

		[Setting("Last maximum price (in ref) used by range command.")]
		public double RangeLastMaxPrice
		{ get; set; }

		#endregion settings

		[JsonIgnore]
		public TimeSpan DownloadTimeout => TimeSpan.FromSeconds(DownloadTimeoutSeconds);

		private Thread _saveThread;

		public static Settings Load(bool affectInstance = true)
		{
			Settings res;

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

			res.Initialized = true;

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
			try
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
			catch (IOException ex)
			{
				VersatileIO.Error("Save Settings Failed:");
				VersatileIO.Error(ex.Message);
			}
		}

		public void SaveOnOtherThread()
		{
			_saveThread = new Thread(Save);
			_saveThread.Name = "Save Thread";
			_saveThread.Start();
		}

		public object Clone()
		{
			Settings res = new Settings();
			res.PriceListLastAccess = PriceListLastAccess;
			res.MarketPricesLastAccess = MarketPricesLastAccess;
			res.SchemaLastAccess = SchemaLastAccess;
			res.BackpackLastAccess = BackpackLastAccess;
			res.FriendsListLastAccess = FriendsListLastAccess;
			res.DownloadTimeoutSeconds = DownloadTimeoutSeconds;
			res.HomeSteamID64 = HomeSteamID64;
			res.SteamPersonaName = SteamPersonaName;
			res.BackpackTFAPIKey = BackpackTFAPIKey;
			res.SteamAPIKey = SteamAPIKey;
			res.DealsPriceDropThresholdListingCount = DealsPriceDropThresholdListingCount;
			res.DealsPriceDropThresholdPriceBelow = DealsPriceDropThresholdPriceBelow;

			return res;
		}

		public Settings()
		{
			DownloadTimeoutSeconds = 20;
			DealsPriceDropThresholdListingCount = 3;
			DealsPriceDropThresholdPriceBelow = 0.97;
			RangeLastMinPrice = 2.0;
			RangeLastMaxPrice = 10.0;

			Initialized = false;
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
