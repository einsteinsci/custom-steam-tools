using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UltimateUtil;

namespace CustomSteamTools.Classifieds
{
	public class DealFilters
	{
		public List<Quality> Qualities
		{ get; set; }

		public List<ItemSlotPlain> Slots
		{ get; set; }

		public Price? MinProfit
		{ get; set; }

		public bool? Halloween
		{ get; set; }

		public bool? Craftable
		{ get; set; }

		public bool? Botkiller
		{ get; set; }

		public DealFilters()
		{
			Craftable = true;
			Slots = new List<ItemSlotPlain>();
			Qualities = new List<Quality>();
		}

		public bool MatchesPricing(ItemPricing pricing)
		{
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

			return true;
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
			if (MinProfit != null)
			{
				res += " MinProfit:" + MinProfit.Value.TotalRefined.ToString() + "ref";
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
