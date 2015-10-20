using BackpackTFPriceLister.BackpackDataJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public static class CommandHandler
	{
		public delegate void Command(List<string> args);
		public delegate bool PreCommandHandler(string name, List<string> args);

		static Dictionary<string, Command> _commands;

		public static event PreCommandHandler PreCommand;

		static CommandHandler()
		{
			_commands = new Dictionary<string, Command>();

			_commands.Add("pc", PriceCheck);
			_commands.Add("pricecheck", PriceCheck);
			_commands.Add("refresh", ForceRefresh);
			_commands.Add("range", GetItemsInPriceRange);
			_commands.Add("bp", BackpackCheck);
			_commands.Add("backpack", BackpackCheck);
			_commands.Add("sellers", (a) => GetClassifieds(a, OrderType.Sell));
			_commands.Add("buyers", (a) => GetClassifieds(a, OrderType.Buy));
		}

		public static void RunCommand(string command, params string[] args)
		{
			List<string> largs = args.ToList();
			if (PreCommand != null)
			{
				bool cancel = PreCommand(command, largs);

				if (cancel)
				{
					Logger.Log("Command canceled.", ConsoleColor.Yellow);
					return;
				}
			}

			if (!_commands.Keys.Contains(command.ToLower()))
			{
				Logger.Log("Command not found.", ConsoleColor.Red);
				return;
			}

			_commands[command.ToLower()](largs); // funky syntax
		}

		// pc {itemname | itemID | searchQuery}
		// pricecheck ...
		public static void PriceCheck(List<string> args)
		{
			if (args.Count == 0)
			{
				Logger.Log("No item specified", ConsoleColor.Red);
			}

			string itemName = "";
			foreach (string s in args)
			{
				itemName += s + " ";
			}

			Item item = SearchItem(itemName.Trim());

			if (item == null)
			{
				return;
			}

			Logger.AddLine();
			Logger.Log(item.Name + " (#" + item.ID.ToString() + ")", ConsoleColor.White);

			List<ItemPricing> itemPricings = PriceLister.PriceData.GetAllPriceData(item);

			if (itemPricings.Count == 0)
			{
				Logger.Log("  No price data found for " + item.Name, ConsoleColor.Red);
				return;
			}

			if (itemPricings.All((p) => !p.Tradable)) // none are tradable
			{
				Logger.Log("  Item not tradable.", ConsoleColor.Red);
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
				Logger.Log("  Enter 'U' to list unusuals.", ConsoleColor.White);
				takeInput = true;
			}
			else
			{
				Logger.Log("  Enter an ID for crate/strangifier information.", ConsoleColor.White);
				takeInput = true;
			}

			if (!takeInput)
			{
				return;
			}

			string input = Logger.GetInput("  Press Enter to continue> ", false, true);

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
						Logger.Log("  No " + item.Name + " found with PriceIndex " + pid.ToString(), ConsoleColor.Red);
						return;
					}

					Logger.Log("  " + item.Name + " #" + pid.ToString() + ": " + p.GetPriceString());
				}
			}
		}

		// buyers {itemname | itemID | searchQuery}
		// sellers {itemname | itemID | searchQuery}
		public static void GetClassifieds(List<string> args, OrderType orderType)
		{
			if (args.Count == 0)
			{
				Logger.Log("No item specified.", ConsoleColor.Red);
				return;
			}

			string query = string.Join(" ", args);
			query = query.Trim();

			Item item = SearchItem(query);
			if (item == null)
			{
				return;
			}

			Logger.Log("Item: " + item.Name, ConsoleColor.White);

			string sQuality = Logger.GetInput("Quality? ");
			Quality quality = ItemQualities.Parse(sQuality);

			bool? tradable = null;
			bool? craftable = null;
			if (quality == Quality.Unique)
			{
				while (craftable == null)
				{
					string sCr = Logger.GetInput("Craftable? ");
					try
					{
						craftable = Util.ParseAdvancedBool(sCr);
					}
					catch (FormatException)
					{
						Logger.Log("  Invalid input: " + sCr);
	                }
				}

				while (tradable == null)
				{
					string sTr = Logger.GetInput("Tradable? ");
					try
					{
						tradable = Util.ParseAdvancedBool(sTr);
					}
					catch (FormatException)
					{
						Logger.Log("  Invalid input: " + sTr);
					}
				}
			}

			if (tradable == null)
			{
				tradable = true;
			}
			if (craftable == null)
			{
				craftable = true;
			}

			string searchedItemInfo = quality.ToString() + " " +
				(tradable.Value ? "" : "Non-Tradable ") +
				(craftable.Value ? "" : "Non-Craftable ") + item.ImproperName;

            Logger.Log("Getting classifieds for " + searchedItemInfo + "...");
			List<ClassifiedsListing> classifieds = ClassifiedsScraper.GetClassifieds(
				item, quality, craftable.Value, tradable.Value);
			classifieds.RemoveAll((c) => c.OrderType != orderType);

			if (classifieds == null || classifieds.Count == 0)
			{
				Logger.Log("No classifieds found for " + searchedItemInfo, ConsoleColor.Red);
				return;
			}

			Price? bestPrice = null;
			string bestLister = null;
			foreach (ClassifiedsListing c in classifieds)
			{
				object[] logLine = new object[] {
					ConsoleColor.Gray,
					c.OrderType == OrderType.Sell ? "[Selling] " : "[Buying]  ",
					ConsoleColor.White, c.Price.ToString(),
					ConsoleColor.Gray, " from ",
					ConsoleColor.White, c.SellerNickname ?? c.SellerSteamID64,
					ConsoleColor.Gray, c.Comment == null ? "" : ": " + c.Comment.SubstringMax(100)
				};

				Logger.LogComplex(logLine);

				if (orderType == OrderType.Buy)
				{
					if (bestPrice == null || c.Price > bestPrice.Value)
					{
						bestPrice = c.Price;
						bestLister = c.SellerNickname ?? "{ NULL }";
					}
				}
				else
				{
					if (bestPrice == null || c.Price < bestPrice.Value)
					{
						bestPrice = c.Price;
						bestLister = c.SellerNickname ?? "{ NULL }";
					}
				}
			}

			List<ItemPricing> relevantPrices = PriceLister.PriceData.GetAllPriceData(item);
			List<ItemPricing> validPrices = new List<ItemPricing>();
			foreach (ItemPricing p in relevantPrices)
			{
				if (p.Craftable != craftable.Value || p.Tradable != tradable.Value)
				{
					continue;
				}

				if (p.Quality != quality)
				{
					continue;
				}

				validPrices.Add(p);
			}

			double avg = 0;
			foreach (ItemPricing p in validPrices)
			{
				avg += (p.PriceHigh.TotalRefined + p.PriceLow.TotalRefined) / 2.0;
			}
			avg /= (double)validPrices.Count;
			Price estimate = new Price(0, avg);
			Logger.Log("Price: " + estimate.ToString());

			Price diff = bestPrice.Value - estimate;
			bool good = true;
			if (orderType == OrderType.Buy)
			{
				good = diff.TotalRefined > 0;
			}
			else
			{
				good = diff.TotalRefined < 0;
				diff = new Price(0, -diff.TotalRefined);
			}

			ConsoleColor goodbadcolor = good ? ConsoleColor.DarkGreen : ConsoleColor.Red;
			Logger.LogComplex(ConsoleColor.White, "Best Deal: ",
				goodbadcolor, bestPrice.ToString(), ConsoleColor.Gray, " from ", 
				ConsoleColor.White, bestLister,
				goodbadcolor, " (" + diff.ToString() + (good ? " better)" : " worse)"));
		}

		// refresh
		public static void ForceRefresh(List<string> args)
		{
			PriceLister.AutoSetup(true, true);
		}

		// range priceMin priceMax [filters...]
		public static void GetItemsInPriceRange(List<string> args)
		{
			if (args.Count < 2)
			{
				Logger.Log("Missing arguments: priceMin, priceMax", ConsoleColor.Red);
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
				Logger.Log("Argument invalid: " + args[0], ConsoleColor.Red);
				return;
			}
			if (!double.TryParse(sMax, out dMax))
			{
				Logger.Log("Argument invalid: " + args[1], ConsoleColor.Red);
				return;
			}

			Price min = kMin ? new Price(dMin, 0) : new Price(0, dMin);
			Price max = kMax ? new Price(dMax, 0) : new Price(0, dMax);

			List<string> sfilters = new List<string>();
			for (int i = 2; i < args.Count; i++)
			{
				sfilters.Add(args[i]);
			}

			List<ItemSlotPlain> allowedSlots = new List<ItemSlotPlain>();
			List<Quality> allowedQualities = new List<Quality>();
			List<PlayerClass> allowedClasses = new List<PlayerClass>();

			bool onlyCraftable = false;
			bool onlyHalloween = false;
			bool noHalloween = false;

			foreach (string s in sfilters)
			{
				if (s.ToLower() == "craftable")
				{
					onlyCraftable = true;
				}

				if (s.ToLower() == "-halloween")
				{
					noHalloween = true;
				}
				else if (s.ToLower() == "halloween")
				{
					onlyHalloween = true;
				}

				ItemSlotPlain bufs = ItemSlots.Plain.Parse(s);
				PlayerClass bufc = PlayerClasses.Parse(s);
				
				if (bufs != ItemSlotPlain.Unused)
				{
					allowedSlots.Add(bufs);
				}
				else if (bufc != 0)
				{
					allowedClasses.Add(bufc);
				}
				else
				{
					allowedQualities.Add(ItemQualities.Parse(s));
				}
			}

			bool filterSlot = allowedSlots.Count != 0;
			bool filterQuality = allowedQualities.Count != 0;
			bool filterClass = allowedClasses.Count != 0;

			if (filterSlot || filterQuality || filterClass || 
				onlyCraftable || onlyHalloween || noHalloween)
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
				foreach (PlayerClass c in allowedClasses)
				{
					res += c.ToString() + " ";
				}

				if (onlyCraftable)
				{
					res += "Craftable ";
				}

				if (onlyHalloween)
				{
					res += "Halloween";
				}
				else if (noHalloween)
				{
					res += "Non-halloween";
				}

				Logger.Log(res, ConsoleColor.White);
			}

			List<ItemPricing> results = new List<ItemPricing>();
			foreach (ItemPricing p in PriceLister.PriceData.Prices)
			{
				if (!p.Tradable)
				{
					continue;
				}

				if (!p.Craftable && onlyCraftable)
				{
					continue;
				}

				if ((!p.Item.HalloweenOnly && p.Item.HasHauntedVersion != true) && onlyHalloween)
				{
					continue;
				}

				if ((p.Item.HalloweenOnly || p.Item.HasHauntedVersion == true) && noHalloween)
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

				if (filterClass && !allowedClasses.Exists((c) => p.Item.ValidClasses.Contains(c)))
				{
					continue;
				}

				if (p.PriceHigh <= max && p.PriceLow >= min)
				{
					results.Add(p);
				}
			}

			Logger.Log("Items in price range " + min.ToString() + " to " + max.ToString() + ": ", ConsoleColor.White);
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
			Logger.Log(results.Count.ToString() + " items found.");
		}

		// bp [steamid64]
		// backpack ...
		public static void BackpackCheck(List<string> args)
		{
			TF2BackpackData backpackData = null;

			if (args.Count == 0)
			{
				Logger.Log("No player specified.", ConsoleColor.Red);
				return;
			}
			
			string id = args[0];
			if (id == PriceLister.SEALEDINTERFACE_STEAMID)
			{
				backpackData = PriceLister.MyBackpackData;
			}
			else
			{
				if (!PriceLister.BackpackData.ContainsKey(id))
				{
					if (!PriceLister.LoadOtherBackpack(id))
					{
						return;
					}
				}

				backpackData = PriceLister.BackpackData[id];
			}
			
			Logger.Log("Backpack for user " + (id == PriceLister.SEALEDINTERFACE_STEAMID ? 
				"'sealed interface'" : "#" + id) + " (" +
				backpackData.SlotCount.ToString() + " slots):", ConsoleColor.White);

			Price lowNetWorth = Price.Zero;
			Price highNetWorth = Price.Zero;

			Price totalPure = Price.Zero;

			foreach (ItemInstance item in backpackData.Items)
			{
				ItemPricing pricing = item.GetPrice();

				if (!item.Tradable)
				{
					Logger.Log("  " + item.ToFullString() + ": Not Tradable", ConsoleColor.DarkGray);
					continue;
				}
				else if (pricing == null)
				{
					Logger.Log("  " + item.ToFullString() + ": Unknown", ConsoleColor.Red);
					continue;
				}
				else if (item.Item.IsCurrency())
				{
					Logger.Log("  " + item.ToFullString() + ": " + item.Item.GetCurrencyPrice().ToString());
					totalPure += item.Item.GetCurrencyPrice();
				}
				else
				{
					ConsoleColor color = ConsoleColor.Gray;
					if (item.Item.PlainSlot != ItemSlotPlain.Weapon || 
						(int)(pricing.PriceLow.TotalRefined * 100.0) != 5) //0.05 ref
					{
						color = ConsoleColor.White;
					}
					if (item.Quality != Quality.Unique)
					{
						color = item.Quality.GetColor();
					}

					Logger.Log("  " + item.ToFullString() + ": " + pricing.GetPriceString(), color);
				}

				lowNetWorth += pricing.PriceLow;
				highNetWorth += pricing.PriceHigh;
			}

			if (lowNetWorth == highNetWorth)
			{
				Logger.Log("Net worth: " + lowNetWorth.ToString(), ConsoleColor.White);
			}
			else
			{
				Logger.Log("Net worth: " + lowNetWorth.ToString() + " - " + highNetWorth.ToString(), ConsoleColor.White);
			}

			Logger.Log("Total pure: " + totalPure.ToString(), ConsoleColor.White);
		}

		// helper function
		public static Item SearchItem(string query)
		{
			int id = -1;
			bool isNum = int.TryParse(query, out id);

			Item item = null;

			// shortcut
			if (query.ToLower() == "key")
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
						if (i.Name.ToLower() == query.ToLower())
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
					if (i.Name.ToLower().Contains(query.ToLower()))
					{
						possibleItems.Add(i);
						Logger.Log("  " + possibleItems.Count.ToString() + ": " + i.Name, ConsoleColor.White);
					}
				}

				if (possibleItems.Count == 0)
				{
					Logger.Log("No items found matching '" + query + "'", ConsoleColor.Red);
					return null;
				}

				while (item == null)
				{
					if (possibleItems.Count == 1)
					{
						item = possibleItems.First();
						break;
					}

					string sint = Logger.GetInput("Enter selection > ");
					if (sint.ToLower() == "esc")
					{
						Logger.Log("Canceled.");
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
						Logger.Log("Invalid choice: " + sint, ConsoleColor.Red);
					}
				}
			}
			#endregion

			return item;
		}
	}
}
