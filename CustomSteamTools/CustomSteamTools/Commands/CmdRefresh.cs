using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdRefresh : ITradeCommand
	{
		public string[] Aliases => new string[] { "refresh", "restart", "reload" };

		public string Description => "Refreshes data from source websites.";

		public string RegistryName => "refresh";

		public string Syntax => "refresh [schema | prices | market | backpack]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			RetrievalType? type = null;

			if (args.HasItems())
			{
				if (args[0].EqualsIgnoreCase("schema"))
				{
					type = RetrievalType.Schema;
				}
				else if (args[0].EqualsIgnoreCase("prices"))
				{
					type = RetrievalType.PriceData;
				}
				else if (args[0].EqualsIgnoreCase("market"))
				{
					type = RetrievalType.MarketData;
				}
				else if (args[0].EqualsIgnoreCase("backpack"))
				{
					type = RetrievalType.BackpackContents;
				}
			}

			Refresh(type);
		}

		public static void Refresh(RetrievalType? type = null)
		{
			bool success = false;
			while (!success)
			{
				try
				{
					if (type == null || type == RetrievalType.Schema)
					{
						DataManager.LoadItemSchema(true);
					}
					if (type == null || type == RetrievalType.BackpackContents)
					{
						DataManager.LoadMyBackpackData(true);
					}
					if (type == null || type == RetrievalType.PriceData)
					{
						DataManager.LoadPriceData(true);
					}
					if (type == null || type == RetrievalType.MarketData)
					{
						DataManager.LoadMarketData(true);
					}

					if (type == null || type == RetrievalType.Schema)
					{
						DataManager.FixHauntedItems();
					}

					success = true;
				}
				catch (RetrievalFailedException e)
				{
					VersatileIO.Fatal("Details: " + e.ToString());
					VersatileIO.Warning("Retrieval failed. Attempting again in 10 seconds.");

					Thread.Sleep(10000);
				}
			}

			if (type == RetrievalType.BackpackContents)
			{
				DataManager.BackpackCaches.Clear();
				DataManager.BackpackDataRaw.Clear();
				DataManager.BackpackData.Clear();
			}
		}
	}
}
