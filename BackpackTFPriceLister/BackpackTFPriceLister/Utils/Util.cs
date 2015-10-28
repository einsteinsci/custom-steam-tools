using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Utils
{
	public static class Util
	{
		public static bool ParseAdvancedBool(string s)
		{
			try
			{
				bool res = bool.Parse(s.ToLower());
				return res;
			}
			catch (FormatException)
			{
				int n = int.MaxValue;

				if (int.TryParse(s, out n))
				{
					return n == 1;
				}

				string l = s.ToLower();
				if (l == "yes" || l == "y" || l == "t")
				{
					return true;
				}
				else if (l == "no" || l == "n" || l == "f")
				{
					return false;
				}
			}

			throw new FormatException("Invalid bool: " + s);
		}

		public static string SubstringMax(this string str, int maxLen)
		{
			if (str == null)
			{
				throw new ArgumentNullException(nameof(str));
			}

			string res = "";
			for (int i = 0; i < maxLen && i < str.Length; i++)
			{
				res += str[i];
			}

			return res;
		}
		public static string CutOffEnd(this string str, int count)
		{
			if (str.Length <= count)
			{
				throw new ArgumentOutOfRangeException(
					"Cannot cut off more characters than are in the string.");
			}

			return str.Substring(0, str.Length - count);
		}

		public static async Task<string> DownloadStringAsync(string url)
		{
			string result = null;

			try
			{
				Logger.Log("  Downloading...", ConsoleColor.DarkGray);
				WebClient client = new WebClient();
				result = await client.DownloadStringTaskAsync(url);
			}
			catch (WebException e)
			{
				Logger.Log("  Error downloading: " + e.Message, ConsoleColor.Red);
				return null;
			}

			Logger.Log("  Download complete.", ConsoleColor.DarkGray);
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
					Logger.Log("  Download complete.", ConsoleColor.DarkGray);
				}
				catch (WebException e)
				{
					Logger.Log("  Error downloading: " + e.Message, ConsoleColor.Red);
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
					Logger.Log("  Download timed out.", ConsoleColor.Red);
					thread.Abort();
					return null;
				}
			}

			return result;
		}

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
