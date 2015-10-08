using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class CommandHandler
	{
		internal delegate void Command(params string[] args);

		static Dictionary<string, Command> _commands;

		static CommandHandler()
		{
			_commands = new Dictionary<string, Command>();

			_commands.Add("pc", PriceCheck);
			_commands.Add("pricecheck", PriceCheck);
		}

		public static void RunCommand(string command, params string[] args)
		{
			_commands[command.ToLower()](args); // funky syntax
		}

		public static void PriceCheck(params string[] args)
		{
			if (args.Length == 0)
			{
				Logger.Log("No item specified", true);
			}

			string itemName = "";
			foreach (string s in args)
			{
				itemName += s + " ";
			}
			itemName = itemName.Trim();
			int id = -1;
			bool isNum = int.TryParse(itemName, out id);

			Item item = null;
			foreach (Item i in PriceLister.ItemData.Items)
			{
				if (isNum)
				{
					if (i.ID == id)
					{
						item = i;
						break;
					}
					continue;
				}
				else
				{
					if (i.Name.ToLower() == itemName.ToLower())
					{
						item = i;
						break;
					}
					continue;
				}
			}

			#region search
			if (item == null)
			{
				Logger.Log("Searching items...");

				List<Item> possibleItems = new List<Item>();
				foreach (Item i in PriceLister.ItemData.Items)
				{
					if (i.Name.ToLower().Contains(itemName.ToLower()))
					{
						possibleItems.Add(i);
						Logger.Log("  " + possibleItems.Count.ToString() + ": " + i.Name);
					}
				}

				if (possibleItems.Count == 0)
				{
					Logger.Log("No items found matching '" + itemName + "'", true);
					return;
				}

				while (item == null)
				{
					string sint = Logger.GetInput("Enter selection > ");
					int n = -1;

					bool worked = int.TryParse(sint, out n);
					if (worked && n > 0 && n <= possibleItems.Count)
					{
						item = possibleItems[n - 1];
					}
					else
					{
						Logger.Log("Invalid choice: " + sint, true);
					}
				}
			}
			#endregion

			Logger.AddLine();
			Logger.Log(item.Name + " (#" + item.ID.ToString() + ")");

			List<ItemPricing> itemPricings = PriceLister.PriceData.GetAllPriceData(item);

			if (itemPricings.Count == 0)
			{
				Logger.Log("  No price data found for " + item.Name, true);
				return;
			}

			if (itemPricings.All((p) => !p.Tradable)) // none are tradable
			{
				Logger.Log("  Item not tradable.", true);
				return;
			}

			List<ItemPricing> unusuals = new List<ItemPricing>();
			List<ItemPricing> uniques = new List<ItemPricing>();
			List<ItemPricing> others = new List<ItemPricing>();

			foreach (ItemPricing p in itemPricings)
			{
				if (!p.Tradable || p.IsChemistrySet)
				{
					continue;
				}

				if (p.Quality == Quality.Unique)
				{
					uniques.Add(p);
				}
				else if (p.Quality == Quality.Unusual)
				{
					unusuals.Add(p);
				}
				else
				{
					others.Add(p);
				}
			}

			bool takeInput = false;
			foreach (ItemPricing p in others)
			{
				string informalName = p.Item.Name;
				if (p.Quality.ToReadableString() != "")
				{
					informalName = p.Quality.ToReadableString() + " " + informalName;
				}

				Logger.Log(informalName + ": " + p.PriceString());
			}
			if (unusuals.Count > 0)
			{
				Logger.Log("Enter 'U' to list unusuals.");
				takeInput = true;
			}
			if (uniques.Count <= 2)
			{
				foreach (ItemPricing p in uniques)
				{
					string informal = p.Craftable ? p.Item.Name : "Non-Craftable " + p.Item.Name;
					Logger.Log(informal + ": " + p.PriceString());
				}
			}
			else
			{
				Logger.Log("Enter an ID for crate/strangifier information.");
				takeInput = true;
			}

			if (!takeInput)
			{
				return;
			}

			string input = Logger.GetInput("> ");
			if (input.ToLower() == "u")
			{
				foreach (ItemPricing p in unusuals)
				{
					UnusualEffect fx = PriceLister.ItemData.Unusuals.First((ue) => ue.ID == p.PriceIndex);
					Logger.Log(fx.Name + "(#" + fx.ID + "): " + p.PriceString());
				}
			}
			else
			{
				int pid = -1;
				if (int.TryParse(input, out pid))
				{
					ItemPricing p = uniques.FirstOrDefault((_p) => _p.PriceIndex == pid);
					if (p == null)
					{
						Logger.Log("No " + item.Name + " found with PriceIndex " + pid.ToString(), true);
						return;
					}

					Logger.Log(item.Name + " #" + pid.ToString() + ": " + p.PriceString());
				}
			}
		}
	}
}
