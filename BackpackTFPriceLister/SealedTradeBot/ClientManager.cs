using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2;

namespace SealedTradeBot
{
    [Obsolete("Use SealedBot instead.")]
	public static class ClientManager
	{
		const string SEALED_STEAMID = "STEAM_0:0:75622499";
		const ulong SEALED_STEAMID64 = 76561198111510726;

		public static SteamID SealedSteamID
		{ get; internal set; }

		static SteamClient steamClient;
		static CallbackManager manager;

		public static SteamUser UserHandler
		{ get; private set; }

		public static SteamFriends FriendsHandler
		{ get; private set; }

		public static List<SteamID> Friends
		{ get; private set; }

		static bool isRunning;

		static string username, password;
		static string authCode, twoFactorAuth;

		public static Thread RunOnThread()
		{
			Thread res = new Thread(Run);
			res.Start();

			return res;
		}

		public static void Run()
		{
			BotLogger.LogLine("Username: sealedinterfacebot", ConsoleColor.White);
			username = "sealedinterfacebot";
			BotLogger.Log("Passsword: ", ConsoleColor.White);
			password = Util.ReadPassword();

			// create our steamclient instance
			steamClient = new SteamClient();
			// create the callback manager which will route callbacks to function calls
			manager = new CallbackManager(steamClient);

			// get the steamuser handler, which is used for logging on after successfully connecting
			UserHandler = steamClient.GetHandler<SteamUser>();

			// register a few callbacks we're interested in
			// these are registered upon creation to a callback manager, which will then route the callbacks
			// to the functions specified
			manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
			manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

			manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
			manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

			// this callback is triggered when the steam servers wish for the client to store the sentry file
			manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

			// get the steam friends handler, which is used for interacting with friends on the network after logging on
			FriendsHandler = steamClient.GetHandler<SteamFriends>();

			// we use the following callbacks for friends related activities
			manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);
			manager.Subscribe<SteamFriends.FriendsListCallback>(OnFriendsList);
			manager.Subscribe<SteamFriends.PersonaStateCallback>(OnPersonaState);
			manager.Subscribe<SteamFriends.FriendAddedCallback>(OnFriendAdded);
			manager.Subscribe<SteamFriends.FriendMsgCallback>(ChatManager.OnFriendChat);

			isRunning = true;

			BotLogger.LogDebug("Connecting to Steam...");

			// initiate the connection
			//steamClient.Connect();

			Thread postInitThread = new Thread(PostInit);
			postInitThread.Start();

			BotLogger.LogLine("Init Complete.");

			// create our callback handling loop
			while (isRunning)
			{
				// in order for the callbacks to get routed, they need to be handled by the manager
				manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));

				if (ChatManager.IsExiting)
				{
					ChatManager.SendChatMessage(SealedSteamID, "Goodbye, sir.");
					break;
				}
			}

			steamClient.Disconnect();
		}

		public static void PostInit()
		{
			SteamID temp = new SteamID(SEALED_STEAMID64);

			while (SealedSteamID == null)
			{
				manager.RunWaitCallbacks(TimeSpan.FromSeconds(5));

				if (FriendsHandler.GetFriendCount() <= 0)
				{
					continue;
				}

				Friends = new List<SteamID>();
				for (int i = 0; i < FriendsHandler.GetFriendCount(); i++)
				{
					SteamID friend = FriendsHandler.GetFriendByIndex(i);

					if (friend.AccountID == temp.AccountID)
					{
						SealedSteamID = friend;
					}

					Friends.Add(friend);
				}
			}

			BotLogger.LogDebug("Sending message to " + FriendsHandler.GetFriendPersonaName(SealedSteamID));
			FriendsHandler.SendChatMessage(SealedSteamID, EChatEntryType.ChatMsg, "Hello, sir.");
			BotLogger.LogLine("PostInit Complete.");
		}

		public static void Sleep(int seconds)
		{
			manager.RunWaitCallbacks(TimeSpan.FromSeconds(seconds));
		}

		static void OnConnected(SteamClient.ConnectedCallback callback)
		{
			if (callback.Result != EResult.OK)
			{
				BotLogger.LogErr("Unable to connect to Steam: " + callback.Result);

				isRunning = false;
				return;
			}

			BotLogger.LogDebug("Connected to Steam! Logging in '" + username + "'...");

			byte[] sentryHash = null;
			if (File.Exists("sentry.bin"))
			{
				// if we have a saved sentry file, read and sha-1 hash it
				byte[] sentryFile = File.ReadAllBytes("sentry.bin");
				sentryHash = CryptoHelper.SHAHash(sentryFile);
			}

			UserHandler.LogOn(new SteamUser.LogOnDetails
			{
				Username = username,
				Password = password,

				// in this sample, we pass in an additional authcode
				// this value will be null (which is the default) for our first logon attempt
				AuthCode = authCode,

				// if the account is using 2-factor auth, we'll provide the two factor code instead
				// this will also be null on our first logon attempt
				TwoFactorCode = twoFactorAuth,

				// our subsequent logons use the hash of the sentry file as proof of ownership of the file
				// this will also be null for our first (no authcode) and second (authcode only) logon attempts
				SentryFileHash = sentryHash,
			});
		}

		static void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			// after recieving an AccountLogonDenied, we'll be disconnected from steam
			// so after we read an authcode from the user, we need to reconnect to begin the logon flow again

			BotLogger.LogLine("Disconnected from Steam, reconnecting in 5...", ConsoleColor.Yellow);

			Thread.Sleep(TimeSpan.FromSeconds(5));

			steamClient.Connect();
		}

		static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{
			bool isSteamGuard = callback.Result == EResult.AccountLogonDenied;
			bool is2FA = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

			if (isSteamGuard || is2FA)
			{
				BotLogger.LogLine("This account is SteamGuard protected!", ConsoleColor.Yellow);

				if (is2FA)
				{
					twoFactorAuth = BotLogger.GetInput("Please enter your 2 factor auth code from your authenticator app: ");
				}
				else
				{
					authCode = BotLogger.GetInput("Please enter the auth code sent to the email at " +
						callback.EmailDomain + ": ");
				}

				return;
			}

			if (callback.Result != EResult.OK)
			{
				BotLogger.LogErr("Unable to logon to Steam: " + callback.Result + " / " +
					callback.ExtendedResult);

				isRunning = false;
				return;
			}

			BotLogger.LogLine("Successfully logged on!");

			// at this point, we'd be able to perform actions on Steam
		}

		static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
		{
			// before being able to interact with friends, you must wait for the account info callback
			// this callback is posted shortly after a successful logon

			// at this point, we can go online on friends, so lets do that
			FriendsHandler.SetPersonaState(EPersonaState.LookingToTrade);
		}

		static void OnFriendsList(SteamFriends.FriendsListCallback callback)
		{
			// at this point, the client has received it's friends list

			int friendCount = FriendsHandler.GetFriendCount();

			BotLogger.LogDebug("We have " + friendCount.ToString() + " friends");

			for (int x = 0; x < friendCount; x++)
			{
				// steamids identify objects that exist on the steam network, such as friends, as an example
				SteamID steamIdFriend = FriendsHandler.GetFriendByIndex(x);

				// we'll just display the STEAM_ rendered version
				BotLogger.LogDebug("Friend: " + steamIdFriend.Render());
			}

			// we can also iterate over our friendslist to accept or decline any pending invites

			foreach (var friend in callback.FriendList)
			{
				if (friend.Relationship == EFriendRelationship.RequestRecipient)
				{
					// this user has added us, let's add him back
					FriendsHandler.AddFriend(friend.SteamID);
				}
			}
		}

		static void OnFriendAdded(SteamFriends.FriendAddedCallback callback)
		{
			// someone accepted our friend request, or we accepted one
			BotLogger.LogDebug(callback.PersonaName + " is now a friend");
		}

		static void OnPersonaState(SteamFriends.PersonaStateCallback callback)
		{
			// this callback is received when the persona state (friend information) of a friend changes

			// for this sample we'll simply display the names of the friends
			BotLogger.LogDebug(callback.Name + " changed status to " + callback.State.ToString());
		}

		static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			BotLogger.LogLine("Logged off of Steam: " + callback.Result);
		}

		static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback e)
		{
			BotLogger.LogDebug("Updating sentryfile...");

			// write out our sentry file
			// ideally we'd want to write to the filename specified in the callback
			// but then this sample would require more code to find the correct sentry file to read during logon
			// for the sake of simplicity, we'll just use "sentry.bin"

			int fileSize;
			byte[] sentryHash;
			using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				fs.Seek(e.Offset, SeekOrigin.Begin);
				fs.Write(e.Data, 0, e.BytesToWrite);
				fileSize = (int)fs.Length;

				fs.Seek(0, SeekOrigin.Begin);
				using (var sha = new SHA1CryptoServiceProvider())
				{
					sentryHash = sha.ComputeHash(fs);
				}
			}

			// inform the steam servers that we're accepting this sentry file
			UserHandler.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
			{
				JobID = e.JobID,

				FileName = e.FileName,

				BytesWritten = e.BytesToWrite,
				FileSize = fileSize,
				Offset = e.Offset,

				Result = EResult.OK,
				LastError = 0,

				OneTimePassword = e.OneTimePassword,

				SentryFileHash = sentryHash,
			});

			BotLogger.LogDebug("Done!");
		}
	}
}
