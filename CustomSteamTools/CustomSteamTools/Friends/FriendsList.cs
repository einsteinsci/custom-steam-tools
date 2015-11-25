using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools.Friends
{
	public class FriendsList : IList<Friend>
	{
		private List<Friend> _friends;

		#region IList props
		public Friend this[int index]
		{
			get
			{
				return _friends[index];
			}
			set
			{
				_friends[index] = value;
			}
		}

		int ICollection<Friend>.Count => _friends.Count;

		public bool IsReadOnly => (_friends as ICollection<Friend>).IsReadOnly;
		#endregion IList props

		public FriendsList()
		{
			_friends = new List<Friend>();
		}
		public FriendsList(int capacity)
		{
			_friends = new List<Friend>(capacity);
		}
		public FriendsList(IEnumerable<Friend> contents)
		{
			_friends = new List<Friend>(contents);
		}
		public FriendsList(params Friend[] friends) : this((IEnumerable<Friend>)friends)
		{ }

		public Friend GetFriendBySteamID(string steamID64)
		{
			return _friends.FirstOrDefault((f) => f.SteamID64.EqualsIgnoreCase(steamID64));
		}

		public List<Friend> GetOnline()
		{
			return _friends.FindAll((f) => f.PersonaState != PersonaState.Offline);
		}

		public override string ToString()
		{
			return "{0} friends ({1} online)".Fmt(_friends.Count, GetOnline().Count);
		}

		#region IList methods
		public void Add(Friend friend)
		{
			_friends.Add(friend);
		}

		public void Clear()
		{
			_friends.Clear();
		}

		public bool Contains(Friend friend)
		{
			return _friends.Contains(friend);
		}

		public void CopyTo(Friend[] array, int startIndex)
		{
			_friends.CopyTo(array, startIndex);
		}

		public IEnumerator<Friend> GetEnumerator()
		{
			return _friends.GetEnumerator();
		}

		public int IndexOf(Friend friend)
		{
			return _friends.IndexOf(friend);
		}

		public void Insert(int index, Friend friend)
		{
			_friends.Insert(index, friend);
		}

		public bool Remove(Friend friend)
		{
			return _friends.Remove(friend);
		}

		public void RemoveAt(int index)
		{
			_friends.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion IList methods
	}
}
