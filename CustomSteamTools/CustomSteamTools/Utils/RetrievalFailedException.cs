using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Utils
{
	public enum RetrievalType
	{
		Unspecified = -1,
		Schema = 0,
		PriceData,
		MarketData,
		BackpackContents,
	}

	[Serializable]
	public class RetrievalFailedException : Exception
	{
		public RetrievalType Retrieval
		{ get; private set; }

		public RetrievalFailedException()
		{
			Retrieval = RetrievalType.Unspecified;
		}

		public RetrievalFailedException(RetrievalType retrieval) : 
			base("Unable to retrieve " + retrieval.ToString())
		{
			Retrieval = retrieval;
		}

		public RetrievalFailedException(RetrievalType retrieval, Exception inner) : 
			base("Unable to retrieve " + retrieval.ToString(), inner)
		{
			Retrieval = retrieval;
		}

		public RetrievalFailedException(string message, Exception inner) : base(message, inner)
		{ }

		protected RetrievalFailedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}
