using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Utils
{
	public static class Util
	{
		public static string CutOffEnd(this string str, int count)
		{
			if (str.Length <= count)
			{
				throw new ArgumentOutOfRangeException(
					"Cannot cut off more characters than are in the string.");
			}

			return str.Substring(0, str.Length - count);
		}

		public static string ToCode(this ConsoleColor col, char esc = '&')
		{
			switch (col)
			{
			case ConsoleColor.Black:
				return esc + "0";
			case ConsoleColor.DarkBlue:
				return esc + "1";
			case ConsoleColor.DarkGreen:
				return esc + "2";
			case ConsoleColor.DarkCyan:
				return esc + "3";
			case ConsoleColor.DarkRed:
				return esc + "4";
			case ConsoleColor.DarkMagenta:
				return esc + "5";
			case ConsoleColor.DarkYellow:
				return esc + "6";
			case ConsoleColor.Gray:
				return esc + "7";
			case ConsoleColor.DarkGray:
				return esc + "8";
			case ConsoleColor.Blue:
				return esc + "9";
			case ConsoleColor.Green:
				return esc + "a";
			case ConsoleColor.Cyan:
				return esc + "b";
			case ConsoleColor.Red:
				return esc + "c";
			case ConsoleColor.Magenta:
				return esc + "d";
			case ConsoleColor.Yellow:
				return esc + "e";
			case ConsoleColor.White:
				return esc + "f";
			default:
				return esc + "?";
			}
		}

		public static async Task<string> DownloadStringAsync(string url)
		{
			string result = null;

			try
			{
				VersatileIO.Verbose("  Downloading...");
				WebClient client = new WebClient();
				result = await client.DownloadStringTaskAsync(url);
			}
			catch (WebException e)
			{
				VersatileIO.Error("  Error downloading: " + e.Message);
				return null;
			}

			VersatileIO.Verbose("  Download complete.");
			return result;
		}

		public static string DownloadString(string url, TimeSpan timeout)
		{
			WebClient client = new WebClient();
			client.Encoding = Encoding.UTF8;

			bool giveup = false;
			string result = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					result = client.DownloadString(url);
					VersatileIO.Verbose("  Download complete.");
				}
				catch (WebException e)
				{
					VersatileIO.Error("  Error downloading: " + e.Message);
					giveup = true;
				}
			});

			DateTime start = DateTime.Now;
			thread.Start();
			while (result == null && !giveup)
			{
				Thread.Sleep(300);

				DateTime now = DateTime.Now;

				if (now.Subtract(start) >= timeout)
				{
					giveup = true;
					VersatileIO.Debug("  Download timed out.");
					thread.Abort();
					return null;
				}
			}

			return result;
		}

		public static string ToCodeString(this List<string> list)
		{
			return "new string[] { " + string.Join(", ", list.ConvertAll((s) => '"' + s + '"')) + " }";
		}

		//[Obsolete("Not good for classifieds scraping")]
		public static string Asciify(string content)
		{
			if (content == null)
			{
				return "";
			}

			return content.Replace('é', 'e').Replace('ò', 'o').Replace('ü', 'u').Replace('ä', 'a');
		}
	}
}
