using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamKit2;
using static SteamKit2.SteamClient;
using static SteamKit2.SteamUser;

namespace SealedTradeBot
{
	public class SealedBot : IDisposable
	{
        private readonly BackgroundWorker _workerThread;
		private readonly LogOnDetails _logOnDetails;

		private bool _hasDisposed = false;
        private string _userNonce;
        private string _uniqueID;

		public event EventHandler<SteamGuardEventArgs> OnSteamGuardRequired;

		public readonly CallbackManager CallbackManager;

        public readonly SteamClient ClientHandler;
        public readonly SteamUser UserHandler;
        public readonly SteamFriends FriendsHandler;
        public readonly SteamTrading TradingHandler;
        public readonly SteamGameCoordinator GameCoordinator;
        // public readonly Dictionary<SteamID, UserHandler> UserHandlers; // custom user handlers?
        // custom notification handler?

        public const string BOT_API_KEY = "6EC366E3D20B931D81378B625575CFF3";
        public const string SEALED_API_KEY = "692BC909FAF4C20E94B49A0DD7CCBC23";

		public const ulong SEALED_STEAMID64 = 76561198111510726;

		public List<SteamID> Friends
		{
			get
			{
				if (_friends != null)
				{
					return _friends;
				}

				_friends = new List<SteamID>();
				for (int i = 0; i < FriendsHandler.GetFriendCount(); i++)
				{
					_friends.Add(FriendsHandler.GetFriendByIndex(i));
				}

				return _friends;
			}
		}
		private List<SteamID> _friends;

        public List<SteamID> Admins
        { get; private set; }

        public string SteamGuardCode
        { get; set; }

        public bool IsRunning
        { get; set; }

        public bool IsLoggedIn
        { get; private set; }

        public int CurrentGame // 440 = TF2
        { get; private set; }

        public SealedBot(string username, string password)
        {
			_logOnDetails = new LogOnDetails();
			_logOnDetails.Username = username;
			_logOnDetails.Password = password;

            Admins = new List<SteamID>();

			ServicePointManager.ServerCertificateValidationCallback += (o, c, x, e) => true;

            BotLogger.LogDebug("Initializing Bot...");
			ClientHandler = new SteamClient();
			UserHandler = ClientHandler.GetHandler<SteamUser>();
			FriendsHandler = ClientHandler.GetHandler<SteamFriends>();
			GameCoordinator = ClientHandler.GetHandler<SteamGameCoordinator>();

			CallbackManager = new CallbackManager(ClientHandler);
			BotEventManager.Bot = this;
			BotEventManager.Initialize(CallbackManager); // ooOOOooo...reflection...

            _workerThread = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _workerThread.DoWork += WorkerThread_DoWork;
            _workerThread.RunWorkerCompleted += WorkerThread_OnCompleted;
            _workerThread.RunWorkerAsync();
        }

		// oooh...destructors!
		~SealedBot()
		{
			Dispose(false);
		}

        private void WorkerThread_OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
			{
				BotLogger.LogErr(e.Error.Message);
				BotLogger.Log("Stopping bot...", ConsoleColor.Yellow);

				StopBot();
			}
        }

        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
			//ICallbackMsg msg;

			while (!_workerThread.CancellationPending)
			{
				try
				{
					//msg = ClientHandler.WaitForCallback(true);
					//HandleMessage(msg);

					CallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
				}
				catch (WebException ex)
				{
					string errMessage = string.Format("URI: {0} >> {1}",
						ex.Response?.ResponseUri?.ToString() ?? "unknown" ?? "unknown", ex.Message);
					BotLogger.LogErr(ex.Message);
					BotLogger.LogErr("Steam is Down (see above), trying again in 45 sec...");
					// Steam is down, try again in 45 sec
					Thread.Sleep(45000);
				}
				catch (Exception ex)
				{
					BotLogger.LogErr("Error in worker thread: " + ex.Message);
					BotLogger.LogLine("Restarting bot...", ConsoleColor.Yellow);
				}
			}
		}

		// apparently this never returns?
		public bool StartBot()
		{
			IsRunning = true;
			BotLogger.LogDebug("Connecting to Steam...");
			if (!_workerThread.IsBusy)
			{
				_workerThread.RunWorkerAsync();
			}

			ClientHandler.Connect();
			BotLogger.LogDebug("  Done loading bot!");
			return true;
		}

		public void StopBot()
		{
			IsRunning = false;
			BotLogger.LogDebug("Attempting Shutdown...");
			ClientHandler.Disconnect();
			_workerThread.CancelAsync();
			while (_workerThread.IsBusy)
			{
				Thread.Yield();
			}
		}

		public void DoSteamGuardRequired(SteamGuardEventArgs e)
		{
			SteamGuardCode = null;

			EventHandler<SteamGuardEventArgs> handler = OnSteamGuardRequired;
			if (handler != null)
			{
				handler(this, e);
			}

			while (true)
			{
				if (SteamGuardCode != null)
				{
					e.SteamGuardCode = SteamGuardCode;
					break;
				}

				Thread.Sleep(50);
			}
        }

        public void UserLogOn()
        {
            // get sentry file which has the machine hw info saved 
            // from when a steam guard code was entered
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "sentryfiles"));
            FileInfo file = new FileInfo(Path.Combine("sentryfiles", _logOnDetails.Username + ".sentryfile"));

            if (file.Exists && file.Length > 0)
            {
                _logOnDetails.SentryFileHash = _makeSHAHash(File.ReadAllBytes(file.FullName));
            }
            else
            {
                _logOnDetails.SentryFileHash = null;
            }

            UserHandler.LogOn(_logOnDetails);
        }

        public void OnLogon(LoggedOnCallback e)
        {
            BotLogger.LogDebug("Logon Event: " + e.Result.ToString());

            if (e.Result == EResult.OK)
            {
                _userNonce = e.WebAPIUserNonce;
            }
            else
            {
                BotLogger.LogErr("  Logon error: " + e.Result.ToString());
            }

            if (e.Result == EResult.AccountLogonDenied)
            {
                BotLogger.Log("SteamGuard is enabled. Enter 'auth' and then the code: ", ConsoleColor.Green);
                SteamGuardEventArgs sge = new SteamGuardEventArgs();
                DoSteamGuardRequired(sge);
                if (sge != null && sge.SteamGuardCode != "")
                {
                    _logOnDetails.AuthCode = sge.SteamGuardCode;
                }
                else
                {
					// this will probably never happen.
                    _logOnDetails.AuthCode = BotLogger.GetInput("");
                }
            }

            if (e.Result == EResult.InvalidLoginAuthCode)
            {
                BotLogger.Log("  SteamGuard code was invalid. Enter 'auth' and then the correct code: ", ConsoleColor.Green);

				SteamGuardCode = null;
				while (SteamGuardCode == null || SteamGuardCode == "")
				{
					Thread.Sleep(50);
				}

				_logOnDetails.AuthCode = SteamGuardCode;
            }

			BotLogger.LogLine("Logged on!", ConsoleColor.DarkGreen);

        }

        public void OnMachineAuth(UpdateMachineAuthCallback e)
        {
            byte[] hash = _makeSHAHash(e.Data);
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "sentryfiles"));

            File.WriteAllBytes(Path.Combine("sentryfiles", _logOnDetails.Username + ".sentryfile"), e.Data);

            MachineAuthDetails dets = new MachineAuthDetails()
            {
                BytesWritten = e.BytesToWrite,
                FileName = e.FileName,
                FileSize = e.BytesToWrite,
                Offset = e.Offset,

                SentryFileHash = hash,

                OneTimePassword = e.OneTimePassword,

                LastError = 0,
                Result = EResult.OK,
                JobID = e.JobID
            };

            UserHandler.SendMachineAuthResponse(dets);
        }

        public void OnLogoff(LoggedOffCallback e)
        {
            foreach (SteamID sid in Admins)
            {
                SendChatMessage(sid, "Goodbye, sir.");
            }

            IsLoggedIn = false;
            BotLogger.LogLine("Logged off steam. Reason: " + e.Result.ToString());
        }

        public void OnDisconnect(DisconnectedCallback e)
        {
            if (IsLoggedIn)
            {
                IsLoggedIn = false;
                // close active trade
                BotLogger.LogLine("Disconnected from steam.");
            }

            ClientHandler.Disconnect();
        }

        public void SendChatMessage(SteamID friend, string message)
        {
            FriendsHandler.SendChatMessage(friend, EChatEntryType.ChatMsg, message);
        }

        private static byte[] _makeSHAHash(byte[] input)
        {
			SHA1Managed sha = new SHA1Managed();

			byte[] output = sha.ComputeHash(input);

			sha.Clear();

			return output;
		}

        public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool topDown)
		{
			if (_hasDisposed)
			{
				return;
			}

			StopBot();

			if (topDown)
			{
				// dispose any other IDisposable objects within this.
			}
		}
	}
}
