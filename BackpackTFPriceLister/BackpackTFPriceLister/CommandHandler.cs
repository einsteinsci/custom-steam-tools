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
			_commands.Add("refresh", ForceRefresh);
			_commands.Add("range", GetItemsInPriceRange);
			_commands.Add("bp", BackpackCheck);
			_commands.Add("backpack", BackpackCheck);
		}

		public static void RunCommand(string command, params string[] args)
		{
			if (!_commands.Keys.Contains(command.ToLower()))
			{
				Logger.Log("Command not found.", MessageType.Error);
				return;
			}

			_commands[command.ToLower()](args); // funky syntax
		}

		// pc {itemname | itemID | searchQuery}
		// pricecheck ...
		public static void PriceCheck(params string[] args)
		{
			if (args.Length == 0)
			{
				Logger.Log("No item specified", MessageType.Error);
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

			// shortcut
			if (itemName.ToLower() == "key")
			{
				item = PriceLister.ItemData.GetItem(5021);
			}
			else
			{
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
						Logger.Log("  " + possibleItems.Count.ToString() + ": " + i.Name, MessageType.Emphasis);
					}
				}

				if (possibleItems.Count == 0)
				{
					Logger.Log("No items found matching '" + itemName + "'", MessageType.Error);
					return;
				}

				while (item == null)
				{
					string sint = Logger.GetInput("Enter selection > ");
					if (sint.ToLower() == "esc")
					{
						Logger.Log("Canceled.");
						return;
					}

					int n = -1;

					bool worked = int.TryParse(sint, out n);
					if (worked && n > 0 && n <= possibleItems.Count)
					{
						item = possibleItems[n - 1];
					}
					else
					{
						Logger.Log("Invalid choice: " + sint, MessageType.Error);
					}
				}
			}
			#endregion

			Logger.AddLine();
			Logger.Log(item.Name + " (#" + item.ID.ToString() + ")", MessageType.Emphasis);

			List<ItemPricing> itemPricings = PriceLister.PriceData.GetAllPriceData(item);

			if (itemPricings.Count == 0)
			{
				Logger.Log("  No price data found for " + item.Name, MessageType.Error);
				return;
			}

			if (itemPricings.All((p) => !p.Tradable)) // none are tradable
			{
				Logger.Log("  Item not tradable.", MessageType.Error);
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
				Logger.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString());
			}
			if (uniques.Count <= 2)
			{
				foreach (ItemPricing p in uniques)
				{
					Logger.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString());
				}
			}
			if (unusuals.Count > 0)
			{
				Logger.Log("  Enter 'U' to list unusuals.", MessageType.Emphasis);
				takeInput = true;
			}
			else
			{
				Logger.Log("  Enter an ID for crate/strangifier information.", MessageType.Emphasis);
				takeInput = true;
			}

			if (!takeInput)
			{
				return;
			}

			string input = Logger.GetInput("  Press Enter to continue> ");

			if (input.ToLower() == "esc")
			{
				Logger.Log("Canceled.");
				return;
			}
			else if (input.ToLower() == "u")
			{
				foreach (ItemPricing p in unusuals)
				{
					UnusualEffect fx = PriceLister.ItemData.Unusuals.First((ue) => ue.ID == p.PriceIndex);
					Logger.Log("  " + fx.Name + " (#" + fx.ID + "): " + p.GetPriceString());
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
						Logger.Log("  No " + item.Name + " found with PriceIndex " + pid.ToString(), MessageType.Error);
						return;
					}

					Logger.Log("  " + item.Name + " #" + pid.ToString() + ": " + p.GetPriceString());
				}
			}
		}

		// refresh
		public static void ForceRefresh(params string[] args)
		{
			PriceLister.AutoSetup(true, true);
		}

		// range priceMin priceMax [filters...]
		public static void GetItemsInPriceRange(params string[] args)
		{
			if (args.Length < 2)
			{
				Logger.Log("Missing arguments: priceMin, priceMax", MessageType.Error);
				return;
			}

			string sMin = args[0].ToLower();
			string sMax = args[1].ToLower();

			bool kMin = sMin.EndsWith("k");
			bool kMax = sMax.EndsWith("k");

			sMin = sMin.TrimEnd('k');
			sMax = sMax.TrimEnd('k');

			double dMin = -1, dMax = -1;
			if (!double.TryParse(sMin, out dMin))
			{
				Logger.Log("Argument invalid: " + args[0], MessageType.Error);
				return;
			}
			if (!double.TryParse(sMax, out dMax))
			{
				Logger.Log("Argument invalid: " + args[1], MessageType.Error);
				return;
			}

			Price min = kMin ? new Price(dMin, 0) : new Price(0, dMin);
			Price max = kMax ? new Price(dMax, 0) : new Price(0, dMax);

			List<string> sfilters = new List<string>();
			for (int i = 2; i < args.Length; i++)
			{
				sfilters.Add(args[i]);
			}

			List<ItemSlotPlain> allowedSlots = new List<ItemSlotPlain>();
			List<Quality> allowedQualities = new List<Quality>();
			foreach (string s in sfilters)
			{
				ItemSlotPlain buf = ItemSlots.Plain.Parse(s);
				
				if (buf != ItemSlotPlain.Unused)
				{
					allowedSlots.Add(buf);
				}
				else
				{
					allowedQualities.Add(ItemQualities.Parse(s));
				}
			}

			bool filterSlot = allowedSlots.Count != 0;
			bool filterQuality = allowedQualities.Count != 0;

			if (filterSlot || filterQuality)
			{
				string res = "Filters: ";
				foreach (ItemSlotPlain s in allowedSlots)
				{
					res += s.ToString() + " ";
				}
				foreach (Quality q in allowedQualities)
				{
					res += q.ToReadableString() + " ";
				}

				Logger.Log(res, MessageType.Emphasis);
			}

			List<ItemPricing> results = new List<ItemPricing>();
			foreach (ItemPricing p in PriceLister.PriceData.Prices)
			{
				if (!p.Tradable)
				{
					continue;
				}

				if (filterSlot && !allowedSlots.Contains(p.Item.PlainSlot))
				{
					continue;
				}

				if (filterQuality && !allowedQualities.Contains(p.Quality))
				{
					continue;
				}

				if (p.PriceHigh <= max && p.PriceLow >= min)
				{
					results.Add(p);
				}
			}

			Logger.Log("Items in price range " + min.ToString() + " to " + max.ToString() + ": ", MessageType.Emphasis);
			foreach (ItemPricing p in results)
			{
				if (p.Quality == Quality.Unusual)
				{
					UnusualEffect fx = PriceLister.ItemData.Unusuals.First((u) => u.ID == p.PriceIndex);
					Logger.Log("  " + p.CompiledTitleName + " (" + fx.Name + "): " + p.GetPriceString());
					continue;
				}

				Logger.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString());
			}
		}

		// bp
		// backpack ...
		public static void BackpackCheck(params string[] args)
		{
			Logger.Log("Backpack for 'sealed interface' (" + 
				PriceLister.BackpackData.SlotCount.ToString() + " slots):", MessageType.Emphasis);

			Price lowNetWorth = Price.Zero;
			Price highNetWorth = Price.Zero;

			Price totalPure = Price.Zero;

			foreach (ItemInstance item in PriceLister.BackpackData.Items)
			{
				ItemPricing pricing = item.GetPrice();

				if (!item.Tradable)
				{
					Logger.Log("  " + item.ToFullString() + ": Not Tradable", MessageType.Debug);
					continue;
				}
				else if (pricing == null)
				{
					Logger.Log("  " + item.ToFullString() + ": Unknown", MessageType.Error);
					continue;
				}
				else if (item.Item.IsCurrency())
				{
					Logger.Log("  " + item.ToFullString() + ": " + item.Item.GetCurrencyPrice().ToString());
					totalPure += item.Item.GetCurrencyPrice();
				}
				else
				{
					Logger.Log("  " + item.ToFullString() + ": " + pricing.GetPriceString(), 
						item.Item.PlainSlot != ItemSlotPlain.Weapon || item.Quality != Quality.Unique ? 
						MessageType.Emphasis : MessageType.Normal);
				}

				lowNetWorth += pricing.PriceLow;
				highNetWorth += pricing.PriceHigh;
			}

			if (lowNetWorth == highNetWorth)
			{
				Logger.Log("Net worth: " + lowNetWorth.ToString(), MessageType.Emphasis);
			}
			else
			{
				Logger.Log("Net worth: " + lowNetWorth.ToString() + " - " + highNetWorth.ToString());
			}

			Logger.Log("Total pure: " + totalPure.ToString());
		}
	}
}
