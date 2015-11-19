﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Utils
{
	public delegate void LogEventOld(object sender, LogEventArgsOld e);

	public class LogEventArgsOld : EventArgs
	{
		public string Message
		{ get; private set; }

		public ConsoleColor? Foreground
		{ get; set; }

		public ConsoleColor? Background
		{ get; set; }

		public LogEventArgsOld(string message, ConsoleColor? fg = ConsoleColor.Gray,
			ConsoleColor? bg = null)
		{
			Message = message;
			Foreground = fg;
			Background = bg;
		}
	}

	public delegate void LogComplexEventOld(object sender, LogComplexEventArgs e);
	public class LogComplexEventArgs : EventArgs
	{
		public List<object> Arguments
		{ get; private set; }

		public string Unformatted
		{
			get
			{
				string res = "";
				foreach (object obj in Arguments)
				{
					if (obj == null)
					{
						continue;
					}

					if (obj is string)
					{
						res += (string)obj;
					}
				}

				return res;
			}
		}

		public LogComplexEventArgs(params object[] args)
		{
			Arguments = new List<object>(args);
		}
	}

	public delegate string PromptEventOld(object sender, PromptEventArgs e);
	public class PromptEventArgs : EventArgs
	{
		public string Prompt
		{ get; private set; }

		public bool NewlineAfterPrompt
		{ get; private set; }

		public ConsoleColor? Foreground
		{ get; set; }

		public ConsoleColor? Background
		{ get; set; }

		public bool NothingAllowed
		{ get; set; }

		public PromptEventArgs(string prompt, bool newline = false, bool optional = false,
			ConsoleColor? fg = ConsoleColor.Green, ConsoleColor? bg = null)
		{
			Prompt = prompt;
			NewlineAfterPrompt = newline;
			Foreground = fg;
			Background = bg;
			NothingAllowed = optional;
		}
	}

	[Obsolete("Use UltimateUtil.UserInteraction.VersatileIO instead")]
	public static class LoggerOld
	{
		public static event LogEventOld Logging;
		public static event LogComplexEventOld LoggingComplex;

		public static PromptEventOld Prompting
		{ get; set; }

		public static void Log(string message, ConsoleColor? color = ConsoleColor.Gray,
			ConsoleColor? background = null, object sender = null)
		{
			if (Logging == null)
			{
				return;
			}

			Logging(sender, new LogEventArgsOld(message, color, background));
		}

		public static void AddLine()
		{
			Log("");
		}

		public static string GetInput(string prompt, bool newlineAfterPrompt = false, bool optional = false,
			ConsoleColor? fg = ConsoleColor.Green, ConsoleColor? bg = null, object sender = null)
		{
			if (Prompting == null)
			{
				return null;
			}

			return Prompting(sender, new PromptEventArgs(prompt, newlineAfterPrompt, optional, fg, bg));
		}

		public static void LogComplex(params object[] logLine)
		{
			if (LoggingComplex == null)
			{
				return;
			}

			LoggingComplex(null, new LogComplexEventArgs(logLine));
		}
	}
}