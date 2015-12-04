using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools;
using SteamAttachedNotifier.Events;
using SteamKit2;

namespace SteamAttachedNotifier
{
	public class SteamNotifier
	{
		public Price LastKeyPrice
		{ get; private set; }

		public event PriceChangedEventHandler KeyPriceChanged;

		public bool HasKeyPriceChanged(BackgroundWorker worker = null)
		{
			if (DataManager.PriceData == null)
			{
				DataManager.AutoSetup(true, worker);
			}

			Price old = Price.OneKey;
			DataManager.LoadPriceData(false);

			LastKeyPrice = Price.OneKey;
			if (KeyPriceChanged != null && old != Price.OneKey)
			{
				KeyPriceChanged(this, new PriceChangedEventArgs(old.ToPriceRange(), Price.OneKey.ToPriceRange()));
			}

			return old != Price.OneKey;
		}
	}
}
