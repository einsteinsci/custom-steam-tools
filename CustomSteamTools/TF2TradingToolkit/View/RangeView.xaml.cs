using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CustomSteamTools;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Commands;
using CustomSteamTools.Lookup;
using CustomSteamTools.Schema;
using CustomSteamTools.Utils;

using TF2TradingToolkit.ViewModel;
using UltimateUtil;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for RangeView.xaml
	/// </summary>
	public partial class RangeView : UserControl
	{
		public MainWindow OwnerWindow
		{ get; private set; }

		public DealsFilters Filters
		{ get; private set; }

		public ObservableCollection<PricedViewModel> Pricings
		{ get; private set; }

		public Price MinPrice
		{ get; private set; }

		public Price MaxPrice
		{ get; private set; }

		public PriceRange Range => new PriceRange(MinPrice, MaxPrice);

		public PriceUnitViewModel SelectedMinUnit => MinPriceUnitCombo.SelectedItem as PriceUnitViewModel;
		public PriceUnitViewModel SelectedMaxUnit => MaxPriceUnitCombo.SelectedItem as PriceUnitViewModel;

		public bool ValidMin
		{
			get
			{
				double d;
				return double.TryParse(MinPriceBox.Text, out d);
			}
		}
		public bool ValidMax
		{
			get
			{
				double d;
				return double.TryParse(MaxPriceBox.Text, out d);
			}
		}

		private bool _loaded = false;

		public RangeView()
		{
			InitializeComponent();

			Pricings = new ObservableCollection<PricedViewModel>();
			ResultsList.ItemsSource = Pricings;
		}

		public void PostLoad(MainWindow window)
		{
			MinPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_REF));
			MinPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_KEYS));
			MinPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_CASH));

			MaxPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_REF));
			MaxPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_KEYS));
			MaxPriceUnitCombo.Items.Add(new PriceUnitViewModel(Price.CURRENCY_CASH));

			Price min = new Price(Settings.Instance.RangeLastMinPrice);
			Price max = new Price(Settings.Instance.RangeLastMaxPrice);

			if (min.TotalKeys >= 2.0)
			{
				MinPriceBox.Text = min.TotalKeys.ToString();
				MinPriceUnitCombo.SelectedIndex = 1;
			}
			else
			{
				MinPriceBox.Text = min.TotalRefined.ToString();
				MinPriceUnitCombo.SelectedIndex = 0;
			}

			if (max.TotalKeys >= 2.0)
			{
				MaxPriceBox.Text = max.TotalKeys.ToString();
				MaxPriceUnitCombo.SelectedIndex = 1;
			}
			else
			{
				MaxPriceBox.Text = max.TotalRefined.ToString();
				MaxPriceUnitCombo.SelectedIndex = 0;
			}

			_loaded = true;

			OwnerWindow = window;

			Filters = new DealsFilters();

			_updateMinPrice();
			_updateMaxPrice();

			RunFilter();
		}

		public void RunFilter()
		{
			if (!_loaded)
			{
				return;
			}

			if (!ValidMin || !ValidMax)
			{
				return;
			}

			const int MAX_SHOWN = 500;

			List<ItemPricing> cmdRes = CmdRange.GetInRange(Range, Filters);

			List<ItemPricing> buf = new List<ItemPricing>();
			for (int i = 0; i < MAX_SHOWN && i < cmdRes.Count; i++)
			{
				buf.Add(cmdRes[i]);
			}
			buf.Sort((a, b) => a.Pricing.Low.TotalRefined.CompareTo(b.Pricing.Low.TotalRefined));

			Pricings.Clear();
			foreach (ItemPricing p in buf)
			{
				ItemPriceInfo info = new ItemPriceInfo(p.Item, p.Quality);

				if (info.Quality == Quality.Unusual && p.PriceIndex > 0)
				{
					info.Unusual = DataManager.Schema.GetUnusual(p.PriceIndex);
				}

				PricedViewModel vm = new PricedViewModel(info, p.Pricing);

				Pricings.Add(vm);
			}

			if (cmdRes.Count > MAX_SHOWN)
			{
				PricingsCountTxt.Text = Pricings.Count + " pricings shown (" +
					cmdRes.Count + " results found)";
			}
			else
			{
				PricingsCountTxt.Text = Pricings.Count + " pricings found matching filters";
			}
		}

		private void _updateMinPrice()
		{
			if (!_loaded)
			{
				return;
			}

			if (ValidMin)
			{
				double d = double.Parse(MinPriceBox.Text);
				MinPrice = new Price(d, SelectedMinUnit.Unit);

				MinPriceBox.BorderBrush = new SolidColorBrush(Colors.DarkGray);

				string tooltip = "Equivalent to:";
				if (SelectedMinUnit.Unit != Price.CURRENCY_REF)
				{
					tooltip += Environment.NewLine + " - " + MinPrice.TotalRefined.ToString() + " ref";
				}
				if (SelectedMinUnit.Unit != Price.CURRENCY_KEYS)
				{
					tooltip += Environment.NewLine + " - " + MinPrice.TotalKeys.ToString("F2") + " keys";
				}
				if (SelectedMinUnit.Unit != Price.CURRENCY_CASH)
				{
					tooltip += Environment.NewLine + " - " + MinPrice.TotalUSD.ToCurrency();
				}
				MinPriceBox.ToolTip = tooltip;

				Settings.Instance.RangeLastMinPrice = MinPrice.TotalRefined;
				Settings.Instance.SaveOnOtherThread();
			}
			else
			{
				MinPriceBox.BorderBrush = new SolidColorBrush(Colors.Red);
				MinPriceBox.ToolTip = "Not a valid number";
			}

			//bool swapped = MaxPrice < MinPrice;
			//SearchBtn.IsEnabled = !swapped;
			//if (swapped)
			//{
			//	MinPriceBox.BorderBrush = new SolidColorBrush(Colors.Red);
			//
			//	SearchBtn.ToolTip = "Max price is greater than min price.";
			//	MinPriceBox.ToolTip = "Min price cannot be above max price.";
			//}
			//else
			//{
			//	SearchBtn.ToolTip = null;
			//	MinPriceBox.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			//}
		}

		private void _updateMaxPrice()
		{
			if (!_loaded)
			{
				return;
			}

			if (ValidMax)
			{
				double d = double.Parse(MaxPriceBox.Text);
				MaxPrice = new Price(d, SelectedMaxUnit.Unit);

				MaxPriceBox.BorderBrush = new SolidColorBrush(Colors.DarkGray);

				string tooltip = "Equivalent to:";
				if (SelectedMaxUnit.Unit != Price.CURRENCY_REF)
				{
					tooltip += Environment.NewLine + " - " + MaxPrice.TotalRefined + " ref";
				}
				if (SelectedMaxUnit.Unit != Price.CURRENCY_KEYS)
				{
					tooltip += Environment.NewLine + " - " + MaxPrice.TotalKeys.ToString("F2") + " keys";
				}
				if (SelectedMaxUnit.Unit != Price.CURRENCY_CASH)
				{
					tooltip += Environment.NewLine + " - " + MaxPrice.TotalUSD.ToCurrency();
				}
				MaxPriceBox.ToolTip = tooltip;

				Settings.Instance.RangeLastMaxPrice = MaxPrice.TotalRefined;
				Settings.Instance.SaveOnOtherThread();
			}
			else
			{
				MaxPriceBox.BorderBrush = new SolidColorBrush(Colors.Red);
				MaxPriceBox.ToolTip = "Not a valid number";
			}

			//bool swapped = MaxPrice < MinPrice;
			//SearchBtn.IsEnabled = !swapped;
			//if (swapped)
			//{
			//	MaxPriceBox.BorderBrush = new SolidColorBrush(Colors.Red);
			//
			//	SearchBtn.ToolTip = "Max price is greater than min price.";
			//	MaxPriceBox.ToolTip = "Max price cannot be above min price.";
			//}
			//else
			//{
			//	SearchBtn.ToolTip = null;
			//	MaxPriceBox.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			//}
		}

		private void MinPriceBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_updateMinPrice();
			RunFilter();
		}

		private void MaxPriceBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_updateMaxPrice();
			RunFilter();
		}

		private void MinPriceUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_updateMinPrice();
			RunFilter();
		}

		private void MaxPriceUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_updateMaxPrice();
			RunFilter();
		}

		private void Qualities_MultiSelectionChanged(object sender, MultiQualitySelectorEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (e.ActionType == SelectorActionType.Add)
			{
				Filters.Qualities.Add(e.Selection);
			}
			else
			{
				Filters.Qualities.Remove(e.Selection);
			}

			CraftableCheck.IsEnabled = Filters.Qualities.Contains(Quality.Unique) || Filters.Qualities.IsEmpty();
			BotkillerCheck.IsEnabled = Filters.Qualities.Contains(Quality.Strange) || Filters.Qualities.IsEmpty();

			RunFilter();
		}

		private void Slots_SelectionChanged(object sender, MultiSlotSelectorEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (e.ActionType == SelectorActionType.Add)
			{
				Filters.Slots.Add(e.Selection);
			}
			else
			{
				Filters.Slots.Remove(e.Selection);
			}
			RunFilter();
		}

		private void Classes_SelectionChanged(object sender, MultiClassSelectorEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (e.ActionType == SelectorActionType.Add)
			{
				Filters.Classes.Add(e.Selection);
			}
			else
			{
				Filters.Classes.Remove(e.Selection);
			}
			RunFilter();
		}

		private void AllClassCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.AllowAllClass = AllClassCheck.IsChecked ?? true;
			RunFilter();
		}

		private void CraftableCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Craftable = CraftableCheck.IsChecked;
			RunFilter();
		}

		private void HalloweenCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Halloween = HalloweenCheck.IsChecked;
			RunFilter();
		}

		private void BotkillerCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Botkiller = BotkillerCheck.IsChecked;
			RunFilter();
		}

		private void SearchBtn_Click(object sender, RoutedEventArgs e)
		{
			RunFilter();
		}

		private void ShowClassifiedsItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			ItemPriceInfo info = item?.Tag as ItemPriceInfo;

			if (info != null)
			{
				OwnerWindow.MainTabControl.SelectedIndex = 2;
				OwnerWindow.ClassifiedsView.ShowClassifieds(info);
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
