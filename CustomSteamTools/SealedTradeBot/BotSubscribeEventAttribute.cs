using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealedTradeBot
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class BotSubscribeEventAttribute : Attribute
	{
		// This is a positional argument
		public BotSubscribeEventAttribute()
		{ }
	}
}
