using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackpackTFPriceLister;
using System.Windows.Forms;
using System.IO;
using System.Net;
using BackpackTFPriceLister.ItemDataJson;

namespace BackpackTFConsole
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Console.Title = "Backpack.tf Console";
			Console.ForegroundColor = ConsoleColor.White;
			PriceData.Initialize(true);
			Logger.Event += DebugLog;

			PriceData.LoadData(true, true);
			//Logger.Log("\n" + PriceData.ItemCache, false, true);

			PriceData.ParseItemsJson();
			TF2Data data = PriceData.TranslateData();

			Console.ReadKey();
		}

		private static void DebugLog(object sender, LogEventArgs e)
		{
			if (sender != null)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("<" + sender.GetType().ToString() + ">: ");
				Console.ForegroundColor = ConsoleColor.White;
			}

			if (e.Debug)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(e.Message);
				Console.ForegroundColor = ConsoleColor.White;
			}
			else if (e.Error)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e.Message);
				Console.ForegroundColor = ConsoleColor.White;
			}
			else
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
