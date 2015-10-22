using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackpackTFPriceLister;
using SteamKit2;

namespace SealedTradeBot
{
    [Obsolete("Unimplemented for now")]
	public static class ChatManager
	{
		static LogEvent currentOutHandler;
		static LogComplexEvent currentOutComplexHandler;
		static PromptEvent currentPromptHandler;
		static CommandHandler.PreCommandHandler preCommandHandler; // never used my ass...

		static Thread commandHandlerThread;
		static bool commandHasChat = false;

		static string awaitedInput = null;

		public static bool IsExiting
		{ get; private set; } = false;

		public static void OnFriendChat(SteamFriends.FriendMsgCallback e)
		{
			if (e.EntryType == EChatEntryType.ChatMsg)
			{
				if (commandHasChat)
				{
					awaitedInput = e.Message;

					return;
				}

				BotLogger.LogLine(ClientManager.FriendsHandler.GetFriendPersonaName(e.Sender) + ": " + 
					e.Message, ConsoleColor.Cyan);

				if (e.Message.StartsWith("!") || e.Message.StartsWith("/"))
				{
					commandHasChat = true;
					string cmd = e.Message.Substring(1);

					SetIOToUser(e.Sender);

					commandHandlerThread = new Thread(() =>
					{
						DataManager.RunCommand(cmd);
						if (IsExiting)
						{
							BotLogger.LogLine("Exiting...");
							return;
						}

						SendChatMessage(e.Sender, "[Command complete]");

						FreeIOUser();
						commandHasChat = false;
					});
					commandHandlerThread.Start();

					BotLogger.LogLine("Started command for user '" + 
						e.Sender.ToString() + "': " + cmd);
				}
			}
		}

		public static void SendChatMessage(SteamID user, string message)
		{
			ClientManager.FriendsHandler.SendChatMessage(user, EChatEntryType.ChatMsg, message);
		}

		public static void SetIOToUser(SteamID user)
		{
			currentOutHandler = (o, e) => 
			{
				SendChatMessage(user, e.Message);
				Thread.Sleep(100);
			};
			currentOutComplexHandler = (o, e) =>
			{
				SendChatMessage(user, e.Unformatted);
				Thread.Sleep(100);
			};

			Logger.Logging += currentOutHandler;
			Logger.LoggingComplex += currentOutComplexHandler;

			currentPromptHandler = Logger.Prompting;

			Logger.Prompting = (o, e) =>
			{
				string res = "";
				if (!e.NothingAllowed)
				{
					res = GetUserInput(user, e.Prompt);
				}

				return res;
			};

			preCommandHandler = (cmd, args) => PreCommand(user, cmd, args);
			CommandHandler.PreCommand += preCommandHandler;
		}

		public static string GetUserInput(SteamID user, string prompt)
		{
			SendChatMessage(user, prompt);

			while (awaitedInput == null)
			{
				Thread.Sleep(1000);

				if (!commandHasChat)
				{
					return "";
				}
			}

			string res = awaitedInput;

			awaitedInput = null;
			return res;
		}

		public static bool PreCommand(SteamID user, string cmd, List<string> args)
		{
			if (cmd.ToLower() == "bp" && args.Count == 0)
			{
				args.Add(user.ConvertToUInt64().ToString());
				return false;
			}

			if (cmd.ToLower() == "exit")
			{
				IsExiting = true;
				return true;
			}

			return false;
		}

		public static void FreeIOUser()
		{
			Logger.Logging -= currentOutHandler;
			Logger.LoggingComplex -= currentOutComplexHandler;
			currentOutHandler = null;
			currentOutComplexHandler = null;

			Logger.Prompting = currentPromptHandler;
			currentPromptHandler = null;

			CommandHandler.PreCommand -= preCommandHandler;
			preCommandHandler = null;

			BotLogger.LogLine("User freed.");
		}
	}
}
