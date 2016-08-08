using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Backpacks;
using CustomSteamTools.Schema;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdBackpack : ITradeCommand
	{
		public string[] Aliases => new string[] { "bp", "backpack", "inventory" };

		public string Description => "Evaluates the contents of a player's backpack.";

		public string RegistryName => "bp";

		public string Syntax => "bp [steamID64] [/noweapons]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			string steamid = Settings.Instance.HomeSteamID64;
			bool noWeapons = false;
			foreach (string s in args)
			{
				if (s.EqualsIgnoreCase("/noweapons"))
				{
					noWeapons = true;
				}
				else if (!s.StartsWith("/"))
				{
					steamid = s;
				}
			}

			string name = "#" + steamid;
			if (steamid == Settings.Instance.HomeSteamID64)
			{
				name = "'" + Settings.Instance.SteamPersonaName + "'";
			}

			Backpack bp = GetBackpack(steamid);
			if (bp == null)
			{
				VersatileIO.Error("Could not retrieve backpack.");
				return;
			}

			VersatileIO.Info("Backpack contents of user {0} ({1} slots):", name, bp.SlotCount);

			PriceRange netWorth = new PriceRange(Price.Zero);
			Price totalPure = Price.Zero;
			foreach (ItemInstance item in bp.GetAllItems())
			{
				if (!item.Tradable)
				{
					if (!item.Item.IsCheapWeapon() || !noWeapons)
					{
						VersatileIO.WriteLine("  " + item + ": Not Tradable", ConsoleColor.DarkGray);
					}
					continue;
				}

				var checkedPrice = PriceChecker.GetPriceFlagged(item);
				PriceRange? price = checkedPrice.Result;

				string priceString = "&cUNKNOWN";
				if (price != null)
				{
					priceString = "&f" + price.Value;

					netWorth += price.Value;
				}

				#region formatting
				if (item.Item.IsCheapWeapon() && item.Quality == Quality.Unique && noWeapons)
				{
					continue;
				}

				if (checkedPrice.Contains("market"))
				{
					priceString += " &8[M]";
				}

				string qualityMarker = item.Quality.GetColorCode();
				if (item.Item.IsCheapWeapon() && item.Quality == Quality.Unique)
				{
					qualityMarker = "&7";

					if (price != null)
					{
						priceString = qualityMarker + price.Value;
					}
				}

				if (item.Item.IsCurrency())
				{
					qualityMarker = "&f";
					priceString = qualityMarker + item.Item.GetCurrencyPrice();

					totalPure += item.Item.GetCurrencyPrice();
				}
				#endregion formatting

				string prefix = "  ";
				if (item.IsNewToBackpack)
				{
					prefix += "&a[New] ";
				}

				VersatileIO.WriteComplex(prefix + qualityMarker + item + "&7: " + priceString);
			}

			VersatileIO.Info("Net worth: " + netWorth);
			VersatileIO.Info("Total pure: " + totalPure);
		}

		public static Backpack GetBackpack(string id)
		{
			if (id == Settings.Instance.HomeSteamID64)
			{
				return DataManager.MyBackpackData;
			}
			else
			{
				if (!DataManager.BackpackData.ContainsKey(id))
				{
					if (!DataManager.LoadOtherBackpack(id))
					{
						return null;
					}
				}

				return DataManager.BackpackData[id];
			}
		}
	}
}
