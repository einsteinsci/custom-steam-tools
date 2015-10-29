using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace SealedTradeBot
{
	public enum TradeOfferState
	{
		Invalid = 1,
		Active = 2,
		Accepted = 3,
		Countered = 4,
		Expired = 5,
		Canceled = 6,
		Declined = 7,
		ItemsNoLongerAvailable = 8,
		EmailPending = 9,
		EmailCanceled = 10,
	};

	public class TradeOffer : IEquatable<TradeOffer>
	{
		public string TradeOfferID
		{ get; set; }

		public long OtherAccountID
		{ get; set; }

		public string Message
		{ get; set; }

		public long ExpirationTime
		{ get; set; }

		public TradeOfferState State
		{ get; set; }

		public bool IsMyOffer
		{ get; set; }

		public long TimeCreated
		{ get; set; }

		public long TimeUpdated
		{ get; set; }

		public bool IsRealTimeTrade
		{ get; set; }

		public List<TradeAsset> ItemsToGive
		{ get; set; }

		public List<TradeAsset> ItemsToReceive
		{ get; set; }

		public TradeOffer()
		{ }

		public TradeOffer(KeyValue kv)
		{
			TradeOfferID = kv["tradeofferid"].Value;
			OtherAccountID = long.Parse(kv["accountid_other"].Value);
			Message = kv["message"].Value;
			ExpirationTime = long.Parse(kv["expiration_time"].Value);
			State = (TradeOfferState)int.Parse(kv["trade_offer_state"].Value);
			IsMyOffer = Util.ParseWebBool(kv["is_our_offer"].Value);
			TimeCreated = long.Parse(kv["time_created"].Value);
			TimeUpdated = long.Parse(kv["time_updated"].Value);
			IsRealTimeTrade = Util.ParseWebBool(kv["from_real_time_trade"].Value);

			ItemsToGive = new List<TradeAsset>();
			foreach (KeyValue _kv in kv["items_to_give"].Children)
			{
				ItemsToGive.Add(new TradeAsset(_kv));
			}

			ItemsToReceive = new List<TradeAsset>();
			foreach (KeyValue _kv in kv["items_to_receive"].Children)
			{
				ItemsToReceive.Add(new TradeAsset(_kv));
			}
		}

		public bool Equals(TradeOffer other)
		{
			return TradeOfferID == other.TradeOfferID;
		}

		public override bool Equals(object obj)
		{
			if (obj is TradeOffer)
			{
				return Equals(obj as TradeOffer);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return TradeOfferID.GetHashCode();
		}

		public override string ToString()
		{
			return (IsMyOffer ? "[S] " : "[R] ") + Message + (Message != "" ? " " : "") + 
				"(" + ItemsToGive.Count.ToString() + " for " + 
				ItemsToReceive.Count.ToString() + "): " + State.ToString();
		}
	}
}
