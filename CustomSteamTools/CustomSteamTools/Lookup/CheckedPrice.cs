using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Items;
using UltimateUtil;

namespace CustomSteamTools.Lookup
{
	public sealed class CheckedPrice
	{
		public ItemPricing Pricing
		{ get; private set; }

		public Quality Quality => Pricing?.Quality ?? Quality.Decorated;

		public UnusualEffect Unusual
		{ get; private set; }

		public Skin Skin
		{ get; private set; }

		public PriceRange Price
		{
			get
			{
				if (Pricing == null)
				{
					return new PriceRange(_price.GetValueOrDefault());
				}

				return Pricing.Pricing;
			}
		}
		private Price? _price;

		public SkinWear? SkinWear
		{ get; private set; }

		public CheckedPrice(ItemPricing pricing)
		{
			Pricing = pricing;
			if (Quality == Quality.Unusual)
			{
				Unusual = DataManager.Schema.Unusuals.First((ue) => ue.ID == pricing.PriceIndex);
			}
		}

		public CheckedPrice(Skin skin, SkinWear wear, Price price)
		{
			Skin = skin;
			SkinWear = wear;
			_price = price;
		}

		public string GetUnusualEffectString()
		{
			return "[#{1:D2}] {0}: {2}".Fmt(Unusual.Name, Unusual.ID, Pricing.GetPriceString());
		}

		public override string ToString()
		{
			if (Skin != null)
			{
				return SkinWear.Value.ToReadableString() + " " + Skin.Name + ": " + Price.ToString();
			}
			else
			{
				string res = Pricing.CompiledTitleName;
				if (Unusual != null)
				{
					res += "(#{0}: {1})".Fmt(Unusual.ID, Unusual.Name);
				}
				res += ": " + Pricing.GetPriceString();

				return res;
			}
		}
	}
}
