using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		private InitWindow _initializerWindow;

		public bool AutoExiting
		{ get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		private void RefreshDropDownBtn_Click(object sender, RoutedEventArgs e)
		{
			RefreshDropDownBtn.ContextMenu.IsOpen = true;
		}

		private void RefreshDropDownBtn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (AutoExiting)
			{
				return;
			}

			if (RefreshDropDownBtn.ContextMenu == null)
			{
				RefreshDropDownBtn.ContextMenu = FindResource("K_RefreshMenu") as ContextMenu;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_initializerWindow = new InitWindow(false);
			bool? loaded = _initializerWindow.ShowDialog();

			if (loaded != true)
			{
				AutoExiting = true;
				Close();
			}

			ItemsView.PostLoad(this);
			BackpackView.PostLoad(this);
			ClassifiedsView.PostLoad(this);
		}

		private void RefreshAllBtn_Click(object sender, RoutedEventArgs e)
		{
			_initializerWindow = new InitWindow(true);
			bool? loaded = _initializerWindow.ShowDialog();

			if (loaded != true)
			{
				AutoExiting = true;
				Close();
			}

			ItemsView.PostLoad(this);
			BackpackView.PostLoad(this);
			ClassifiedsView.PostLoad(this);
		}

		private void RefreshItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item == null)
			{
				return;
			}

			switch (item.Tag as string)
			{
			case "SchemaRefreshItem":
				break;
			case "PricesRefreshItem":
				break;
			case "BackpackMyRefreshItem":
				break;
			case "BackpackOtherRefreshItem":
				break;
			case "MarketRefreshItem":
				break;
			case "PlayerFriendsRefreshItem":
				break;
			case "PlayerAllRefreshItem":
				break;
			default:
				break;
			}
		}

		private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			const int ITEMS_TAB_INDEX = 0;
			const int CLASSIFIEDS_TAB_INDEX = 2;

			if (MainTabControl.SelectedIndex == ITEMS_TAB_INDEX)
			{
				ItemsView.ItemSearchBox.Focus();
			}
			else if (MainTabControl.SelectedIndex == CLASSIFIEDS_TAB_INDEX)
			{
				ClassifiedsView.ItemSearchBox.Focus();
			}
		}
	}
}
