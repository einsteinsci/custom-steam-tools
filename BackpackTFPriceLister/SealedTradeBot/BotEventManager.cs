using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.Reflection;

namespace SealedTradeBot
{
	public static class BotEventManager
	{
		public static SealedBot Bot
		{ get; set; }

		public static void Initialize(CallbackManager manager)
		{
			Type t_emg = typeof(BotEventManager);

			MethodInfo[] mgrMethods = typeof(CallbackManager).GetMethods();
			MethodInfo subMI = null;
			foreach (MethodInfo m in mgrMethods)
			{
				if (m.Name == "Subscribe" && m.GetParameters().Length == 1)
				{
					subMI = m;
				}
			}

			MethodInfo[] methods = t_emg.GetMethods();
			foreach (MethodInfo m in methods)
			{
				var atts = m.GetCustomAttributes<BotSubscribeEventAttribute>();
				if (atts == null || atts.Count() == 0)
				{
					continue;
				}

				ParameterInfo[] pars = m.GetParameters();
				if (pars.Length == 0)
				{
					throw new ArgumentException("Event has no parameters");
				}

				Type eType = pars[0].ParameterType;

				if (!typeof(CallbackMsg).IsAssignableFrom(eType))
				{
					throw new ArgumentException("Type " + eType.Name + " does not inherit from CallbackMsg.");
				}

				MethodInfo generic = subMI.MakeGenericMethod(eType);
				object del = Util.CreateDelegateByParameter(eType, null, m);

				// manager.Subscribe<T>(func); <-- essentially this
				generic.Invoke(manager, new object[] { del });
			}
		}

		[BotSubscribeEvent]
		public static void OnConnected(SteamClient.ConnectedCallback e)
		{
            BotLogger.LogDebug("Connection Event: " + e.Result);

            if (e.Result == EResult.OK)
            {
				Bot.UserLogOn();
            }
			else
			{
				BotLogger.LogErr("  Failed to connect to steam. trying again...");
				Bot.ClientHandler.Connect();
			}
		}

        [BotSubscribeEvent]
        public static void OnDisconnected(SteamClient.DisconnectedCallback e)
        {
            Bot.OnDisconnect(e);
        }

        [BotSubscribeEvent]
		public static void OnLogon(SteamUser.LoggedOnCallback e)
		{
            Bot.OnLogon(e);
		}
        
        [BotSubscribeEvent]
        public static void OnLogoff(SteamUser.LoggedOffCallback e)
        {
            Bot.OnLogoff(e);
        }

		[BotSubscribeEvent]
        public static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback e)
        {
            Bot.OnMachineAuth(e);
        }

		[BotSubscribeEvent]
		public static void OnFriendChat(SteamFriends.FriendMsgCallback e)
		{
			if (e.EntryType != EChatEntryType.ChatMsg)
			{
				return;
			}

			string friendlyName = Bot.FriendsHandler.GetFriendPersonaName(e.Sender);
			BotLogger.LogLine("[CHAT] " + friendlyName + ": " + e.Message, ConsoleColor.DarkCyan);
		}

		[BotSubscribeEvent]
		public static void OnFriendsListReceived(SteamFriends.FriendsListCallback e)
		{
			List<SteamID> friends = Bot.Friends; // trigger initialize

			Bot.FriendsHandler.SetPersonaState(EPersonaState.Online);

			SteamID buf = new SteamID(SealedBot.SEALED_STEAMID64);
			SteamID sealeD = null;
			foreach (SteamID f in friends)
			{
				if (f.AccountID == buf.AccountID)
				{
					sealeD = f;
					break;
				}
			}

			if (sealeD != null)
			{
				if (Bot.Admins.Contains(sealeD))
				{
					Bot.Admins.Add(sealeD);
				}

				Bot.SendChatMessage(sealeD, "Hello, sir.");
			}
		}
	}
}
