using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Items;
using CustomSteamTools.Utils;

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
				LoggerOld.Log("Searching items...");

				List<Item> possibleItems = new List<Item>();
				foreach (Item i in DataManager.Schema.Items)
				{
					if (i.Name.ToLower().Contains(query.ToLower()))
					{
						possibleItems.Add(i);
						LoggerOld.Log("  " + possibleItems.Count.ToString() + ": " + i.Name, ConsoleColor.White);
					}
				}

				if (possibleItems.Count == 0)
				{
					LoggerOld.Log("No items found matching '" + query + "'.", ConsoleColor.Red);
					return null;
				}

				while (item == null)
				{
					if (possibleItems.Count == 1)
					{
						item = possibleItems.First();
						break;
					}

					string sint = LoggerOld.GetInput("Enter selection > ");
					if (sint.ToLower() == "esc")
					{
						LoggerOld.Log("Canceled.");
						return null;
					}

					int n = -1;

					bool worked = int.TryParse(sint, out n);
					if (worked && n > 0 && n <= possibleItems.Count)
					{
						item = possibleItems[n - 1];
					}
					else
					{
						LoggerOld.Log("Invalid choice: " + sint, ConsoleColor.Red);
					}
				}
			}
			#endregion

			return item;
		}
	}
}
