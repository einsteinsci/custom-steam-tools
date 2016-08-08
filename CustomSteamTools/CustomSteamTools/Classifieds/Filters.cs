using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Classifieds
{
	public class DealsFilters
	{
		public List<Quality> Qualities
		{ get; set; }

		public List<ItemSlotPlain> Slots
		{ get; set; }

		public List<PlayerClass> Classes
		{ get; set; }

		public Price? DealsMinProfit
		{ get; set; }

		public bool? Halloween
		{ get; set; }

		public bool? Craftable
		{ get; set; }

		public bool? Botkiller
		{ get; set; }

		public bool AllowAllClass
		{ get; set; }

		public DealsFilters()
		{
			Craftable = true;
			AllowAllClass = true;

			Slots = new List<ItemSlotPlain>();
			Qualities = new List<Quality>();
			Classes = new List<PlayerClass>();
		}

		public static string GetSyntax(bool deals)
		{
			return "[/q=QUALITY x?] [/s=PLAINSLOT x?] [/c=CLASS x?]" + (deals ? " [/minprofit=MINPROFITREF]" : "") +
			" [/nohw | /hw] [/ac | /nc | /uc] [/nobot | /bot]";
		}

		public bool MatchesPricing(ItemPricing pricing)
		{
			if (pricing.Item == null)
			{
				return false;
			}

			if (Craftable != null && pricing.Craftable != Craftable.Value)
			{
				return false;
			}

			if (Botkiller != null && pricing.Item.IsBotkiller() != Botkiller.Value)
			{
				return false;
			}

			bool isHalloween = (pricing.Item.HalloweenOnly || (pricing.Item.HasHauntedVersion ?? false));
			if (Halloween != null && isHalloween != Halloween.Value)
			{
				return false;
			}

			if (Qualities.HasItems() && !Qualities.Contains(pricing.Quality))
			{
				return false;
			}

			if (Slots.HasItems() && !Slots.Contains(pricing.Item.PlainSlot))
			{
				return false;
			}

			if (!AllowAllClass && Classes.HasItems() && pricing.Item.ValidClasses.IsAllClass())
			{
				return false;
			}

			if (Classes.HasItems() && !Classes.Exists((c) => pricing.Item.ValidClasses.Contains(c)))
			{
				return false;
			}

			return true;
		}

		public void HandleArg(string arg)
		{
			if (arg.StartsWithIgnoreCase("/q="))
			{
				string sq = arg.Substring("/q=");
				Quality? q = ItemQualities.ParseNullable(sq);

				if (q != null)
				{
					Qualities.Add(q.Value);
				}
			}

			if (arg.StartsWithIgnoreCase("/s="))
			{
				string ss = arg.Substring("/s=");
				ItemSlotPlain sp = ItemSlots.Plain.Parse(ss);

				if (sp != ItemSlotPlain.Unused)
				{
					Slots.Add(sp);
				}
			}

			if (arg.StartsWithIgnoreCase("/c="))
			{
				string cs = arg.Substring("/c=");
				PlayerClass? c = PlayerClasses.ParseNullable(cs);

				if (c != null)
				{
					Classes.Add(c.Value);
				}
			}

			if (arg.StartsWithIgnoreCase("/minprofit="))
			{
				string mps = arg.Substring("/minprofit=");
				if (mps.EndsWith("ref"))
				{
					mps = mps.CutOffEnd("ref".Length);
				}

				double d;
				if (double.TryParse(mps, out d))
				{
					DealsMinProfit = new Price(d);
				}
				else
				{
					VersatileIO.Warning("Invalid number: {0}. Ignoring.", mps);
				}
			}

			if (arg.EqualsIgnoreCase("/nohw"))
			{
				Halloween = false;
			}
			else if (arg.EqualsIgnoreCase("/hw"))
			{
				Halloween = true;
			}

			if (arg.EqualsIgnoreCase("/ac"))
			{
				Craftable = null;
			}
			else if (arg.EqualsIgnoreCase("/nc") || arg.EqualsIgnoreCase("/uc"))
			{
				Craftable = false;
			}

			if (arg.EqualsIgnoreCase("/nobot"))
			{
				Botkiller = false;
			}
			else if (arg.EqualsIgnoreCase("/bot"))
			{
				Botkiller = true;
			}
		}

		public override string ToString()
		{
			string res = "";
			if (Qualities.HasItems())
			{
				res += Qualities.ToReadableString(" ", false);
			}
			if (Slots.HasItems())
			{
				if (res != "")
				{
					res += " ";
				}

				res += Slots.ToReadableString(" ", false);
			}
			if (Classes.HasItems())
			{
				if (res != "")
				{
					res += " ";
				}

				res += Classes.ToReadableString(" ", false);
			}

			if (DealsMinProfit != null)
			{
				res += " MinProfit:" + DealsMinProfit.Value.TotalRefined + "ref";
			}

			if (Halloween == true)
			{
				res += " Halloween-Only";
			}
			else if (Halloween == false)
			{
				res += " No-Halloween";
			}

			if (Craftable == null)
			{
				res += " Any-Craftable";
			}
			else if (Craftable == false)
			{
				res += " Non-Craftable";
			}

			if (Botkiller == true)
			{
				res += " Botkillers";
			}
			else if (Botkiller == false)
			{
				res += " No-Botkillers";
			}

			return res;
		}
	}
}
