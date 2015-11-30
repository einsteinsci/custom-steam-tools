using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools;

namespace TF2TradingToolkit.ViewModel
{
	public class PriceUnitViewModel
	{
		public const string CASH_IMAGE_URL = "https://wiki.teamfortress.com/w/images/thumb/f/f9/Smallcredits.png/120px-Smallcredits.png?t=20120905231342";

		public string Unit
		{ get; private set; }

		public string UnitReadable
		{
			get
			{
				switch (Unit)
				{
				case Price.CURRENCY_REF:
					return "Refined Metal";
				case Price.CURRENCY_KEYS:
					return "Crate Keys";
				case Price.CURRENCY_CASH:
					return "USD ($)";
				default:
					return Unit;
				}
			}
		}

		public string ImageURL
		{
			get
			{
				switch (Unit)
				{
				case Price.CURRENCY_REF:
					return Price.RefinedMetal.ImageURL;
				case Price.CURRENCY_KEYS:
					return Price.Key.ImageURL;
				case Price.CURRENCY_CASH:
					return CASH_IMAGE_URL;
				default:
					return null;
				}
			}
		}

		public PriceUnitViewModel(string unit)
		{
			Unit = unit;
		}

		public Price GetPrice(double amount)
		{
			return new Price(amount, Unit);
		}
	}
}
