using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Market
{
	public enum KillstreakType
	{
		None = 0,
		Basic = 1,
		Specialized = 2,
		Professional = 3
	}

	public static class KillstreakTypes
	{
		public static string ToReadableString(this KillstreakType ks)
		{
			switch (ks)
			{
				case KillstreakType.None:
					return "";
				case KillstreakType.Basic:
					return "Killstreak";
				case KillstreakType.Specialized:
					return "Specialized Killstreak";
				case KillstreakType.Professional:
					return "Professional Killstreak";
				default:
					return "KS.ERR";
			}
		}

		public static KillstreakType? ParseNullable(string input)
		{
			string s = input.ToLower().Trim();

			if (s == "" || s == "none" || s == "0")
			{
				return KillstreakType.None;
			}

			if (s == "killstreak" || s == "basic" || s == "ks" || s == "1")
			{
				return KillstreakType.Basic;
			}

			if (s == "specialized killstreak" || s == "specialized" || 
				s == "spec" || s == "spec ks" || s == "2")
			{
				return KillstreakType.Specialized;
			}

			if (s == "professional killstreak" || s == "professional" || 
				s == "prof" || s == "prof ks" || s == "3")
			{
				return KillstreakType.Professional;
			}

			return null;
		}

		public static KillstreakType Parse(string input)
		{
			return ParseNullable(input) ?? KillstreakType.None;
		}

		public static bool HasKillstreak(this KillstreakType ks)
		{
			return ks != KillstreakType.None;
		}
	}
}
