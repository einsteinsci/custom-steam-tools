using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Utils;
using CustomSteamTools.Items;
using CustomSteamTools.Lookup;
using CustomSteamTools.Market;
using CustomSteamTools.Classifieds;
using System.IO;
using CustomSteamTools.Commands;
using UltimateUtil;

namespace CustomSteamTools
{
	[Obsolete("Use CommandHandler instead")]
	public static class CommandHandlerOld
	{
		public delegate void Command(List<string> args);

		static Dictionary<string, Command> _commands;

		public static event CommandHandler.PreCommandHandler PreCommand;

		static CommandHandlerOld()
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
			_commands.Add("deals", GetDeals);
			_commands.Add("skin", PriceSkin);
			_commands.Add("ks", PriceKillstreak);
			_commands.Add("weapons", FindWeapons);
		}

		public static void RunCommand(string command, params string[] args)
		{
			List<string> largs = args.ToList();
			if (PreCommand != null)
			{
				bool cancel = PreCommand(command, new PreCommandArgs(command, null, largs));

				if (cancel)
				{
					LoggerOld.Log("Command canceled.", ConsoleColor.Yellow);
					return;
				}
			}

			if (!_commands.Keys.Contains(command.ToLower()))
			{
				LoggerOld.Log("Command not found.", ConsoleColor.Red);
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
				LoggerOld.Log("No item specified", ConsoleColor.Red);
			}

			string itemName = "";
			foreach (string s in args)
			{
				itemName += s + " ";
			}

			Item item = CmdInfo.SearchItem(itemName.Trim());

			if (item == null)
			{
				return;
			}
			
			LoggerOld.Log(item.Name + " (#" + item.ID.ToString() + ")", ConsoleColor.White);

			List<ItemPricing> itemPricings = DataManager.PriceData.GetAllPriceData(item);

			if (itemPricings.Count == 0)
			{
				LoggerOld.Log("  No price data found for " + item.Name, ConsoleColor.Red);
				return;
			}

			if (itemPricings.All((p) => !p.Tradable)) // none are tradable
			{
				LoggerOld.Log("  Item not tradable.", ConsoleColor.Red);
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

			if (uniques.Count <= 2)
			{
				foreach (ItemPricing p in uniques)
				{
					LoggerOld.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString(), ConsoleColor.Yellow);
				}
			}
			else
			{
				LoggerOld.Log("  Enter an ID for crate/strangifier information.", ConsoleColor.White);
			}

			foreach (ItemPricing p in others)
			{
				LoggerOld.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString(), p.Quality.GetColor());
			}

			if (unusuals.Count > 0)
			{
				LoggerOld.Log("  Enter 'U' to list unusuals.", ConsoleColor.White);
				takeInput = true;
			}

			if (!takeInput)
			{
				return;
			}

			string input = LoggerOld.GetInput("  Press Enter to continue> ", false, true);

			if (input.IsNullOrEmpty() || input.ToLower() == "esc")
			{
				return;
			}
			else if (input.ToLower() == "u")
			{
				foreach (ItemPricing p in unusuals)
				{
					UnusualEffect fx = DataManager.Schema.Unusuals.First((ue) => ue.ID == p.PriceIndex);
					LoggerOld.Log("  " + fx.Name + " (#" + fx.ID + "): " + p.GetPriceString(), ConsoleColor.DarkMagenta);
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
						LoggerOld.Log("  No " + item.Name + " found with PriceIndex " + pid.ToString(), ConsoleColor.Red);
						return;
					}

					LoggerOld.Log("  " + item.Name + " #" + pid.ToString() + ": " + p.GetPriceString());
				}
			}
		}

		// buyers [/verify] {itemname | itemID | searchQuery}
		// sellers [/verify] {itemname | itemID | searchQuery}
		public static void GetClassifieds(List<string> args, OrderType orderType)
		{
			if (args.Count == 0)
			{
				LoggerOld.Log("No item specified.", ConsoleColor.Red);
				return;
			}

			bool verify = false;
			if (args[0].ToLower() == "/verify" || args[0].ToLower() == "/v")
			{
				args.RemoveAt(0);
				verify = true;

				if (args.Count == 0)
				{
					LoggerOld.Log("No item specified.", ConsoleColor.Red);
					return;
				}
			}

			string query = string.Join(" ", args);
			query = query.Trim();

			Item item = CmdInfo.SearchItem(query);
			if (item == null)
			{
				return;
			}

			LoggerOld.Log("Item: " + item.Name, ConsoleColor.White);

			List<ItemPricing> relatedPricings = DataManager.PriceData.GetAllPriceData(item);
			Quality _testQ = relatedPricings.First().Quality;

			Quality quality = _testQ;
			if (!relatedPricings.All((p) => p.Quality == _testQ))
			{
				string sQuality = LoggerOld.GetInput("Quality? ");
				quality = ItemQualities.Parse(sQuality);
			}

			bool? australium = false;
			if (relatedPricings.Exists((p) => p.Australium))
			{
				australium = null;
				while (australium == null)
				{
					string sA = LoggerOld.GetInput("Australium? ");
					try
					{
						australium = BooleanUtil.ParseLoose(sA);
					}
					catch (FormatException)
					{
						LoggerOld.Log("  Invalid input: " + sA);
					}
				}
			}

			bool? tradable = true;
			bool? craftable = true;
			if (quality == Quality.Unique && !australium.Value)
			{
				if (relatedPricings.Exists((p) => !p.Craftable))
				{
					craftable = null;
					while (craftable == null)
					{
						string sCr = LoggerOld.GetInput("Craftable? ");
						try
						{
							craftable = BooleanUtil.ParseLoose(sCr);
						}
						catch (FormatException)
						{
							LoggerOld.Log("  Invalid input: " + sCr);
						}
					}
				}

				if (relatedPricings.Exists((p) => !p.Tradable))
				{
					tradable = null;
					while (tradable == null)
					{
						string sTr = LoggerOld.GetInput("Tradable? ");
						try
						{
							tradable = BooleanUtil.ParseLoose(sTr);
						}
						catch (FormatException)
						{
							LoggerOld.Log("  Invalid input: " + sTr);
						}
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
				(craftable.Value ? "" : "Non-Craftable ") +
				(tradable.Value ? "" : "Non-Tradable ") +
				(australium.Value ? "Australium " : "") + item.ImproperName;

			LoggerOld.Log("Getting classifieds for " + searchedItemInfo + "...");
			List<ClassifiedsListing> classifieds = ClassifiedsScraper.GetClassifieds(
				item, quality, craftable.Value, tradable.Value, australium.Value);
			classifieds.RemoveAll((c) => c.OrderType != orderType);

			if (classifieds == null || classifieds.Count == 0)
			{
				LoggerOld.Log("No classifieds found for " + searchedItemInfo, ConsoleColor.Red);
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
					ConsoleColor.White, c.ListerNickname ?? c.ListerSteamID64,
					ConsoleColor.Gray, c.Comment == null ? "" : ": " + c.Comment.Shorten(100)
				};

				LoggerOld.LogComplex(logLine);

				if (orderType == OrderType.Buy)
				{
					if (bestPrice == null || c.Price > bestPrice.Value)
					{
						bestPrice = c.Price;
						bestLister = c.ListerNickname ?? "{ NULL }";
					}
				}
				else
				{
					if (bestPrice == null || c.Price < bestPrice.Value)
					{
						bestPrice = c.Price;
						bestLister = c.ListerNickname ?? "{ NULL }";
					}
				}
			}

			List<ItemPricing> relevantPrices = DataManager.PriceData.GetAllPriceData(item);
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

				if (p.Australium != australium.Value)
				{
					continue;
				}

				validPrices.Add(p);
			}

			double avg = 0;
			foreach (ItemPricing p in validPrices)
			{
				avg += p.Pricing.Mid.TotalRefined;
			}
			avg /= (double)validPrices.Count;
			Price estimate = new Price(0, avg);
			LoggerOld.LogComplex(ConsoleColor.Green, "Price: ", ConsoleColor.White, estimate.ToString());

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
			LoggerOld.LogComplex(ConsoleColor.White, "Best Deal: ",
				goodbadcolor, bestPrice.ToString(), ConsoleColor.Gray, " from ", 
				ConsoleColor.White, bestLister,
				goodbadcolor, " (" + diff.ToString() + (good ? " better)" : " worse)"));
		}

		// refresh
		public static void ForceRefresh(List<string> args)
		{
			DataManager.AutoSetup(true);
			ClassifiedsScraper.SteamBackpackDown = false;

			DataManager.BackpackCaches.Clear();
			DataManager.BackpackDataRaw.Clear();
			DataManager.BackpackData.Clear();
		}

		// skin {skinname}
		public static void PriceSkin(List<string> args)
		{
			if (args.Count == 0)
			{
				LoggerOld.Log("No skin name given.", ConsoleColor.Red);
				return;
			}

			string query = string.Join(" ", args);

			Skin skin = null;
			foreach (Skin s in GunMettleSkins.Skins)
			{
				if (s.Name.ToLower() == query.ToLower())
				{
					skin = s;
					break;
				}
			}

			#region search
			if (skin == null)
			{
				LoggerOld.Log("Searching skins...");

				List<Skin> possibleSkins = new List<Skin>();
				foreach (Skin s in GunMettleSkins.Skins)
				{
					if (s.Name.ToLower().Contains(query.ToLower()))
					{
						possibleSkins.Add(s);
						LoggerOld.Log("  " + possibleSkins.Count.ToString() + ": " + s.Name, ConsoleColor.White);
					}
				}

				if (possibleSkins.Count == 0)
				{
					LoggerOld.Log("No skins found matching '" + query + "'.", ConsoleColor.Red);
					return;
				}

				while (skin == null)
				{
					if (possibleSkins.Count == 1)
					{
						skin = possibleSkins.First();
						break;
					}

					string sint = LoggerOld.GetInput("Enter Selection >");
					if (sint.ToLower() == "esc")
					{
						LoggerOld.Log("Search Cancelled.");
						return;
					}

					int n = -1;
					bool worked = int.TryParse(sint, out n);
					if (worked && n > 0 && n <= possibleSkins.Count)
					{
						skin = possibleSkins[n];
					}
					else
					{
						LoggerOld.Log("Invalid choice: " + sint + ". Try again.", ConsoleColor.Red);
					}
				}
			}
			#endregion search

			List<MarketPricing> prices = new List<MarketPricing>();
			for (SkinWear w = SkinWear.FactoryNew; w <= SkinWear.BattleScarred; w++)
			{
				string hash = skin.GetMarketHash(w);
				MarketPricing res = DataManager.MarketPrices.GetPricing(hash);

				if (res != null)
				{
					prices.Add(res);
				}
			}

			LoggerOld.Log("Prices found for '" + skin.Name + "':", ConsoleColor.White);
			foreach (MarketPricing p in prices)
			{
				LoggerOld.Log("  " + p.Wear.Value.ToReadableString() + ": " + p.Price.ToString());
			}
		}

		// range {priceMin} {priceMax} [filters...]
		public static void GetItemsInPriceRange(List<string> args)
		{
			if (args.Count < 2)
			{
				LoggerOld.Log("Missing arguments: priceMin, priceMax", ConsoleColor.Red);
				return;
			}

			Price min, max;
			if (!Price.TryParse(args[0], out min))
			{
				LoggerOld.Log("Invalid priceMin: " + args[0], ConsoleColor.Red);
				return;
			}
			if (!Price.TryParse(args[1], out max))
			{
				LoggerOld.Log("Invalid priceMax: " + args[1], ConsoleColor.Red);
				return;
			}

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
			bool noAllClass = false;

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
				else if (s.ToLower() == "-allclass")
				{
					noAllClass = true;
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
				onlyCraftable || onlyHalloween || noHalloween || noAllClass)
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

				if (noAllClass)
				{
					res += "Limited classes: ";

					foreach (PlayerClass c in allowedClasses)
					{
						res += c.ToString() + " ";
					}
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

				LoggerOld.Log(res, ConsoleColor.White);
			}

			List<ItemPricing> results = new List<ItemPricing>();
			foreach (ItemPricing p in DataManager.PriceData.Prices)
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

				if (filterClass && noAllClass && !p.Item.ValidClasses.IsAllClass())
				{
					continue;
				}

				if (p.Pricing.Contains(new PriceRange(min, max)))
				{
					results.Add(p);
				}
			}

			LoggerOld.Log("Items in price range " + min.ToString() + " to " + max.ToString() + ": ", ConsoleColor.White);
			foreach (ItemPricing p in results)
			{
				if (p.Quality == Quality.Unusual)
				{
					UnusualEffect fx = DataManager.Schema.Unusuals.First((u) => u.ID == p.PriceIndex);
					LoggerOld.Log("  " + p.CompiledTitleName + " (" + fx.Name + "): " + p.GetPriceString());
					continue;
				}

				LoggerOld.Log("  " + p.CompiledTitleName + ": " + p.GetPriceString());
			}
			LoggerOld.Log(results.Count.ToString() + " items found.");
		}

		// ks {itemname}
		public static void PriceKillstreak(List<string> args)
		{
			if (args.Count == 0)
			{
				LoggerOld.Log("No item name provided.", ConsoleColor.Red);
				return;
			}

			string query = string.Join(" ", args);
			Item item = CmdInfo.SearchItem(query);
			if (item == null)
			{
				// logging already done
				return;
			}

			if (item.PlainSlot != ItemSlotPlain.Weapon)
			{
				LoggerOld.Log("Item must be a weapon.", ConsoleColor.Red);
				return;
			}

			LoggerOld.Log("Item: " + item.Name, ConsoleColor.White);

			Quality? q = null;
			while (q == null)
			{
				string s = LoggerOld.GetInput("Quality? ");
				q = ItemQualities.ParseNullable(s);

				if (q == null)
				{
					LoggerOld.Log("  Invalid Quality: " + s, ConsoleColor.Red);
				}
			}

			KillstreakType ks = KillstreakType.None;
			while (ks == KillstreakType.None)
			{
				string s = LoggerOld.GetInput("Killstreak type (basic/specialized/professional)? ");
				ks = KillstreakTypes.Parse(s);

				if (ks == KillstreakType.None)
				{
					LoggerOld.Log("  Invalid Killstreak type: " + s, ConsoleColor.Red);
				}
			}

			List<MarketPricing> pricings = new List<MarketPricing>();
			foreach (MarketPricing p in DataManager.MarketPrices.Pricings)
			{
				if (p.Item != item)
				{
					continue;
				}

				if (p.Quality != q.Value)
				{
					continue;
				}

				if (p.Killstreak != ks)
				{
					continue;
				}

				pricings.Add(p);
				break;
			}

			string qstr = q.Value.ToReadableString();
			LoggerOld.Log("Pricings for " + qstr + (qstr != "" ? " " : "") +
				ks.ToReadableString() + " " + item.Name + ":", ConsoleColor.White);
			foreach (MarketPricing p in pricings)
			{
				LoggerOld.Log("  " + p.MarketHash + ": " + p.Price.ToString());
			}
		}

		// bp [steamid64]
		// backpack ...
		public static void BackpackCheck(List<string> args)
		{
			Backpack backpackData = null;

			if (args.Count == 0)
			{
				LoggerOld.Log("No player specified.", ConsoleColor.Red);
				return;
			}
			
			string id = args[0];
			if (id == Settings.Instance.HomeSteamID64)
			{
				backpackData = DataManager.MyBackpackData;
			}
			else
			{
				if (!DataManager.BackpackData.ContainsKey(id))
				{
					if (!DataManager.LoadOtherBackpack(id))
					{
						return;
					}
				}

				backpackData = DataManager.BackpackData[id];
			}

			bool skipWeapons = false;
			if (args.Count > 1)
			{
				for (int i = 1; i < args.Count; i++)
				{
					if (args[i].ToLower() == "-weapons")
					{
						skipWeapons = true;
					}
				}
			}
			
			LoggerOld.Log("Backpack for user " + (id == Settings.Instance.HomeSteamID64 ? 
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
					if (item.Item.PlainSlot != ItemSlotPlain.Weapon || !skipWeapons)
					{
						LoggerOld.Log("  " + item.ToFullString() + ": Not Tradable", ConsoleColor.DarkGray);
					}

					continue;
				}
				else if (pricing == null)
				{
					string hash = "";

					if (item.Item.IsSkin())
					{
						Skin skin = item.Item.GetSkin();
						hash = skin.GetMarketHash(item.GetSkinWear());
					}
					else
					{
						hash = MarketPricing.GetMarketHash(item.Item, item.GetKillstreak(), item.Quality);
					}

					MarketPricing market = DataManager.MarketPrices.GetPricing(hash);

					if (market != null)
					{
						LoggerOld.Log("  [M] " + hash + ": " + market.Price.ToString(), item.Quality.GetColor());
						continue;
					}
					else
					{
						LoggerOld.Log("  " + item.ToFullString() + ": Unknown", ConsoleColor.Red);
						continue;
					}
				}
				else if (item.Item.IsCurrency())
				{
					LoggerOld.Log("  " + item.ToFullString() + ": " + item.Item.GetCurrencyPrice().ToString(), ConsoleColor.Yellow);
					totalPure += item.Item.GetCurrencyPrice();
				}
				else
				{
					ConsoleColor color = ConsoleColor.Gray;
					if (!item.Item.IsCheapWeapon())
					{
						color = ConsoleColor.White;
					}
					else if (skipWeapons)
					{
						continue;
					}

					if (item.Quality != Quality.Unique)
					{
						color = item.Quality.GetColor();
					}

					LoggerOld.Log("  " + item.ToFullString() + ": " + pricing.GetPriceString(), color);
				}

				lowNetWorth += pricing.Pricing.Low;
				highNetWorth += pricing.Pricing.High;
			}

			if (lowNetWorth == highNetWorth)
			{
				LoggerOld.Log("Net worth: " + lowNetWorth.ToString(), ConsoleColor.White);
			}
			else
			{
				LoggerOld.Log("Net worth: " + lowNetWorth.ToString() + " - " + highNetWorth.ToString(), ConsoleColor.White);
			}

			LoggerOld.Log("Total pure: " + totalPure.ToString(), ConsoleColor.White);
		}

		// deals {steamid64} [filters...]
		public static void GetDeals(List<string> args)
		{
			if (args.Count == 0)
			{
				LoggerOld.Log("SteamID is required (64-bit form).", ConsoleColor.Red);
				return;
			}

			string steamID = args[0];
			ulong nothing;
			if (!ulong.TryParse(steamID, out nothing))
			{
				LoggerOld.Log("Invalid SteamID64: " + steamID, ConsoleColor.Red);
				return;
			}

			LoggerOld.Log("Searching deals for user #" + steamID + "...", ConsoleColor.White);
			
			List<string> filters = new List<string>();
			for (int i = 1; i < args.Count; i++)
			{
				filters.Add(args[i]);
			}

			#region filters
			List<object> buf = new List<object>();
			foreach (string s in filters)
			{
				ItemSlotPlain bufs = ItemSlots.Plain.Parse(s);
				Quality? bufq = ItemQualities.ParseNullable(s);
				//PlayerClass bufc = PlayerClasses.Parse(s);

				if (bufs != ItemSlotPlain.Unused)
				{
					buf.Add(bufs);
				}
				//else if (bufc != 0)
				//{
				//	allowedClasses.Add(bufc);
				//}
				else if (bufq != null)
				{
					buf.Add(bufq.Value);
				}
				else
				{
					buf.Add(s);
				}
			}

			object[] filtersRes = buf.ToArray();
			#endregion filters

			List<ItemSale> res = DealFinder.FindDeals(steamID, null);
			if (res == null) // failed
			{
				return;
			}

			res.Sort((a, b) => a.Profit.TotalRefined.CompareTo(b.Profit.TotalRefined));

			LoggerOld.AddLine();
			LoggerOld.Log(res.Count.ToString() + " deals found:", ConsoleColor.White);
			foreach (ItemSale sale in res)
			{
				LoggerOld.LogComplex(sale.ToComplexOutputOld("  "));
			}

			Console.Beep();
		}

		// weapons [filters...]
		public static void FindWeapons(List<string> args)
		{
			List<Item> weapons = DataManager.Schema.Items.FindAll((i) => i.PlainSlot == ItemSlotPlain.Weapon);

			List<ItemSlot> validWeaponSlots = new List<ItemSlot>();
			List<PlayerClass> validClasses = new List<PlayerClass>();
			bool? specialWeapons = null;
			Backpack excludeBp = null;
			string excludeBpSteamID = "";
			bool createCopyString = false;

			foreach (string str in args)
			{
				ItemSlot bufSlot = ItemSlots.Parse(str);
				if (bufSlot != ItemSlot._Grenade)
				{
					validWeaponSlots.Add(bufSlot);
					continue;
				}

				PlayerClass bufClass = PlayerClasses.Parse(str);
				if (bufClass != 0)
				{
					validClasses.Add(bufClass);
					continue;
				}

				if (str.ToLower() == "normal" || str.ToLower() == "-special")
				{
					specialWeapons = false;
				}
				else if (str.ToLower() == "special" || str.ToLower() == "-normal")
				{
					specialWeapons = true;
				}
				else if (str.ToLower() == "copy")
				{
					createCopyString = true;
				}

				if (str.ToLower().StartsWith("exclude="))
				{
					excludeBpSteamID = str.Substring("exclude=".Length);

					if (excludeBpSteamID == Settings.Instance.HomeSteamID64)
					{
						excludeBp = DataManager.MyBackpackData;
					}
					else
					{
						bool worked = DataManager.LoadOtherBackpack(excludeBpSteamID);
						if (!worked)
						{
							excludeBp = DataManager.BackpackData[excludeBpSteamID];
						}
					}
				}
			}

			string copyString = "\t\tprivate static List<Weapon> _weapons = new List<Weapon>()\n" +
				"\t\t{\n";
			copyString += "\t\t\t#region WeaponsList\n";

			List<Item> res = new List<Item>();
			foreach (Item i in weapons)
			{
				if (i.DefaultQuality != Quality.Unique)
					continue;

				if (specialWeapons == true && i.IsCheapWeapon())
					continue;
				if (specialWeapons == false && !i.IsCheapWeapon())
					continue;

				if (validWeaponSlots.Count > 0 && !validWeaponSlots.Contains(i.Slot))
					continue;

				if (!i.ValidClasses.IsAllClass() && validClasses.Count > 0)
				{
					bool matches = false;
					foreach (PlayerClass c in validClasses)
					{
						if (i.ValidClasses.Contains(c))
						{
							matches = true;
							break;
						}
					}

					if (!matches)
						continue;
				}

				if (excludeBp != null)
				{
					bool found = false;
					foreach (ItemInstance inst in excludeBp.Items)
					{
						if (inst.Item == i)
						{
							found = true;
							break;
						}
					}

					if (found)
						continue;
				}

				// eliminated stuff
				res.Add(i);
				copyString += string.Format("\t\t\tnew Weapon({0}, \"{1}\", {2}),\n", i.ID, i.Name, 
					i.ValidClasses.ConvertAll((c) => c.ToString().ToUpper()).ToCodeString());
			}
			copyString += "\t\t\t#endregion WeaponsList\n";
			copyString += "\t\t};";

			string wepsFilters = "";
			if (validWeaponSlots.Count > 0)
			{
				wepsFilters = string.Join(" ", validWeaponSlots.ConvertAll((s) => s.ToString()) );
				wepsFilters += " ";
			}

			string classFilters = "";
			if (validClasses.Count > 0)
			{
				classFilters = string.Join(" ", validClasses.ConvertAll((c) => c.ToString()) );
				classFilters += " ";
			}

			string specialFilter = "";
			if (specialWeapons != null)
			{
				specialFilter = specialWeapons.Value ? "Special" : "Normal";
				specialFilter += " ";
			}

			string bpFilter = "";
			if (excludeBp != null)
			{
				bpFilter = "Backpack:" + excludeBpSteamID;
				bpFilter += " ";
			}

			LoggerOld.Log("Filters: " + wepsFilters + classFilters + specialFilter + bpFilter, ConsoleColor.White);
			LoggerOld.Log("Found " + res.Count + " matching weapons:", ConsoleColor.White);

			Price? priciestValue = null;
			Item priciestWeapon = null;
			foreach (Item i in res)
			{
				ItemPricing p = DataManager.PriceData.GetPriceData(i);
				string priceStr = p?.GetPriceString() ?? "UNKNOWN";

				LoggerOld.Log("  " + i.Name + ": " + priceStr);

				if (p != null)
				{
					if (priciestValue == null)
					{
						priciestValue = p.Pricing.High;
						priciestWeapon = i;
					}
					else if (p.Pricing.High > priciestValue)
					{
						priciestValue = p.Pricing.High;
						priciestWeapon = i;
					}
				}
			}

			if (createCopyString)
			{
				LoggerOld.Log("Writing code snippet...", ConsoleColor.DarkGray);
				File.WriteAllText(DataManager.CacheLocation + "temporaryStuff.txt", copyString, Encoding.ASCII);
				LoggerOld.Log("  Code snippet complete.", ConsoleColor.DarkGray);
			}

			LoggerOld.Log("Most expensive weapon: " + priciestWeapon + " at " + 
				priciestValue.ToString(), ConsoleColor.White);
		}
	}
}
