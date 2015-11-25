using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Market;
using UltimateUtil;
using UltimateUtil.Fluid;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Skins;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdSkins : ITradeCommand
	{
		public string[] Aliases => new string[] { "skin", "priceskin", "gmskin", "skins" };

		public string Description => "Prices an item by its Gun Mettle Skin.";

		public string RegistryName => "skin";

		public string Syntax => "skin {skinName | searchQuery}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.IsEmpty())
			{
				VersatileIO.Error("Syntax: " + Syntax);
				return;
			}

			string query = string.Join(" ", args);

			Skin skin = null;
			foreach (Skin s in GunMettleSkins.Skins)
			{
				if (s.Name.EqualsIgnoreCase(query))
				{
					skin = s;
					break;
				}
			}

			#region search
			if (skin == null)
			{
				VersatileIO.WriteLine("Searching skins...", ConsoleColor.Gray);

				List<Skin> matches = new List<Skin>();
				foreach (Skin s in GunMettleSkins.Skins)
				{
					if (s.Name.ContainsIgnoreCase(query))
					{
						matches.Add(s);
					}
				}

				if (matches.Count == 1)
				{
					skin = matches.First();
				}

				while (skin == null)
				{
					for (int i = 0; i < matches.Count; i++)
					{
						VersatileIO.WriteLine("  [{0}]: {1}".Fmt(i, matches[i].Name), ConsoleColor.Gray);
					}
					string input = VersatileIO.GetString("Select a Skin: ");

					if (input.EqualsIgnoreCase("esc"))
					{
						VersatileIO.Info("Search cancelled.");
						return;
					}

					int n = -1;
					bool worked = int.TryParse(input, out n);
					if (worked && n.IsBetween(0, matches.Count - 1))
					{
						skin = matches[n];
					}
					else
					{
						VersatileIO.Error("Invalid selection. Try again.");
					}
				}
			}
			#endregion

			Dictionary<SkinWear, Price> prices = GetSkinPrices(skin);

			VersatileIO.Write("Skin: ", ConsoleColor.White);
			VersatileIO.WriteLine("{0} ({1} {2})".Fmt(skin.Name, skin.Grade.ToReadableString(), 
				skin.BaseWeapon), skin.Grade.GetColor());

			prices.ForEach((w, p) => 
			{
				VersatileIO.WriteLine("  {0}: {1}".Fmt(w.ToReadableString(), p.ToString()), ConsoleColor.Gray);
			});
		}

		public static Dictionary<SkinWear, Price> GetSkinPrices(Skin skin)
		{
			Dictionary<SkinWear, Price> results = new Dictionary<SkinWear, Price>();

			for (SkinWear w = SkinWear.FactoryNew; w <= SkinWear.BattleScarred; w++)
			{
				Price? price = GetSkinPrice(skin, w);

				if (price != null)
				{
					results.Add(w, price.Value);
				}
			}

			return results;
		}

		public static Price? GetSkinPrice(Skin skin, SkinWear wear)
		{
			string hash = skin.GetMarketHash(wear);
			MarketPricing pricing = DataManager.MarketPrices.GetPricing(hash);

			if (pricing != null)
			{
				return pricing.Price;
			}
			else
			{
				VersatileIO.Warning("  No market pricing found for {0} {1}.", wear.ToReadableString(), skin.Name);
				return null;
			}
		}
	}
}
