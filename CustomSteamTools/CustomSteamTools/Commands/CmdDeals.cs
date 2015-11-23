using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdDeals : ITradeCommand
	{
		public string[] Aliases => new string[] { "deals", "getdeals", "profitable" };

		public string Description => "Lists the best 'deals' currently available to a user on bp.tf.";

		public string RegistryName => "deals";

		public string Syntax => "deals [steamID64] [/q=QUALITY x?] [/s=PLAINSLOT x?] [/minprofit=MINPROFITREF] " +
			"[/nohw | /hw] [/ac | /nc | /uc] [/nobot | /bot]";

		public static bool DoBeepOnFinished
		{ get; set; }

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			DealFilters filters = new DealFilters();

			string steamid = Settings.Instance.HomeSteamID64;

			#region args
			foreach (string s in args)
			{
				if (!s.StartsWith("/"))
				{
					steamid = s;
					continue;
				}

				if (s.StartsWithIgnoreCase("/q="))
				{
					string sq = s.Substring("/q=");
					Quality? q = ItemQualities.ParseNullable(sq);

					if (q != null)
					{
						filters.Qualities.Add(q.Value);
					}
				}

				if (s.StartsWithIgnoreCase("/s="))
				{
					string ss = s.Substring("/s=");
					ItemSlotPlain sp = ItemSlots.Plain.Parse(ss);

					if (sp != ItemSlotPlain.Unused)
					{
						filters.Slots.Add(sp);
					}
				}

				if (s.StartsWithIgnoreCase("/minprofit="))
				{
					string mps = s.Substring("/minprofit=");
					if (mps.EndsWith("ref"))
					{
						mps = mps.CutOffEnd("ref".Length);
					}

					double d;
					if (double.TryParse(mps, out d))
					{
						filters.MinProfit = new Price(d);
					}
					else
					{
						VersatileIO.Warning("Invalid number: {0}. Ignoring.", mps);
					}
				}

				if (s.EqualsIgnoreCase("/nohw"))
				{
					filters.Halloween = false;
				}
				else if (s.EqualsIgnoreCase("/hw"))
				{
					filters.Halloween = true;
				}

				if (s.EqualsIgnoreCase("/ac"))
				{
					filters.Craftable = null;
				}
				else if (s.EqualsIgnoreCase("/nc") || s.EqualsIgnoreCase("/uc"))
				{
					filters.Craftable = false;
				}

				if (s.EqualsIgnoreCase("/nobot"))
				{
					filters.Botkiller = false;
				}
				else if (s.EqualsIgnoreCase("/bot"))
				{
					filters.Botkiller = true;
				}
			}
			#endregion args

			List<ItemSale> sales = DealFinder.FindDeals(steamid, filters);
			if (sales == null)
			{
				// already logged
				return;
			}

			sales.Sort((a, b) => a.Profit.TotalRefined.CompareTo(b.Profit.TotalRefined));

			VersatileIO.WriteLine();
			VersatileIO.Info("{0} deals found:", sales.Count);

			foreach (ItemSale s in sales)
			{
				VersatileIO.WriteComplex("  " + s.ToComplexString());
			}
		}
	}
}
