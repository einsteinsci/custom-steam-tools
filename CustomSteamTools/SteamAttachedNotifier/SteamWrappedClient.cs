using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2;
using UltimateUtil.UserInteraction;

namespace SteamAttachedNotifier
{
	public abstract class SteamWrappedClient
	{
		public readonly string SentryFilePath = Path.Combine(Environment.GetEnvironmentVariable("appdata"), 
			"CustomSteamTools", "sentry.bin");

		public readonly SteamClient Client;
		public readonly SteamUser User;
		public readonly CallbackManager CallbackManager;
		public readonly SteamTrading TradingHandler;

		public string LoginUsername
		{ get; private set; }

		[SecuritySafeCritical]
		protected string password;

		[SecuritySafeCritical]
		protected string steamGuardCode;

		[SecuritySafeCritical]
		protected string twoFactorAuth;

		public bool IsRunning
		{ get; private set; }

		public SteamWrappedClient()
		{
			Client = new SteamClient();
			CallbackManager = new CallbackManager(Client);

			User = Client.GetHandler<SteamUser>();
			TradingHandler = Client.GetHandler<SteamTrading>();

			CallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnect);
			CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnect);

			CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLogon);
			CallbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLogoff);

			CallbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

			RegisterOtherCallbacks();
		}

		public abstract void RegisterOtherCallbacks();

		public virtual void StartAttachment(string username, string password, 
			string steamGuardCode = null, string twoFactorAuth = null)
		{
			LoginUsername = username;
			this.password = password;

			this.steamGuardCode = steamGuardCode;
			this.twoFactorAuth = twoFactorAuth;

			Client.Connect();

			IsRunning = true;
			while (IsRunning)
			{
				CallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(2));
			}
		}

		public virtual void Stop()
		{
			IsRunning = false;
		}

		public virtual void OnConnect(SteamClient.ConnectedCallback e)
		{
			if (e.Result != EResult.OK)
			{
				VersatileIO.Error("Unable to connect to Steam: " + e.Result.ToString());

				Stop();
				return;
			}

			VersatileIO.Success("Connected to Steam!");
			VersatileIO.Debug("Logging in user '{0}'...", LoginUsername);

			byte[] hash = null;
			if (!File.Exists(SentryFilePath))
			{
				byte[] sentryFile = File.ReadAllBytes(SentryFilePath);
				hash = CryptoHelper.SHAHash(sentryFile);
			}

			SteamUser.LogOnDetails logon = new SteamUser.LogOnDetails();
			logon.Username = LoginUsername;
			logon.Password = password;
			logon.AuthCode = steamGuardCode;
			logon.TwoFactorCode = twoFactorAuth;
			logon.SentryFileHash = hash;

			User.LogOn(logon);
		}

		public virtual void OnDisconnect(SteamClient.DisconnectedCallback e)
		{
			if (e.UserInitiated)
			{
				VersatileIO.Info("Client Disconnected.");
				return;
			}

			VersatileIO.Warning("Disconnected from Steam, reconnecting in 5 secs...");

			Thread.Sleep(5000);

			Client.Connect();
		}

		public virtual void OnLogon(SteamUser.LoggedOnCallback e)
		{
			bool isSteamGuard = e.Result == EResult.AccountLogonDenied;
			bool isTwoFactorAuth = e.Result == EResult.AccountLoginDeniedNeedTwoFactor;

			if (isSteamGuard || isTwoFactorAuth)
			{
				VersatileIO.Warning("Account has additional authentication.");

				if (isTwoFactorAuth)
				{
					twoFactorAuth = VersatileIO.GetString("Enter Two-Factor Authentication code: ");
				}
				else if (isSteamGuard)
				{
					steamGuardCode = VersatileIO.GetString("Enter SteamGuard code: ");
				}

				return;
			}

			if (e.Result != EResult.OK)
			{
				VersatileIO.Error("Unable to log on to Steam: {0} ({1})", e.Result, e.ExtendedResult);
				IsRunning = false;
				return;
			}

			VersatileIO.Success("Successfully logged on!");
		}

		public virtual void OnLogoff(SteamUser.LoggedOffCallback e)
		{
			VersatileIO.Warning("Logged off of Steam: " + e.Result);
		}

		public virtual void OnMachineAuth(SteamUser.UpdateMachineAuthCallback e)
		{
			VersatileIO.Debug("Updating sentry file...");

			int fileSize;
			byte[] sentryHash;
			using (FileStream fs = File.Open(SentryFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				fs.Seek(e.Offset, SeekOrigin.Begin);
				fs.Write(e.Data, 0, e.BytesToWrite);
				fileSize = (int)fs.Length;

				fs.Seek(0, SeekOrigin.Begin);
				using (SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider())
				{
					sentryHash = sha.ComputeHash(fs);
				}
			}

			SteamUser.MachineAuthDetails details = new SteamUser.MachineAuthDetails();
			details.JobID = e.JobID;
			details.FileName = e.FileName;
			details.BytesWritten = e.BytesToWrite;
			details.FileSize = fileSize;
			details.Offset = e.Offset;

			details.Result = EResult.OK;
			details.LastError = 0;
			details.OneTimePassword = e.OneTimePassword;
			details.SentryFileHash = sentryHash;

			User.SendMachineAuthResponse(details);

			VersatileIO.Success("Machine Authentication Complete!");
		}
	}
}
