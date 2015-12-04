using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SteamAttachedNotifier.Events
{
	public delegate void TradeProposedEventHandler(object sender, TradeProposedEventArgs e);

	public sealed class TradeProposedEventArgs : EventArgs
	{
		public bool Cancel
		{ get; set; }

		public SteamID OtherTrader
		{ get; private set; }

		public TradeProposedEventArgs(SteamID other)
		{
			OtherTrader = other;
		}
	}
}
