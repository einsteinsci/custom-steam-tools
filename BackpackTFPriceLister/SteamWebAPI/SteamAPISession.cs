// Steam Web API Library by Overv
// =======================================================================
// Copyright (c) 2015 Alexander Overvoorde
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace SteamWebAPI
{
	/// <summary>
	/// Class allowing you to use the Steam Web API to log in and use Steam Friends functionality.
	/// </summary>
	public class SteamAPISession
	{
		private string accessToken;
		private string umqid;
		private string steamid;
		private int message = 0;

		#region innter classes

		/// <summary>
		/// Enumeration of possible authentication results.
		/// </summary>
		public enum LoginStatus
		{
			LoginFailed,
			LoginSuccessful,
			SteamGuard
		}

		/// <summary>
		/// Status of a user.
		/// </summary>
		public enum UserStatus
		{
			Offline = 0,
			Online = 1,
			Busy = 2,
			Away = 3,
			Snooze = 4
		}

		/// <summary>
		/// Visibility of a user's profile.
		/// </summary>
		public enum ProfileVisibility
		{
			Private = 1,
			Public = 3,
			FriendsOnly = 8
		}

		/// <summary>
		/// Available sizes of user avatars.
		/// </summary>
		public enum AvatarSize
		{
			Small,
			Medium,
			Large
		}

		/// <summary>
		/// Available update types.
		/// </summary>
		public enum UpdateType
		{
			UserUpdate,
			Message,
			Emote,
			TypingNotification
		}

		/// <summary>
		/// Structure containing basic friend info.
		/// </summary>
		public class Friend
		{
			public string steamid;
			public bool blocked;
			public DateTime friendSince;
		}

		/// <summary>
		/// Structure containing extensive user info.
		/// </summary>
		public class User
		{
			public string steamid;
			public ProfileVisibility profileVisibility;
			public int profileState;
			public string nickname;
			public DateTime lastLogoff;
			public string profileUrl;
			internal string avatarUrl;
			public UserStatus status;
			public string realName;
			public string primaryGroupId;
			public DateTime joinDate;
			public string locationCountryCode;
			public string locationStateCode;
			public int locationCityId;
		}

		/// <summary>
		/// Basic group info.
		/// </summary>
		public class Group
		{
			public string steamid;
			public bool inviteonly;
		}

		/// <summary>
		/// Structure containing extensive group info.
		/// </summary>
		public class GroupInfo
		{
			public string steamid;
			public DateTime creationDate;
			public string name;
			public string headline;
			public string summary;
			public string abbreviation;
			public string profileUrl;
			internal string avatarUrl;
			public string locationCountryCode;
			public string locationStateCode;
			public int locationCityId;
			public int favoriteAppId;
			public int members;
			public int usersOnline;
			public int usersInChat;
			public int usersInGame;
			public string owner;
		}

		/// <summary>
		/// Structure containing information about a single update.
		/// </summary>
		public class Update
		{
			public DateTime timestamp;
			public string origin;
			public bool localMessage;
			public UpdateType type;
			public string message;
			public UserStatus status;
			public string nick;
		}

		/// <summary>
		/// Structure containing server info.
		/// </summary>
		public class ServerInfo
		{
			public DateTime serverTime;
			public string serverTimeString;
		}

		#endregion

		/// <summary>
		/// Authenticate with a username and password. Sends the SteamGuard e-mail if it has been
		/// set up.
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="password">Password</param>
		/// <param name="emailauthcode">SteamGuard code sent by e-mail</param>
		/// <returns>Indication of the authentication status.</returns>
		public LoginStatus Authenticate(string username, string password, string emailauthcode = "")
		{
			string response = steamRequest("ISteamOAuth2/GetTokenWithCredentials/v0001",
				"client_id=DE45CD61&grant_type=password&username=" + Uri.EscapeDataString(username) + 
				"&password=" + Uri.EscapeDataString(password) + "&x_emailauthcode=" + emailauthcode + 
				"&scope=read_profile%20write_profile%20read_client%20write_client");

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				string err = (string)data["x_errorcode"];

				if (data["access_token"] != null)
				{
					accessToken = (string)data["access_token"];

					return login() ? LoginStatus.LoginSuccessful : LoginStatus.LoginFailed;
				}
				else if (err == "steamguard_code_required")
				{
					return LoginStatus.SteamGuard;
				}
				else
				{
					return LoginStatus.LoginFailed;
				}
			}
			else
			{
				return LoginStatus.LoginFailed;
			}
		}

		/// <summary>
		/// Authenticate with an access token previously retrieved with a username and password (and
		/// SteamGuard code).
		/// </summary>
		/// <param name="accessToken">Access token retrieved with credentials</param>
		/// <returns>Indication of the authentication status.</returns>
		public LoginStatus Authenticate(string accessToken)
		{
			this.accessToken = accessToken;
			return login() ? LoginStatus.LoginSuccessful : LoginStatus.LoginFailed;
		}

		/// <summary>
		/// Fetch all friends of a given user.
		/// </summary>
		/// <remarks>This function does not provide detailed information.</remarks>
		/// <param name="steamid">steamid of target user or self</param>
		/// <returns>List of friends or null on failure.</returns>
		public List<Friend> GetFriends(string steamid = null)
		{
			if (umqid == null) return null;
			if (steamid == null) steamid = this.steamid;

			string response = steamRequest("ISteamUserOAuth/GetFriendList/v0001?access_token=" + accessToken + "&steamid=" + steamid);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["friends"] != null)
				{
					List<Friend> friends = new List<Friend>();

					foreach (JObject friend in data["friends"])
					{
						Friend f = new Friend();
						f.steamid = (string)friend["steamid"];
						f.blocked = ((string)friend["relationship"]).Equals("ignored");
						f.friendSince = unixTimestamp((long)friend["friend_since"]);
						friends.Add(f);
					}

					return friends;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieve information about the specified users.
		/// </summary>
		/// <remarks>This function doesn't have the 100 users limit the original API has.</remarks>
		/// <param name="steamids">64-bit SteamIDs of users</param>
		/// <returns>Information about the specified users</returns>
		public List<User> GetUserInfo(List<string> steamids)
		{
			if (umqid == null) return null;

			string response = steamRequest("ISteamUserOAuth/GetUserSummaries/v0001?access_token=" +
				accessToken + "&steamids=" + string.Join(",", steamids.GetRange(0, Math.Min(steamids.Count, 100)).ToArray()));

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["players"] != null)
				{
					List<User> users = new List<User>();

					foreach (JObject info in data["players"])
					{
						User user = new User();

						user.steamid = (string)info["steamid"];
						user.profileVisibility = (ProfileVisibility)(int)info["communityvisibilitystate"];
						user.profileState = (int)info["profilestate"];
						user.nickname = (string)info["personaname"];
						user.lastLogoff = unixTimestamp((long)info["lastlogoff"]);
						user.profileUrl = (string)info["profileurl"];
						user.status = (UserStatus)(int)info["personastate"];

						user.avatarUrl = info["avatar"] != null ? (string)info["avatar"] : "";
						if (user.avatarUrl != null) user.avatarUrl = user.avatarUrl.Substring(0, user.avatarUrl.Length - 4);

						user.joinDate = unixTimestamp(info["timecreated"] != null ? (long)info["timecreated"] : 0);
						user.primaryGroupId = info["primaryclanid"] != null ? (string)info["primaryclanid"] : "";
						user.realName = info["realname"] != null ? (string)info["realname"] : "";
						user.locationCountryCode = info["loccountrycode"] != null ? (string)info["loccountrycode"] : "";
						user.locationStateCode = info["locstatecode"] != null ? (string)info["locstatecode"] : "";
						user.locationCityId = info["loccityid"] != null ? (int)info["loccityid"] : -1;

						users.Add(user);
					}

					// Requests are limited to 100 steamids, so issue multiple requests
					if (steamids.Count > 100)
						users.AddRange(GetUserInfo(steamids.GetRange(100, Math.Min(steamids.Count - 100, 100))));

					return users;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		public List<User> GetUserInfo(List<Friend> friends)
		{
			List<string> steamids = new List<string>(friends.Count);
			foreach (Friend f in friends) steamids.Add(f.steamid);
			return GetUserInfo(steamids);
		}

		public User GetUserInfo(string steamid = null)
		{
			if (steamid == null) steamid = this.steamid;
			return GetUserInfo(new List<string>(new string[] { steamid }))[0];
		}

		/// <summary>
		/// Retrieve the avatar of the specified user in the specified format.
		/// </summary>
		/// <param name="user">User</param>
		/// <param name="size">Requested avatar size</param>
		/// <returns>The avatar as bitmap on success or null on failure.</returns>
		public Bitmap GetUserAvatar(User user, AvatarSize size = AvatarSize.Small)
		{
			if (user.avatarUrl.Length == 0) return null;

			try
			{
				WebClient client = new WebClient();

				Stream stream;
				if (size == AvatarSize.Small)
					stream = client.OpenRead(user.avatarUrl + ".jpg");
				else if (size == AvatarSize.Medium)
					stream = client.OpenRead(user.avatarUrl + "_medium.jpg");
				else
					stream = client.OpenRead(user.avatarUrl + "_full.jpg");

				Bitmap avatar = new Bitmap(stream);
				stream.Flush();
				stream.Close();

				return avatar;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieve the avatar of the specified group in the specified format.
		/// </summary>
		/// <param name="group">Group</param>
		/// <param name="size">Requested avatar size</param>
		/// <returns>The avatar as bitmap on success or null on failure.</returns>
		public Bitmap GetGroupAvatar(GroupInfo group, AvatarSize size = AvatarSize.Small)
		{
			User user = new User();
			user.avatarUrl = group.avatarUrl;
			return GetUserAvatar(user, size);
		}

		/// <summary>
		/// Fetch all groups of a given user.
		/// </summary>
		/// <param name="steamid">SteamID</param>
		/// <returns>List of groups.</returns>
		public List<Group> GetGroups(string steamid = null)
		{
			if (umqid == null) return null;
			if (steamid == null) steamid = this.steamid;

			string response = steamRequest("ISteamUserOAuth/GetGroupList/v0001?access_token=" + accessToken + "&steamid=" + steamid);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["groups"] != null)
				{
					List<Group> groups = new List<Group>();

					foreach (JObject info in data["groups"])
					{
						Group group = new Group();

						group.steamid = (string)info["steamid"];
						group.inviteonly = ((string)info["permission"]).Equals("2");

						if (((string)info["relationship"]).Equals("Member"))
							groups.Add(group);
					}

					return groups;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieve information about the specified groups.
		/// </summary>
		/// <param name="steamids">64-bit SteamIDs of groups</param>
		/// <returns>Information about the specified groups</returns>
		public List<GroupInfo> GetGroupInfo(List<string> steamids)
		{
			if (umqid == null) return null;

			string response = steamRequest("ISteamUserOAuth/GetGroupSummaries/v0001?access_token=" + accessToken + 
				"&steamids=" + string.Join(",", steamids.GetRange(0, Math.Min(steamids.Count, 100)).ToArray()));

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["groups"] != null)
				{
					List<GroupInfo> groups = new List<GroupInfo>();

					foreach (JObject info in data["groups"])
					{
						GroupInfo group = new GroupInfo();

						group.steamid = (string)info["steamid"];
						group.creationDate = unixTimestamp((long)info["timecreated"]);
						group.name = (string)info["name"];
						group.profileUrl = "http://steamcommunity.com/groups/" + (string)info["profileurl"];
						group.usersOnline = (int)info["usersonline"];
						group.usersInChat = (int)info["usersinclanchat"];
						group.usersInGame = (int)info["usersingame"];
						group.owner = (string)info["ownerid"];
						group.members = (int)info["users"];

						group.avatarUrl = (string)info["avatar"];
						if (group.avatarUrl != null) group.avatarUrl = group.avatarUrl.Substring(0, group.avatarUrl.Length - 4);

						group.headline = info["headline"] != null ? (string)info["headline"] : "";
						group.summary = info["summary"] != null ? (string)info["summary"] : "";
						group.abbreviation = info["abbreviation"] != null ? (string)info["abbreviation"] : "";
						group.locationCountryCode = info["loccountrycode"] != null ? (string)info["loccountrycode"] : "";
						group.locationStateCode = info["locstatecode"] != null ? (string)info["locstatecode"] : "";
						group.locationCityId = info["loccityid"] != null ? (int)info["loccityid"] : -1;
						group.favoriteAppId = info["favoriteappid"] != null ? (int)info["favoriteappid"] : -1;

						groups.Add(group);
					}

					// Requests are limited to 100 steamids, so issue multiple requests
					if (steamids.Count > 100)
						groups.AddRange(GetGroupInfo(steamids.GetRange(100, Math.Min(steamids.Count - 100, 100))));

					return groups;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		public List<GroupInfo> GetGroupInfo(List<Group> groups)
		{
			List<string> steamids = new List<string>(groups.Count);
			foreach (Group g in groups) steamids.Add(g.steamid);
			return GetGroupInfo(steamids);
		}

		public GroupInfo GetGroupInfo(string steamid)
		{
			return GetGroupInfo(new List<string>(new string[] { steamid }))[0];
		}

		/// <summary>
		/// Let a user know you're typing a message. Should be called periodically.
		/// </summary>
		/// <param name="steamid">Recipient of notification</param>
		/// <returns>Returns a boolean indicating success of the request.</returns>
		public bool SendTypingNotification(User user)
		{
			if (umqid == null) return false;

			string response = steamRequest("ISteamWebUserPresenceOAuth/Message/v0001", "?access_token=" + accessToken + 
				"&umqid=" + umqid + "&type=typing&steamid_dst=" + user.steamid);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				return data["error"] != null && ((string)data["error"]).Equals("OK");
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Send a text message to the specified user.
		/// </summary>
		/// <param name="steamid">Recipient of message</param>
		/// <param name="message">Message contents</param>
		/// <returns>Returns a boolean indicating success of the request.</returns>
		public bool SendMessage(User user, string message)
		{
			if (umqid == null) return false;

			string response = steamRequest("ISteamWebUserPresenceOAuth/Message/v0001", "?access_token=" + accessToken + 
				"&umqid=" + umqid + "&type=saytext&text=" + Uri.EscapeDataString(message) + "&steamid_dst=" + user.steamid);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				return data["error"] != null && ((string)data["error"]).Equals("OK");
			}
			else
			{
				return false;
			}
		}

		public bool SendMessage(string steamid, string message)
		{
			User user = new User();
			user.steamid = steamid;
			return SendMessage(user, message);
		}

		/// <summary>
		/// Check for updates and new messages.
		/// </summary>
		/// <returns>A list of updates.</returns>
		public List<Update> Poll()
		{
			if (umqid == null) return null;

			string response = steamRequest("ISteamWebUserPresenceOAuth/Poll/v0001", "?access_token=" + accessToken + 
				"&umqid=" + umqid + "&message=" + message);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (((string)data["error"]).Equals("OK"))
				{
					message = (int)data["messagelast"];

					List<Update> updates = new List<Update>();

					foreach (JObject info in data["messages"])
					{
						Update update = new Update();

						update.timestamp = unixTimestamp((long)info["timestamp"]);
						update.origin = (string)info["steamid_from"];

						string type = (string)info["type"];
						if (type.Equals("saytext") || type.Equals("my_saytext") || type.Equals("emote"))
						{
							update.type = type.Equals("emote") ? UpdateType.Emote : UpdateType.Message;
							update.message = (string)info["text"];
							update.localMessage = type.Equals("my_saytext");
						}
						else if (type.Equals("typing"))
						{
							update.type = UpdateType.TypingNotification;
							update.message = (string)info["text"]; // Not sure if this is useful
						}
						else if (type.Equals("personastate"))
						{
							update.type = UpdateType.UserUpdate;
							update.status = (UserStatus)(int)info["persona_state"];
							update.nick = (string)info["persona_name"];
						}
						else
						{
							continue;
						}

						updates.Add(update);
					}

					return updates;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieves information about the server.
		/// </summary>
		/// <returns>Returns a structure with the information.</returns>
		public ServerInfo GetServerInfo()
		{
			string response = steamRequest("ISteamWebAPIUtil/GetServerInfo/v0001");

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["servertime"] != null)
				{
					ServerInfo info = new ServerInfo();
					info.serverTime = unixTimestamp((long)data["servertime"]);
					info.serverTimeString = (string)data["servertimestring"];
					return info;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Helper function to complete the login procedure and check the credentials.
		/// </summary>
		/// <returns>Whether the login was successful or not.</returns>
		private bool login()
		{
			string response = steamRequest("ISteamWebUserPresenceOAuth/Logon/v0001",
				"?access_token=" + accessToken);

			if (response != null)
			{
				JObject data = JObject.Parse(response);

				if (data["umqid"] != null)
				{
					steamid = (string)data["steamid"];
					umqid = (string)data["umqid"];
					message = (int)data["message"];
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Helper function to perform Steam API requests.
		/// </summary>
		/// <param name="get">Path URI</param>
		/// <param name="post">Post data</param>
		/// <returns>Web response info</returns>
		private string steamRequest(string get, string post = null)
		{
			System.Net.ServicePointManager.Expect100Continue = false;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.steampowered.com/" + get);
			request.Host = "api.steampowered.com:443";
			request.ProtocolVersion = HttpVersion.Version11;
			request.Accept = "*/*";
			request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
			request.Headers[HttpRequestHeader.AcceptLanguage] = "en-us";
			request.UserAgent = "Steam 1291812 / iPhone";

			if (post != null)
			{
				request.Method = "POST";
				byte[] postBytes = Encoding.ASCII.GetBytes(post);
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = postBytes.Length;

				Stream requestStream = request.GetRequestStream();
				requestStream.Write(postBytes, 0, postBytes.Length);
				requestStream.Close();

				message++;
			}

			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				if ((int)response.StatusCode != 200) return null;

				string src = new StreamReader(response.GetResponseStream()).ReadToEnd();
				response.Close();
				return src;
			}
			catch (WebException e)
			{
				Console.WriteLine("Steam request failed: " + e.Message);
				return null;
			}
		}

		private DateTime unixTimestamp(long timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}
	}
}