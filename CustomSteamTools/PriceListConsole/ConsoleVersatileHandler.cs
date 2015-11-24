using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace BackpackTFConsole
{
	public sealed class ConsoleVersatileHandler : VersatileHandlerBase, IDisposable
	{
		public bool BePersistent
		{ get; set; }

		public StreamWriter FileStream
		{ get; private set; }

		// [IDisposable] To detect redundant calls
		private bool _disposedValue = false;

		public ConsoleVersatileHandler(string logFolder)
		{
			string filePath = Path.Combine(logFolder, "log-" +
				DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt");

			DeleteOldLogs(logFolder, 10);

			FileStream = new StreamWriter(filePath);
			FileStream.AutoFlush = true;
		}
		
		// TODO: override a destructor only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ConsoleVersatileHandler() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		public void DeleteOldLogs(string folder, int max)
		{
			List<string> filePaths = Directory.GetFiles(folder).ToList();
			filePaths.Sort((a, b) => File.GetLastWriteTime(a).CompareTo(File.GetLastWriteTime(b)) );

			if (filePaths.Count > max)
			{
				for (int i = max; i < filePaths.Count; i++)
				{
					File.Delete(filePaths[i]);
				}
			}
		}

		#region io

		public override void OnLogLine(string line, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(line);

			LogLineToFile(line);
		}

		public override void OnLogPart(string text, ConsoleColor? color)
		{
			if (color != null)
			{
				Console.ForegroundColor = color.Value;
			}
			Console.Write(text);

			LogPartToFile(text);
		}

		public override string GetString(string prompt)
		{
			OnLogPart(prompt, ConsoleColor.Blue);

			string res = Console.ReadLine();
			LogLineToFile(res);

			return res;
		}

		public override double GetDouble(string prompt)
		{
			double d = double.NaN;

			bool worked = false;
			while (!worked)
			{
				string str = GetString(prompt);

				if (double.TryParse(str, out d))
				{
					worked = true;
				}
				else
				{
					VersatileIO.Error("'{0}' is not a valid number.", d);

					if (!BePersistent)
					{
						VersatileIO.Info("Defaulting to NaN.");
						d = double.NaN;
						worked = true;
					}
				}
			}

			return d;
		}

		public override string GetSelection(string prompt, IDictionary<string, object> options)
		{
			foreach (KeyValuePair<string, object> kvp in options)
			{
				OnLogLine("  [{0}]: {1}".Fmt(kvp.Key, kvp.Value), ConsoleColor.White);
			}

			while (true)
			{
				string input = GetString(prompt);

				if (options.ContainsKeyIgnoreCase(input))
				{
					return input;
				}
				else
				{
					VersatileIO.Error("'{0}' is not one of the options above.", input);

					if (!BePersistent)
					{
						VersatileIO.Info("Defaulting to first option.");
						return options.Keys.FirstOrDefault();
					}
				}
			}
		}

		public override string GetSelectionIgnorable(string prompt, IDictionary<string, object> options)
		{
			foreach (KeyValuePair<string, object> kvp in options)
			{
				OnLogLine("  [{0}]: {1}".Fmt(kvp.Key, kvp.Value), ConsoleColor.White);
			}
			
			string input = GetString(prompt);

			if (options.ContainsKeyIgnoreCase(input))
			{
				return input;
			}
			else
			{
				return null;
			}
		}

		#endregion io

		public void LogPartToFile(string text)
		{
			if (FileStream != null)
			{
				FileStream.Write(text);
			}
		}
		public void LogLineToFile(string text)
		{
			if (FileStream != null)
			{
				FileStream.WriteLine(text);
			}
		}

		#region IDisposable
		private void Dispose(bool disposing) // this is the one case where I do not follow my standards for private methods.
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					if (FileStream != null)
					{
						FileStream.Close();
						FileStream.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);

			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion IDisposable
	}
}
