using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using static SteamKit2.WebAPI;
using SteamWebAPI;
using static SteamWebAPI.SteamAPISession;
using BackpackTFPriceLister;
using Sample5_SteamGuard;

namespace SealedTradeBot
{
	public class Program
	{
		public static TF2Data ItemData
		{ get; private set; }

		public const string BOT_API_KEY = "6EC366E3D20B931D81378B625575CFF3";
		public const string BOT_STEAMID = "";
		
		public const string SEALED_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";
		public const string SEALED_STEAMID = "";

		public static void Main(string[] args)
		{
			Console.Title = "Sealed's Trade Bot";

			Logger.Logging += (sender, e) => BotLogger.LogLine(e.Message, e.Foreground, e.Background);
			Logger.LoggingComplex += (sender, e) => BotLogger.LogComplex(e.Arguments);
			Logger.Prompting = (sender, e) => BotLogger.GetInput(e.Prompt +
				(e.NewlineAfterPrompt ? "\n" : ""), e.Foreground, e.Background);

			PriceLister.AutoSetup(true, true);
			ItemData = PriceLister.ItemData;

			Console.WriteLine();
			List<TradeOffer> openOffers = OfferManager.GetAllOpenOffers();

			ClientManager.Run();

			//Console.ReadKey();
		}

		private static void WebAPINewsExample()
		{
			// in order to interact with the Web APIs, you must first acquire an interface
			// for a certain API
			using (dynamic steamNews = WebAPI.GetInterface("ISteamNews"))
			{
				// note the usage of c#'s dynamic feature, which can be used
				// to make the api a breeze to use

				// the ISteamNews WebAPI has only 1 function: GetNewsForApp,
				// so we'll be using that

				// when making use of dynamic, we call the interface function directly
				// and pass any parameters as named arguments
				KeyValue kvNews = steamNews.GetNewsForApp(appid: 440); // get news for tf2

				// the return of every WebAPI call is a KeyValue class that contains the result data

				// for this example we'll iterate the results and display the title
				foreach (KeyValue news in kvNews["newsitems"]["newsitem"].Children)
				{
					Console.WriteLine("News: {0}", news["title"].AsString());
				}
				Console.WriteLine();

				// for functions with multiple versions, the version can be specified by
				// adding a number after the function name when calling the API

				kvNews = steamNews.GetNewsForApp2(appid: 570);

				// if a number is not specified, version 1 is assumed by default

				// notice that the output of this version differs from the first version
				foreach (KeyValue news in kvNews["newsitems"].Children)
				{
					Console.WriteLine("News: {0}", news["title"].AsString());
				}
				Console.WriteLine();

				// note that the interface functions can throw WebExceptions when the API
				// is otherwise inaccessible (networking issues, server downtime, etc)
				// and these should be handled appropriately
				try
				{
					kvNews = steamNews.GetNewsForApp002(appid: 730, maxlength: 100, count: 5);
				}
				catch (WebException ex)
				{
					Console.WriteLine("Unable to make API request: {0}", ex.Message);
				}
			}

			// for WebAPIs that require an API key, the key can be specified in the GetInterface function
			using (dynamic steamUserAuth = WebAPI.GetInterface("ISteamUserAuth", SEALED_API_KEY))
			{
				// as the interface functions are synchronous, it may be beneficial to specify a timeout for calls
				steamUserAuth.Timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;

				// additionally, if the API you are using requires you to POST or use an SSL connection, you may specify
				// these settings with the "method" and "secure" reserved parameters
				//steamUserAuth.AuthenticateUser(someParam: "someValue", method: WebRequestMethods.Http.Post, secure: true);
			}

			// if you are using a language that does not have dynamic object support, or you otherwise don't wish to use it
			// you can call interface functions through a Call method
			using (WebAPI.Interface steamNews = WebAPI.GetInterface("ISteamNews"))
			{
				Dictionary<string, string> newsArgs = new Dictionary<string, string>();
				newsArgs["appid"] = "440";

				KeyValue results = steamNews.Call("GetNewsForApp", /* version */ 1, newsArgs);
			}
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
