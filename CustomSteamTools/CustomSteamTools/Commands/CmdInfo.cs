using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdInfo : ITradeCommand
	{
		public string[] Aliases => new string[] { "info", "item" };

		public string Description => "Lists info about an item.";

		public string RegistryName => "info";

		public string Syntax => "info {defindex | itemName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.Count == 0)
			{
				VersatileIO.WriteLine("Usage: " + Syntax, ConsoleColor.Red);
				return;
			}

			string query = string.Join(" ", args);
			Item item = SearchItem(query);

			if (item == null)
			{
				return;
			}

			VersatileIO.WriteLine(item.ToString(), ConsoleColor.White);
			VersatileIO.WriteLine(" - Description: " + item.Description?.Shorten(120).Replace('\n', ' ') ?? "", ConsoleColor.Gray);
			VersatileIO.WriteLine(" - Defindex: " + item.ID, ConsoleColor.Gray);
			VersatileIO.WriteLine(" - Slot: {0} ({1})".Fmt(item.PlainSlot, item.Slot), ConsoleColor.Gray);
			VersatileIO.WriteLine(" - Classes: " + item.ValidClasses.ToReadableString(includeBraces: false));
			VersatileIO.WriteLine(" - " + item.GetSubtext());
			VersatileIO.WriteComplex(" - Default Quality: {0}" + item.DefaultQuality.ToString(), item.DefaultQuality.GetColor());
			if (!item.Styles.IsNullOrEmpty())
			{
				VersatileIO.WriteLine(" - Styles: " + item.Styles.ToReadableString(includeBraces: false));
			}
			if (item.CanBeAustralium())
			{
				VersatileIO.WriteLine(" - Can be Australium", ConsoleColor.Yellow);
			}
			if (item.IsCheapWeapon())
			{
				VersatileIO.WriteLine(" - Drop weapon", ConsoleColor.Gray);
			}
			if (item.HalloweenOnly || item.HasHauntedVersion == true)
			{
				VersatileIO.WriteLine(" - Halloween only", ConsoleColor.Cyan);
			}
		}

		public static Item SearchItem(string query)
		{
			int id = -1;
			bool isNum = int.TryParse(query, out id);

			Item item = null;

			#region lookup
			// shortcut
			if (query.ToLower() == "key")
			{
				item = DataManager.Schema.GetItem(5021);
			}
			else
			{
				foreach (Item i in DataManager.Schema.Items)
				{
					if (isNum)
					{
						if (i.ID == id)
						{
							item = i;
							break;
						}
					}
					else
					{
						if (i.Name.ToLower() == query.ToLower())
						{
							item = i;
							break;
						}
					}
				}
			}
			#endregion lookup

			#region search
			if (item == null)
			{
				VersatileIO.WriteLine("Searching items...", ConsoleColor.Gray);

				List<Item> possibleItems = DataManager.Schema.Items.FindAll(
					(i) => i.Name.ContainsIgnoreCase(query) || i.UnlocalizedName.ContainsIgnoreCase(query));

				if (possibleItems.Count == 0)
				{
					VersatileIO.WriteLine("No items found matching '{0}'.".Fmt(query), ConsoleColor.Red);
					return null;
				}

				int index = -1;
				if (possibleItems.Count == 1)
				{
					index = 0;
				}
				else
				{
					index = VersatileIO.GetSelection("Select an item: ", possibleItems.ConvertAll((i) => i.Name));
				}

				item = possibleItems[index];
			}
			#endregion

			return item;
		}
	}
}
