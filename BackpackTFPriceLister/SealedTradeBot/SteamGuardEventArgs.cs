using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealedTradeBot
{
	public class SteamGuardEventArgs : EventArgs
	{
		public string SteamGuardCode
		{ get; set; }

		public SteamGuardEventArgs(string code)
		{
			SteamGuardCode = code;
		}

        public SteamGuardEventArgs()
        {
            SteamGuardCode = "";
        }
	}
}
