using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SealedTradeBot
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Settings
	{
		public enum RefreshPolicy
		{
			OnStartup,
			OnStartupAfterExpiration,
			Manual,
		};

		public static Settings Instance
		{ get; private set; }

		[JsonProperty]
		public List<ulong> VipSteamIDs
		{ get; private set; }

		[JsonProperty]
		public double NonAutomaticRefreshHours
		{ get; private set; }

		[JsonProperty]
		public RefreshPolicy AutoRefreshPolicy
		{ get; private set; }

		public static Settings LoadFromFile()
		{
			string appdata = Environment.GetEnvironmentVariable("APPDATA");
			string dir = appdata + "\\SealedTradeBot";

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			
			string filePath = dir + "\\settings.json";
			if (File.Exists(filePath))
			{
				string contents = File.ReadAllText(filePath);
				Instance = JsonConvert.DeserializeObject<Settings>(contents);
			}
			else
			{
				Instance = new Settings();
			}

			return Instance;
		}

		public static void SaveToFile()
		{
			string appdata = Environment.GetEnvironmentVariable("APPDATA");
			string dir = appdata + "\\SealedTradeBot";

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			string filePath = dir + "\\settings.json";
			string contents = JsonConvert.SerializeObject(Instance);
			File.WriteAllText(filePath, contents);
		}

		public Settings()
		{
			VipSteamIDs = new List<ulong>();
			NonAutomaticRefreshHours = 2;
			AutoRefreshPolicy = RefreshPolicy.OnStartupAfterExpiration;
		}
	}
}
