using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CustomSteamTools;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;

using SteamKit2;

using SteamWebAPI;

using static SteamKit2.WebAPI;
using static SteamWebAPI.SteamAPISession;

namespace SealedTradeBot
{
	public class Program
	{
		public static GameSchema ItemData
		{ get; private set; }

		public const string BOT_API_KEY = "6EC366E3D20B931D81378B625575CFF3";
		public const string SEALED_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";

		public static SealedBot Bot
		{ get; private set; }

		public static Thread OfferCheckerThread
		{ get; private set; }

		public static void Main(string[] args)
		{
			StartOfferChecker();
			Console.ReadKey();
		}

		private static void StartOfferChecker()
		{
			TimeSpan CHECK_INTERVAL = TimeSpan.FromMinutes(10);

			bool shutDownThread = false;

			OfferCheckerThread = new Thread(() =>
			{
				List<TradeOffer> knownOffers = OfferManager.GetAllOpenOffers();

				DateTime lastCheck = DateTime.Now.Subtract(TimeSpan.FromMinutes(20));
				while (!shutDownThread)
				{
					if (DateTime.Now.Subtract(lastCheck) < CHECK_INTERVAL)
					{
						Thread.Sleep(2000);
						continue;
					}

					lastCheck = DateTime.Now;
					OfferManager.LoadOffers();
					List<TradeOffer> offers = OfferManager.GetAllOpenOffers();
					DateTime currentTime = DateTime.Now;
					string timeStamp = currentTime.ToString("[HH:mm:ss] ");

					List<TradeOffer> incoming = new List<TradeOffer>();
					foreach (TradeOffer o in offers)
					{
						if (o.State != TradeOfferState.Active)
						{
							continue;
						}

						if (o.IsRealTimeTrade)
						{
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.WriteLine(timeStamp + "Real-time trade completed with " +
								o.OtherAccountID.ToString());
							continue; // I already know about these.
						}

						if (!knownOffers.Exists((_o) => _o.Equals(o)))
						{
							incoming.Add(o);
						}
					}

					if (incoming.Count == 0)
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.WriteLine(timeStamp + "No new offers found.");
					}
					else
					{
						Console.Beep(); // make some noise
						Console.ForegroundColor = ConsoleColor.Green;
						foreach (TradeOffer o in incoming)
						{
							string id = o.OtherAccountID.ToString();
							string msg = o.Message;

							Console.WriteLine(timeStamp + "Offer from " + id + ": " + msg);
						}
					}

					knownOffers = new List<TradeOffer>(offers);
				}

				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Shutting down thread...");
			});
			OfferCheckerThread.Name = "Offer Checker";
			OfferCheckerThread.Start();

			Thread.Sleep(10000);

			while (OfferCheckerThread.IsAlive)
			{
				if (shutDownThread)
				{
					Thread.Sleep(100);
					continue;
				}

				Console.ForegroundColor = ConsoleColor.White;
				string input = Console.ReadLine();

				if (input.ToLower() == "exit")
				{
					shutDownThread = true;
				}
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Press any key to exit.");
		}

		private static void StartTradeBot()
		{
			Console.Title = "Sealed's Trade Bot";

			Logger.Logging += (sender, e) => BotLogger.LogLine(e.Message, e.Foreground, e.Background);
			Logger.LoggingComplex += (sender, e) => BotLogger.LogComplex(e.Arguments);
			Logger.Prompting = (sender, e) => BotLogger.GetInput(e.Prompt +
				(e.NewlineAfterPrompt ? "\n" : ""), e.Foreground, e.Background);

			Settings.LoadFromFile();
			DataManager.AutoSetup(true, true);
			ItemData = DataManager.Schema;

			Console.WriteLine();
			List<TradeOffer> openOffers = OfferManager.GetAllOpenOffers();
			BotEventManager.Initialize(new CallbackManager(new SteamClient()));

			BotLogger.LogLine(" Log in to Steam");
			BotLogger.LogLine("  Username: sealedinterfacebot", ConsoleColor.White);
			string username = "sealedinterfacebot";
			BotLogger.Log("  Passsword: ", ConsoleColor.White);
			string password = Util.ReadPassword();

			Bot = new SealedBot(username, password);
			Bot.StartBot();

			//ClientManager.Run();
			Settings.SaveToFile();

			BotLogger.LogLine("Bot has started");

			while (Bot.IsRunning)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				string input = Console.ReadLine();

				string[] cmdAndArgs = input.Split(' ');
				if (cmdAndArgs.Length == 0)
				{
					continue;
				}

				if (cmdAndArgs[0].ToLower() == "auth")
				{
					if (cmdAndArgs.Length < 2)
					{
						BotLogger.LogErr("A code must be provided.");
						continue;
					}

					string auth = cmdAndArgs[1].ToUpper();
					Bot.SteamGuardCode = auth;

					BotLogger.LogLine("SteamGuard code set.");
				}

				if (cmdAndArgs[0].ToLower() == "exit")
				{
					BotLogger.LogLine("Exit code called.");
					Bot.StopBot();
				}
			}

			BotLogger.LogLine("Bot has exited!", ConsoleColor.Yellow);
		}

		public static void LoginToSteam()
		{
			SteamAPISession session = new SteamAPISession();

			BotLogger.LogLine("Username: sealedinterfacebot", ConsoleColor.White);
			string username = "sealedinterfacebot";
			BotLogger.Log("Password: ", ConsoleColor.White);
			string password = Util.ReadPassword();

			Console.WriteLine("Logging in...");
			LoginStatus logStatus = LoginStatus.LoginFailed;
			try
			{
				logStatus = session.Authenticate(username, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not log in: " + e.Message);
				Console.ReadKey();
				return;
			}

			if (logStatus == LoginStatus.SteamGuard)
			{
				Console.Write("SteamGuard Code: ");
				string steamguardCode = Console.ReadLine();

				logStatus = session.Authenticate(username, password, steamguardCode);
			}

			if (logStatus == LoginStatus.LoginFailed)
			{
				Console.WriteLine("Login Failed!");
				Console.ReadKey();
				return;
			}

			Console.WriteLine("Login Successful!");
			Console.WriteLine();

			List<Friend> friends = session.GetFriends();
			foreach (Friend f in friends)
			{
				User u = session.GetUserInfo(f.steamid);
				Console.WriteLine("#" + u.steamid + ": " + u.nickname);
			}
		}
	}
}
