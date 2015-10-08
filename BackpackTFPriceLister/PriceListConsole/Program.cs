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
using Newtonsoft.Json;
using BackpackTFPriceLister.PriceDataJson;

namespace BackpackTFConsole
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Console.Title = "Backpack.tf Console";
			Console.ForegroundColor = ConsoleColor.White;
			PriceLister.Initialize(true);
			Logger.Event += DebugLog;

			PriceLister.LoadData(true, false);
			//Logger.Log("\n" + PriceData.ItemCache, false, true);

			PriceLister.ParseItemsJson();
			TF2Data tf2 = PriceLister.TranslateData();

			BPTFPriceDataJson price = PriceLister.ParsePricesJson();

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
