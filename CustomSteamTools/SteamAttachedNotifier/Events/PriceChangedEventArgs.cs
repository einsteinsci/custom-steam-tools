using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools;

namespace SteamAttachedNotifier.Events
{
	public delegate void PriceChangedEventHandler(object sender, PriceChangedEventArgs e);

	public sealed class PriceChangedEventArgs : EventArgs
	{
		public PriceRange OldPrice
		{ get; private set; }

		public PriceRange NewPrice
		{ get; private set; }

		public PriceChangedEventArgs(PriceRange old, PriceRange _new)
		{
			OldPrice = old;
			NewPrice = _new;
		}
	}
}
