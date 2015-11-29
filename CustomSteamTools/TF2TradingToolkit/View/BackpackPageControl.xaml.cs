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

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for BackpackPage.xaml
	/// </summary>
	public partial class BackpackPageControl : UserControl
	{
		public ItemInstance[] Items
		{ get; private set; }

		public int PageNum
		{ get; private set; }

		public ObservableCollection<ItemSlotViewModel> ItemsVM
		{ get; private set; }

		public BackpackPageControl(Backpack backpack, int page)
		{
			InitializeComponent();

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
				ItemsVM.Add(new ItemSlotViewModel(inst));
			}

			PageControl.ItemsSource = ItemsVM;
		}
	}
}
