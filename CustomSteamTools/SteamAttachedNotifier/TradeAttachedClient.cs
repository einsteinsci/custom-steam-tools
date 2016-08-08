using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamAttachedNotifier.Events;
using SteamKit2;
#pragma warning disable 67

namespace SteamAttachedNotifier
{
	public sealed class TradeAttachedClient : SteamWrappedClient
	{
		public readonly SteamID MySteamID;

		public event TradeProposedEventHandler TradeProposed;

		public TradeAttachedClient(SteamID id) : base()
		{
			MySteamID = id;
		}

		public override void RegisterOtherCallbacks()
		{
			
		}
	}
}
