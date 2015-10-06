using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public delegate void LogEvent(object sender, LogEventArgs e);
	public class LogEventArgs : EventArgs
	{
		public string Message
		{ get; private set; }

		public bool Debug
		{ get; private set; }

		public bool Error
		{ get; private set; }

		public LogEventArgs(string message, bool error = false, bool debug = false)
		{
			Message = message;
			Debug = debug;
			Error = error;
		}
	}

	public static class Logger
	{
		public static LogEvent Event
		{ get; set; }

		public static void Log(string message, bool error = false, bool debug = false, object sender = null)
		{
			if (Event == null)
			{
				return;
			}

			Event(sender, new LogEventArgs(message, error, debug));
		}
	}
}
