using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SealedTradeBot
{
	public static class Util
	{
		public static bool ParseWebBool(string s)
		{
			bool b;
			if (!bool.TryParse(s, out b))
			{
				int n;
				if (!int.TryParse(s, out n))
				{
					throw new FormatException("Could not parse '" + s + "' into bool.");
				}

				return n == 1;
			}

			return b;
		}

		public static string ReadPassword(char mask = '*')
		{
			const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
			int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

			var pass = new Stack<char>();
			char chr = (char)0;

			while ((chr = Console.ReadKey(true).KeyChar) != ENTER)
			{
				if (chr == BACKSP)
				{
					if (pass.Count > 0)
					{
						Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (chr == CTRLBACKSP)
				{
					while (pass.Count > 0)
					{
						Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (FILTERED.Count(x => chr == x) > 0)
				{ }
				else
				{
					pass.Push(chr);
					Console.Write(mask);
				}
			}

			Console.WriteLine();

			return new string(pass.Reverse().ToArray());
		}

		public static object CreateDelegateByParameter(Type parameterType, object target, MethodInfo method)
		{

			var createDelegate = typeof(Util).GetMethod("CreateDelegate")
				.MakeGenericMethod(parameterType);

			var del = createDelegate.Invoke(null, new object[] { target, method });

			return del;
		}

		public static Action<TEvent> CreateDelegate<TEvent>(object target, MethodInfo method)
		{
			var del = (Action<TEvent>)Delegate.CreateDelegate(typeof(Action<TEvent>), target, method);

			return del;
		}
	}
}
