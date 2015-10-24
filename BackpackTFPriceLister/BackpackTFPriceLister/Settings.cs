using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BackpackTFPriceLister
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Settings
	{
		public static readonly string LOCATION = Environment.GetEnvironmentVariable("appdata") + "\\CustomSteamTools\\";
		public static readonly string FILEPATH = LOCATION + "settings.json";

		public static Settings Instance
		{ get; private set; }

		[JsonProperty]
		public long BpTfLastAccess
		{ get; set; }

		[JsonProperty]
		public long SteamLastAccess
		{ get; set; }

		[JsonProperty]
		public int DownloadTimeoutSeconds
		{ get; set; }

		public TimeSpan DownloadTimeout => TimeSpan.FromSeconds(DownloadTimeoutSeconds);

		public static void Load()
		{
			Logger.Log("Loading settings...", ConsoleColor.DarkGray);

			if (!Directory.Exists(LOCATION))
			{
				Directory.CreateDirectory(LOCATION);
			}

			if (File.Exists(FILEPATH))
			{
				string contents = File.ReadAllText(FILEPATH);
				Instance = JsonConvert.DeserializeObject<Settings>(contents);
			}
			else
			{
				Instance = new Settings();
			}

			Logger.Log("  Settings loaded.", ConsoleColor.DarkGray);
		}

		public static void Save()
		{
			Logger.Log("Saving settings...", ConsoleColor.DarkGray);
			
			if (!Directory.Exists(LOCATION))
			{
				Directory.CreateDirectory(LOCATION);
			}

			string contents = JsonConvert.SerializeObject(Instance, Formatting.Indented);
			File.WriteAllText(FILEPATH, contents);

			Logger.Log("  Settings saved.", ConsoleColor.DarkGray);
		}

		public Settings()
		{
			DownloadTimeoutSeconds = 20;
		}
	}
}
