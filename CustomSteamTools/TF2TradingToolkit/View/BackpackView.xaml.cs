using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CustomSteamTools;
using CustomSteamTools.Backpacks;
using CustomSteamTools.Commands;
using CustomSteamTools.Friends;
using CustomSteamTools.Utils;
using TF2TradingToolkit.ViewModel;
using UltimateUtil;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for BackpackView.xaml
	/// </summary>
	public partial class BackpackView : UserControl
	{
		public const bool FORCE_LOAD = false;

		public ObservableCollection<PlayerViewModel> AvailablePlayers
		{ get; private set; }

		public ObservableCollection<PlayerViewModel> Friends
		{ get; private set; }

		public ObservableCollection<BackpackPageControl> BackpackPages
		{ get; private set; }

		public PlayerViewModel ActivePlayer
		{ get; private set; }

		public BackgroundWorker FriendsLoader
		{ get; private set; }

		public BackgroundWorker BackpackLoader
		{ get; private set; }

		public Backpack CurrentBackpack
		{ get; private set; }
		
		private bool _isSelecting = false;

		public BackpackView()
		{
			InitializeComponent();

			AvailablePlayers = new ObservableCollection<PlayerViewModel>();
			PlayersCombo.ItemsSource = AvailablePlayers;

			Friends = new ObservableCollection<PlayerViewModel>();
			FriendsList.ItemsSource = Friends;

			BackpackPages = new ObservableCollection<BackpackPageControl>();
			BackpackItemsControl.ItemsSource = BackpackPages;

			FriendsLoader = new BackgroundWorker();
			FriendsLoader.DoWork += FriendsLoader_DoWork;
			FriendsLoader.RunWorkerCompleted += FriendsLoader_RunWorkerCompleted;

			BackpackLoader = new BackgroundWorker();
			BackpackLoader.DoWork += BackpackLoader_DoWork;
			BackpackLoader.RunWorkerCompleted += BackpackLoader_RunWorkerCompleted;
		}

		public void PostLoad()
		{
			foreach (Player p in DataManager.AllLoadedPlayers)
			{
				AvailablePlayers.Add(new PlayerViewModel(p));
			}

			SteamIDBox.Text = Settings.Instance.HomeSteamID64;
			LoadFriends(Settings.Instance.HomeSteamID64);
			BackpackBtn_Click(BackpackBtn, null);
		}

		public void LoadFriends(string steamid)
		{
			FriendLoadingBar.Visibility = Visibility.Visible;
			FriendsLoader.RunWorkerAsync(steamid);
		}

		public void OpenBackpack(string steamid)
		{
			if (ActivePlayer == null && steamid == null)
			{
				return;
			}

			FriendLoadingBar.Visibility = Visibility.Visible;

			Thread thread = new Thread(() => {
				while (BackpackLoader.IsBusy)
				{
					Thread.Sleep(20);
				}

				BackpackLoader.RunWorkerAsync(steamid);
			});
			thread.Start();
		}

		public void UpdateUIBackpack()
		{
			if (CurrentBackpack == null)
			{
				BackpackTitleText.Dispatcher.Invoke(() => 
				{
					BackpackTitleText.Text = "No backpack opened.";
				});
				BackpackItemsControl.Dispatcher.Invoke(() => { BackpackPages.Clear(); });
				return;
			}

			BackpackTitleText.Dispatcher.Invoke(() => 
			{
				BackpackTitleText.Text = "Backpack for " + ActivePlayer?.PersonaName ?? "[NULL]";
			});

			BackpackItemsControl.Dispatcher.Invoke(() => {
				BackpackPages.Clear();
				BackpackPageControl newItems = new BackpackPageControl(CurrentBackpack, -1);
				newItems.Margin = new Thickness(5);
				BackpackPages.Add(newItems);

				for (int i = 0; i < CurrentBackpack.Pages.Length; i++)
				{
					BackpackPageControl bpc = new BackpackPageControl(CurrentBackpack, i);
					bpc.Margin = new Thickness(5);
					BackpackPages.Add(bpc);
				}
			});

			SlotCountText.Dispatcher.Invoke(() => {
				SlotCountText.Text = "(" + CurrentBackpack.SlotCount.ToString() + " slots)";
			});

			PriceRange networth = PriceRange.Zero;
			Price totalpure = Price.Zero;
			foreach (ItemInstance inst in CurrentBackpack.GetAllItems())
			{
				if (!inst.Tradable)
				{
					continue;
				}

				var checkedPrice = PriceChecker.GetPriceFlagged(inst);
				PriceRange? price = checkedPrice.Result;

				if (price != null)
				{
					networth += price.Value;
				}

				if (inst.Item.IsCurrency())
				{
					totalpure += inst.Item.GetCurrencyPrice();
				}
			}

			NetWorthText.Dispatcher.Invoke(() => {
				NetWorthText.Text = "Net Worth: " + networth.ToString();
				NetWorthText.ToolTip = "USD: " + networth.ToStringUSD();
			});

			TotalPureText.Dispatcher.Invoke(() => {
				TotalPureText.Text = "Total Pure: " + totalpure.ToString();
				TotalPureText.ToolTip = "USD: " + totalpure.TotalUSD.ToCurrency();
			});
		}

		private void FriendsLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			string steamid = e.Result as string;
			_isSelecting = true;
			PlayersCombo.SelectedIndex = AvailablePlayers.IndexOf((pvm) => pvm.SteamID == steamid);
			_isSelecting = false;

			FriendLoadingBar.Visibility = Visibility.Collapsed;
		}

		private void FriendsLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			string steamid = e.Argument as string;
			PlayerList res = CmdFriends.GetFriendsList(steamid, FORCE_LOAD);

			FriendsList.Dispatcher.Invoke(() => {
				Friends.Clear();
				if (res == null)
				{
					return;
				}

				foreach (Player p in res)
				{
					Friends.Add(new PlayerViewModel(p));
				}
			});

			PlayersCombo.Dispatcher.Invoke(() => {
				AvailablePlayers.Clear();
				foreach (Player p in DataManager.AllLoadedPlayers)
				{
					AvailablePlayers.Add(new PlayerViewModel(p));
				}
			});

			e.Result = steamid;
		}

		private void BackpackLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			FriendLoadingBar.Dispatcher.Invoke(() => {
				FriendLoadingBar.Visibility = Visibility.Collapsed;
			});
		}

		private void BackpackLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			string steamid = (e.Argument as string) ?? ActivePlayer.SteamID;
			
			if (e.Argument == null && !DataManager.AllLoadedPlayers.Contains(steamid))
			{
				CmdPlayer.GetPlayerInfo(steamid, FORCE_LOAD);

				PlayersCombo.Dispatcher.Invoke(() => {
					AvailablePlayers.Clear();
					foreach (Player p in DataManager.AllLoadedPlayers)
					{
						AvailablePlayers.Add(new PlayerViewModel(p));
					}
				});
			}

			Backpack bp = CmdBackpack.GetBackpack(steamid);
			Dispatcher.Invoke(() => { CurrentBackpack = bp; });
			UpdateUIBackpack();
		}

		private void FriendsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FriendsList.SelectedIndex == -1 || _isSelecting)
			{
				return;
			}

			ActivePlayer = FriendsList.SelectedItem as PlayerViewModel;
			SteamIDBox.Text = ActivePlayer.SteamID;
			OpenBackpack(null);
		}

		private void BackpackBtn_Click(object sender, RoutedEventArgs e)
		{
			if (SteamIDBox.Text.IsNullOrWhitespace())
			{
				return;
			}

			string steamid = SteamIDBox.Text;
			OpenBackpack(steamid);
		}

		private void PlayersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PlayersCombo.SelectedIndex == -1 || _isSelecting)
			{
				return;
			}

			PlayerViewModel vm = PlayersCombo.SelectedItem as PlayerViewModel;

			if (vm != null)
			{
				LoadFriends(vm.SteamID);
			}
		}
	}
}
