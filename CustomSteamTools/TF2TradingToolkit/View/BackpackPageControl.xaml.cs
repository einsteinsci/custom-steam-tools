using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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
using CustomSteamTools.Backpacks;
using TF2TradingToolkit.ViewModel;
using UltimateUtil;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for BackpackPage.xaml
	/// </summary>
	public partial class BackpackPageControl : UserControl
	{
		public BackpackView OwnerView
		{ get; private set; }

		public ItemInstance[] Items
		{ get; private set; }

		public int PageNum
		{ get; private set; }

		public ObservableCollection<ItemSlotViewModel> ItemsVM
		{ get; private set; }

		public BackpackPageControl(BackpackView view, Backpack backpack, int page)
		{
			InitializeComponent();
			OwnerView = view;

			PageNum = page;
			if (PageNum == -1)
			{
				Items = backpack.NewItems.ToArray();
			}
			else
			{
				Items = backpack.Pages[page].Items;
			}

			ItemsVM = new ObservableCollection<ItemSlotViewModel>();
			foreach (ItemInstance inst in Items)
			{
				ItemsVM.Add(new ItemSlotViewModel(inst, backpack));
			}

			PageControl.ItemsSource = ItemsVM;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (PageNum == -1 && Items.HasItems())
			{
				PageNumTxt.Text = "New Items";
			}
			else if (PageNum == -1)
			{
				PageNumTxt.Visibility = Visibility.Collapsed;
			}
			else
			{
				PageNumTxt.Text = "Page " + (PageNum + 1).ToString();
			}
		}

		private void SlotShowClassifiedsItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			ItemInstance inst = item.Tag as ItemInstance;

			if (inst != null)
			{
				OwnerView.OwnerWindow.MainTabControl.SelectedIndex = 2;
				OwnerView.OwnerWindow.ClassifiedsView.ShowClassifieds(inst.ToPriceInfo());
			}
		}

		private void FollowLink_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			string link = item.Tag as string;

			if (link != null)
			{
				Util.OpenLink(link);
			}
		}
	}
}
