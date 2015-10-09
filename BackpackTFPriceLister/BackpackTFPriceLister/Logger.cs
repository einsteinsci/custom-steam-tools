using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public delegate void LogEvent(object sender, LogEventArgs e);
	public enum MessageType
	{
		Normal = 0,
		Debug,
		Error,
		Emphasis,
	}

	public class LogEventArgs : EventArgs
	{
		public string Message
		{ get; private set; }

		public MessageType Type
		{ get; set; }

		public LogEventArgs(string message, MessageType type = MessageType.Normal)
		{
			Message = message;
			Type = type;
		}
	}

	public delegate string PromptEvent(object sender, PromptEventArgs e);
	public class PromptEventArgs : EventArgs
	{
		public string Prompt
		{ get; private set; }

		public bool NewlineAfterPrompt
		{ get; private set; }

		public PromptEventArgs(string prompt, bool newline = false)
		{
			Prompt = prompt;
			NewlineAfterPrompt = newline;
		}
	}

	public static class Logger
	{
		public static event LogEvent Logging;

		public static PromptEvent Prompting
		{ get; set; }

		public static void Log(string message, MessageType type = MessageType.Normal, object sender = null)
		{
			if (Logging == null)
			{
				return;
			}

			Logging(sender, new LogEventArgs(message, type));
		}

		public static void AddLine()
		{
			Log("");
		}

		public static string GetInput(string prompt, bool newlineAfterPrompt = false, object sender = null)
		{
			if (Prompting == null)
			{
				return null;
			}

			return Prompting(sender, new PromptEventArgs(prompt, newlineAfterPrompt));
		}
	}
}
