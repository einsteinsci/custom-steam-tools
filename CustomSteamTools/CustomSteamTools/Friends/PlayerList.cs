using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil;

namespace CustomSteamTools.Friends
{
	public class PlayerList : IList<Player>
	{
		private List<Player> _friends;

		#region IList props
		public Player this[int index]
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

		public int Count => _friends.Count;

		public bool IsReadOnly => (_friends as ICollection<Player>).IsReadOnly;
		#endregion IList props

		public PlayerList()
		{
			_friends = new List<Player>();
		}
		public PlayerList(int capacity)
		{
			_friends = new List<Player>(capacity);
		}
		public PlayerList(IEnumerable<Player> contents)
		{
			_friends = new List<Player>(contents);
		}
		public PlayerList(params Player[] friends) : this((IEnumerable<Player>)friends)
		{ }

		public Player GetFriendBySteamID(string steamID64)
		{
			return _friends.FirstOrDefault((f) => f.SteamID64.EqualsIgnoreCase(steamID64));
		}

		public List<Player> GetOnline()
		{
			return _friends.FindAll((f) => f.PersonaState != PersonaState.Offline);
		}

		public override string ToString()
		{
			return "{0} players ({1} online)".Fmt(_friends.Count, GetOnline().Count);
		}

		public bool Contains(string steamid)
		{
			return _friends.Exists((p) => p.SteamID64.EqualsIgnoreCase(steamid));
		}

		public void RemoveAll(Predicate<Player> pred)
		{
			_friends.RemoveAll(pred);
		}

		public void Sort()
		{
			_friends.Sort();
		}
		public void Sort(Comparison<Player> comparer)
		{
			_friends.Sort(comparer);
		}
		public void Sort(IComparer<Player> comparer)
		{
			_friends.Sort(comparer);
		}

		#region IList methods
		public void Add(Player p)
		{
			_friends.Add(p);
		}

		public void Clear()
		{
			_friends.Clear();
		}

		public bool Contains(Player p)
		{
			return _friends.Contains(p);
		}

		public void CopyTo(Player[] array, int startIndex)
		{
			_friends.CopyTo(array, startIndex);
		}

		public IEnumerator<Player> GetEnumerator()
		{
			return _friends.GetEnumerator();
		}

		public int IndexOf(Player p)
		{
			return _friends.IndexOf(p);
		}

		public void Insert(int index, Player p)
		{
			_friends.Insert(index, p);
		}

		public bool Remove(Player p)
		{
			return _friends.Remove(p);
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

		public class PlayerGameStateComparer : IComparer<Player>
		{
			public int Compare(Player x, Player y)
			{
				int xn = x.PersonaState.GetTier();
				int yn = y.PersonaState.GetTier();

				if (x.IsInGame && x.IsOnline)
				{
					xn = 10;
				}
				if (y.IsInGame && y.IsOnline)
				{
					yn = 10;
				}

				if (xn != yn)
				{
					return yn - xn;
				}

				return y.Name.CompareTo(x.Name);
			}
		}
	}
}
