using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealedTradeBot
{
	public static class BotLogger
	{
		public static void Log(string message, ConsoleColor? color = ConsoleColor.Gray, 
			ConsoleColor? bg = null)
		{
			if (color.HasValue)
			{
				Console.ForegroundColor = color.Value;
			}
			if (bg.HasValue)
			{
				Console.BackgroundColor = bg.Value;
			}

			Console.Write(message);
		}

		public static void LogDebug(string message)
		{
			LogLine(message, ConsoleColor.DarkGray);
		}

		public static void LogErr(string message)
		{
			LogLine(message, ConsoleColor.Red);
		}

		public static void LogComplex(params object[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];

				if (obj == null)
				{
					throw new ArgumentNullException("args[" + i.ToString() + "]");
				}

				if (obj is ConsoleColor)
				{
					Console.ForegroundColor = (ConsoleColor)obj;
				}

				if (obj is string)
				{
					Log((string)obj, null);
				}
			}
		}

		public static void LogLine(string message, ConsoleColor? color = ConsoleColor.Gray,
			ConsoleColor? bg = null)
		{
			Log(message + "\n", color, bg);
		}

		public static void LogLine()
		{
			LogLine("");
		}

		public static string GetInput(string prompt, ConsoleColor? color = ConsoleColor.Green,
			ConsoleColor? bg = null)
		{
			Log(prompt, color, bg);

			return Console.ReadLine();
		}
	}
}
