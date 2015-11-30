using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
using System.Windows.Shell;
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
	/// Interaction logic for ClassifiedsView.xaml
	/// </summary>
	public partial class ClassifiedsView : UserControl
	{
		public MainWindow OwnerWindow
		{ get; private set; }

		public ItemViewModel ActiveItem => ItemSearchResultList.SelectedItem as ItemViewModel;

		public ItemPriceInfo Info
		{ get; private set; }

		public ListingProperties Props => Info.ToListingProperties();

		public ObservableCollection<ItemViewModel> AvailableItems
		{ get; private set; }

		public ObservableCollection<ClassifiedsListingViewModel> Sellers
		{ get; private set; }

		public ObservableCollection<ClassifiedsListingViewModel> Buyers
		{ get; private set; }

		public ClassifiedsListingViewModel BestSeller
		{ get; private set; }

		public ClassifiedsListingViewModel BestBuyer
		{ get; private set; }

		public PriceRange? EvaluatedPrice
		{ get; private set; }

		public bool IsMarket
		{ get; private set; }

		public BackgroundWorker ClassifiedsLoader
		{ get; private set; }

		public DealsFilters Filters
		{ get; private set; }

		public ObservableCollection<SaleViewModel> DealsResults
		{ get; private set; }

		public ObservableCollection<DealExclusionViewModel> DealsExcluded
		{ get; private set; }

		public bool IsMinProfitValid
		{
			get
			{
				if (!_loaded)
				{
					return true;
				}

				double buf;
				return double.TryParse(DealsMinProfitBox.Text, out buf);
			}
		}

		public Brush MinProfitBorder => new SolidColorBrush(IsMinProfitValid ? Colors.Gray : Colors.Red);

		public BackgroundWorker DealsLoader
		{ get; private set; }

		private bool _loaded = false;
		private bool _changingQuality = false;

		public ClassifiedsView()
		{
			InitializeComponent();

			ClassifiedsLoader = new BackgroundWorker();
			ClassifiedsLoader.DoWork += ClassifiedsLoader_DoWork;
			ClassifiedsLoader.RunWorkerCompleted += ClassifiedsLoader_RunWorkerCompleted;

			AvailableItems = new ObservableCollection<ItemViewModel>();
			ItemSearchResultList.ItemsSource = AvailableItems;

			Sellers = new ObservableCollection<ClassifiedsListingViewModel>();
			SellersList.ItemsSource = Sellers;

			Buyers = new ObservableCollection<ClassifiedsListingViewModel>();
			BuyersList.ItemsSource = Buyers;

			Filters = new DealsFilters();
			Filters.Qualities.Add(Quality.Unique);

			DealsLoader = new BackgroundWorker();
			DealsLoader.WorkerReportsProgress = true;
			DealsLoader.DoWork += DealsLoader_DoWork;
			DealsLoader.RunWorkerCompleted += DealsLoader_RunWorkerCompleted;
			DealsLoader.ProgressChanged += DealsLoader_ProgressChanged;

			DealFinder.OnProgressChanged += DealFinder_OnProgressChanged;

			DealsResults = new ObservableCollection<SaleViewModel>();
			DealsResultsList.ItemsSource = DealsResults;

			DealsExcluded = new ObservableCollection<DealExclusionViewModel>();
			DealsExcludedList.ItemsSource = DealsExcluded;
		}

		public void PostLoad(MainWindow window)
		{
			_loaded = true;

			OwnerWindow = window;

			ItemSearchBox.Focus();
			ItemSearchBox_TextChanged(ItemSearchBox, null);

			if (IsMinProfitValid)
			{
				double minProfit = double.Parse(DealsMinProfitBox.Text);
				Filters.DealsMinProfit = new Price(minProfit);
			}

			RefreshDealsFiltersTooltip();
		}

		public void ShowClassifieds(ClassifiedsListing listing)
		{
			ItemPriceInfo buf = new ItemPriceInfo(listing.ItemInstance);
			ShowClassifieds(buf);
		}
		public void ShowClassifieds(ItemPriceInfo info)
		{
			ItemSearchBox.Text = info.Item.ImproperName; // triggers TextChanged event
			ClassifiedsQualities.SelectedQuality = info.Quality; // triggers QualityChanged event
			ClassifiedsCraftableCheck.IsChecked = info.Craftable;
			ClassifiedsCraftableCheck_Click(ClassifiedsCraftableCheck, null); // must be manually triggered
			ClassifiedsTradableCheck.IsChecked = info.Tradable;
			ClassifiedsTradableCheck_Click(ClassifiedsTradableCheck, null); // must be manually triggered
			ClassifiedsAustraliumCheck.IsChecked = info.Australium;
			ClassifiedsAustraliumCheck_Click(ClassifiedsAustraliumCheck, null); // must be manually triggered

			// And...SEARCH!
			ClassifiedsSearchBtn_Click(ClassifiedsSearchBtn, null);
		}

		public void RefreshPriceLabel()
		{
			if (Info == null)
			{
				EvaluatedPrice = null;
			}
			else
			{
				var flagged = PriceChecker.GetPriceFlagged(Info);
				IsMarket = flagged.Contains("market");
				EvaluatedPrice = flagged.Result;
			}

			if (EvaluatedPrice == null)
			{
				ListedPriceTxt.Text = "No known price";
				ListedPriceTxt.ToolTip = null;
				return;
			}
			
			string lbl = ActiveItem.Item.ToString(Props.Quality, Props.Australium);
			lbl = lbl.Shorten(50);
			if (IsMarket)
			{
				lbl += " [Market]";
			}
			if (lbl.EndsWith("..."))
			{
				lbl += " ";
			}
			lbl += ": ";
			ListedPriceTxt.Text = lbl + EvaluatedPrice.Value.ToString();
			ListedPriceTxt.ToolTip = "USD: " + EvaluatedPrice.Value.ToStringUSD();
		}

		public void RefreshDealsFiltersTooltip()
		{
			if (!_loaded)
			{
				return;
			}
			
			DealsSearchBtn.ToolTip = Filters.GetTooltip();
		}

		private void _updateSearch(bool resetPos)
		{
			int index = ItemSearchResultList.SelectedIndex;

			List<ItemViewModel> results = CmdInfo.GetMatchingItems(ItemSearchBox.Text, 50)
							.ConvertAll((i) => new ItemViewModel(i, ClassifiedsQualities));
			AvailableItems.Clear();
			AvailableItems.AddRange(results);
			if (resetPos)
			{
				ItemSearchResultList.SelectedIndex = AvailableItems.IsNullOrEmpty() ? -1 : 0;
			}
			else
			{
				ItemSearchResultList.SelectedIndex = index;
			}
		}

		private void ClassifiedsLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ClassifiedsProgress.Visibility = Visibility.Collapsed;
		}

		private void ClassifiedsLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			Tuple<Item, ListingProperties> arg = e.Argument as Tuple<Item, ListingProperties>;
			Item item = arg.Item1;
			ListingProperties props = arg.Item2;

			ClassifiedsProgress.Dispatcher.Invoke(() => {
				ClassifiedsProgress.Visibility = Visibility.Visible;
			});

			List<ClassifiedsListing> sells = new List<ClassifiedsListing>();
			List<ClassifiedsListing> buys = new List<ClassifiedsListing>();
			try
			{
				sells = CmdClassifiedsBase.GetListings(item, props, OrderType.Sell);
				buys = CmdClassifiedsBase.GetListings(item, props, OrderType.Buy);
			}
			catch (WebException ex)
			{
				SellersList.Dispatcher.Invoke(() => {
					Sellers.Clear();
					Sellers.Add(ClassifiedsListingViewModel.DownloadFailed);
				});

				ClassifiedsBestSellerBtn.Dispatcher.Invoke(() => {
					ClassifiedsBestSellerBtn.IsEnabled = false;
				});

				BestSellerText.Dispatcher.Invoke(() => {
					BestSellerText.Text = "Download Failed.";
					BestSellerText.ToolTip = "Exception Details: " + ex.ToString();
				});

				BuyersList.Dispatcher.Invoke(() => {
					Buyers.Clear();
					Buyers.Add(ClassifiedsListingViewModel.DownloadFailed);
				});

				ClassifiedsBestBuyerBtn.Dispatcher.Invoke(() => {
					ClassifiedsBestBuyerBtn.IsEnabled = false;
				});

				BestBuyerText.Dispatcher.Invoke(() => {
					BestBuyerText.Text = "Download Failed.";
					BestBuyerText.ToolTip = "Exception Details: " + ex.ToString();
				});

				return;
			}

			Price? cheapestSellPrice = null;
			ClassifiedsListing bestSeller = null;
			SellersList.Dispatcher.Invoke(() => {
				Sellers.Clear();
			});
			foreach (ClassifiedsListing s in sells)
			{
				if (cheapestSellPrice == null || s.Price < cheapestSellPrice.Value)
				{
					cheapestSellPrice = s.Price;
					bestSeller = s;
				}

				SellersList.Dispatcher.Invoke(() => {
					Sellers.Add(new ClassifiedsListingViewModel(s));
				});
			}
			if (bestSeller != null)
			{
				BestSeller = new ClassifiedsListingViewModel(bestSeller);
				string s = bestSeller.Price.ToString() + " from " +
					(bestSeller.ListerNickname ?? bestSeller.ListerSteamID64);

				BestSellerText.Dispatcher.Invoke(() => {
					BestSellerText.Text = "Lowest Price: " + s;
					BestSellerText.ToolTip = bestSeller.Comment;
				});
			}
			else
			{
				BestSeller = null;
				BestSellerText.Dispatcher.Invoke(() => {
					BestSellerText.Text = "No sellers found.";
					BestSellerText.ToolTip = null;
				});
			}

			Price? highestBuyPrice = null;
			ClassifiedsListing bestBuyer = null;
			BuyersList.Dispatcher.Invoke(() => {
				Buyers.Clear();
			});
			foreach (ClassifiedsListing b in buys)
			{
				if (highestBuyPrice == null || b.Price > highestBuyPrice.Value)
				{
					highestBuyPrice = b.Price;
					bestBuyer = b;
				}

				BuyersList.Dispatcher.Invoke(() => {
					Buyers.Add(new ClassifiedsListingViewModel(b));
				});
			}
			if (bestBuyer != null)
			{
				BestBuyer = new ClassifiedsListingViewModel(bestBuyer);
				string s = bestBuyer.Price.ToString() + " from " + 
					(bestBuyer.ListerNickname ?? bestBuyer.ListerSteamID64);
				BestBuyerText.Dispatcher.Invoke(() => {
					BestBuyerText.Text = "Highest Price: " + s;
					BestBuyerText.ToolTip = bestBuyer.Comment;
				});
			}
			else
			{
				BestBuyer = null;
				BestBuyerText.Dispatcher.Invoke(() => {
					BestBuyerText.Text = "No buyers found.";
					BestBuyerText.ToolTip = null;
				});
			}

			ClassifiedsBestSellerBtn.Dispatcher.Invoke(() => {
				ClassifiedsBestSellerBtn.IsEnabled = Sellers.HasItems();
			});
			ClassifiedsBestBuyerBtn.Dispatcher.Invoke(() => {
				ClassifiedsBestBuyerBtn.IsEnabled = Buyers.HasItems();
			});
		}

		private void DealsLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			DealsProgress.Value = 100;
			DealsProgress.Visibility = Visibility.Collapsed;
			OwnerWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
		}

		private void DealsLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			int percent = e.ProgressPercentage;
			DealsProgress.Value = percent;
			OwnerWindow.TaskbarItemInfo.ProgressValue = percent / 100.0;

			List<ItemSale> sales = e.UserState as List<ItemSale>;

			DealsResults.Clear();
			foreach (ItemSale s in sales)
			{
				SaleViewModel vm = new SaleViewModel(s);
				DealsResults.Add(vm);
			}

			if (DealsResults.HasItems())
			{
				SaleViewModel last = DealsResults.Last();
				DealsResultsList.ScrollIntoView(last);
			}
		}

		private void DealFinder_OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Dispatcher.Invoke(() => {
				DealsLoader.ReportProgress(e.ProgressPercentage, e.UserState);
			});
		}

		private void DealsLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			DealsFilters filters = e.Argument as DealsFilters;

			var flagged = DealFinder.FindDealsFlagged(Settings.Instance.HomeSteamID64, filters);

			List<ItemSale> results = new List<ItemSale>(flagged.Result);
			results.Sort((a, b) => a.Profit.TotalRefined.CompareTo(b.Profit.TotalRefined));

			var excluded = flagged.Flags;
			excluded.Sort((a, b) => a.Value.CompareTo(b.Value));

			DealsResultsList.Dispatcher.Invoke(() => {
				DealsResults.Clear();
				foreach (ItemSale s in results)
				{
					SaleViewModel vm = new SaleViewModel(s);
					DealsResults.Add(vm);
				}
			});

			DealsExcludedList.Dispatcher.Invoke(() => {
				DealsExcluded.Clear();
				foreach (var kvp in excluded)
				{
					DealExclusionViewModel vm = new DealExclusionViewModel(kvp.Key, kvp.Value);
					DealsExcluded.Add(vm);
				}
			});
		}

		private void ItemSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			_updateSearch(true);
		}

		private void ClassifiedsQualities_QualityChanged(object sender, Quality e)
		{
			if (!_loaded || _changingQuality)
			{
				return;
			}

			_updateSearch(false);
			if (Info == null)
			{
				return;
			}

			Info.Quality = ClassifiedsQualities.SelectedQuality;

			RefreshPriceLabel();
		}

		private void ItemSearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded || ActiveItem == null)
			{
				return;
			}

			PriceCheckResults pcr = null;
			if (ActiveItem != null)
			{
				pcr = CmdPriceCheck.GetPriceCheckResults(ActiveItem.Item);

				_changingQuality = true;
				Quality oldQuality = ClassifiedsQualities.SelectedQuality;
				ClassifiedsQualities.ClearAllQualities();
				foreach (CheckedPrice cp in pcr.All)
				{
					ClassifiedsQualities.EnableQuality(cp.Quality);
				}
				if (ClassifiedsQualities.AvailableQualities.Contains(oldQuality))
				{
					ClassifiedsQualities.SelectedQuality = oldQuality;
				}
				else
				{
					ClassifiedsQualities.SelectFirstAvailable();
				}
				_changingQuality = false;

				if (pcr.All.Exists((cp) => cp.Quality == ClassifiedsQualities.SelectedQuality))
				{
					bool hasCraftable = pcr.All.Exists((cp) => cp.Craftable);
					bool hasUncraftable = pcr.All.Exists((cp) => !cp.Craftable);

					ClassifiedsCraftableCheck.IsEnabled = hasCraftable && hasUncraftable;
					if (!ClassifiedsCraftableCheck.IsEnabled)
					{
						ClassifiedsCraftableCheck.IsChecked = hasCraftable;
					}

					bool hasTradable = pcr.All.Exists((cp) => cp.Tradable);
					bool hasNontradable = pcr.All.Exists((cp) => !cp.Tradable);

					ClassifiedsTradableCheck.IsEnabled = hasTradable && hasNontradable;
					if (!ClassifiedsTradableCheck.IsEnabled)
					{
						ClassifiedsTradableCheck.IsChecked = hasTradable;
					}
				}
				else
				{
					ClassifiedsCraftableCheck.IsEnabled = false;
					ClassifiedsCraftableCheck.IsChecked = true;

					ClassifiedsTradableCheck.IsEnabled = false;
					ClassifiedsTradableCheck.IsChecked = true;
				}

				Info = new ItemPriceInfo(ActiveItem.Item, ClassifiedsQualities.SelectedQuality);
				Info.Craftable = ClassifiedsCraftableCheck.IsChecked ?? false;
				Info.Tradable = ClassifiedsCraftableCheck.IsChecked ?? false;

				ClassifiedsAustraliumCheck.IsEnabled = ActiveItem.Item.CanBeAustralium();
				if (!ClassifiedsAustraliumCheck.IsEnabled)
				{
					ClassifiedsAustraliumCheck.IsChecked = false;
				}
			}

			RefreshPriceLabel();
		}

		private void ClassifiedsCraftableCheck_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Craftable = ClassifiedsCraftableCheck.IsChecked ?? true;
			RefreshPriceLabel();
		}

		private void ClassifiedsTradableCheck_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Tradable = ClassifiedsTradableCheck.IsChecked ?? true;
			RefreshPriceLabel();
		}

		private void ClassifiedsAustraliumCheck_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			if (ClassifiedsAustraliumCheck.IsChecked ?? false)
			{
				// all australiums are strange
				ClassifiedsQualities.SelectedQuality = Quality.Strange;
			}

			Info.Australium = ClassifiedsAustraliumCheck.IsChecked ?? false;
			RefreshPriceLabel();
		}

		private void ClassifiedsSearchBtn_Click(object sender, RoutedEventArgs e)
		{
			if (ActiveItem == null)
			{
				return;
			}

			ClassifiedsLoader.RunWorkerAsync(new Tuple<Item, ListingProperties>(ActiveItem.Item, Props));
		}

		private void ClassifiedsBestSellerBtn_Click(object sender, RoutedEventArgs e)
		{
			if (BestSeller?.OfferLink != null)
			{
				Util.OpenLink(BestSeller.OfferLink);
			}
		}

		private void ClassifiedsBestBuyerBtn_Click(object sender, RoutedEventArgs e)
		{
			if (BestBuyer?.OfferLink != null)
			{
				Util.OpenLink(BestBuyer.OfferLink);
			}
		}

		private void Classifieds_OfferBtn_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			string offerURL = btn?.Tag as string;

			if (offerURL != null)
			{
				Util.OpenLink(offerURL);
			}
		}

		private void ClassifiedsShowBackpack_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			ClassifiedsListing listing = item?.Tag as ClassifiedsListing;

			if (listing != null)
			{
				OwnerWindow.MainTabControl.SelectedIndex = 1;
				OwnerWindow.BackpackView.OpenBackpack(listing.ListerSteamID64, true);
			}
		}

		private void DealsCraftableCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Craftable = DealsCraftableCheck.IsChecked;
			RefreshDealsFiltersTooltip();
		}

		private void DealsHalloweenCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Halloween = DealsHalloweenCheck.IsChecked;
			RefreshDealsFiltersTooltip();
		}

		private void DealsBotkillerCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.Botkiller = DealsBotkillerCheck.IsChecked;
			RefreshDealsFiltersTooltip();
		}

		private void DealsAllClassCheck_Click(object sender, RoutedEventArgs e)
		{
			Filters.AllowAllClass = DealsAllClassCheck.IsChecked ?? true;
			RefreshDealsFiltersTooltip();
		}

		private void DealsMinProfitBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			DealsMinProfitBox.BorderBrush = MinProfitBorder;
			if (IsMinProfitValid)
			{
				double d = double.Parse(DealsMinProfitBox.Text);
				Price p = new Price(d);
				DealsMinProfitBox.ToolTip = "USD: " + p.TotalUSD.ToCurrency();

				Filters.DealsMinProfit = p;
				DealsSearchBtn.IsEnabled = true;
				RefreshDealsFiltersTooltip();
			}
			else
			{
				DealsMinProfitBox.ToolTip = "Not a valid number";
				DealsSearchBtn.IsEnabled = false;
			}
		}

		private void DealsSearchBtn_Click(object sender, RoutedEventArgs e)
		{
			DealsResults.Clear();
			DealsExcluded.Clear();

			DealsProgress.Value = 0;
			DealsProgress.Visibility = Visibility.Visible;
			OwnerWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
			DealsLoader.RunWorkerAsync(Filters);
		}

		private void DealsQualities_MultiSelectionChanged(object sender, MultiQualitySelectorEventArgs e)
		{
			if (e.ActionType == SelectorActionType.Add)
			{
				if (!Filters.Qualities.Contains(e.Selection))
				{
					Filters.Qualities.Add(e.Selection);
				}
			}
			else
			{
				if (Filters.Qualities.Contains(e.Selection))
				{
					Filters.Qualities.Remove(e.Selection);
				}
			}
			RefreshDealsFiltersTooltip();
		}

		private void DealsSlots_SelectionChanged(object sender, MultiSlotSelectorEventArgs e)
		{
			if (e.ActionType == SelectorActionType.Add)
			{
				if (!Filters.Slots.Contains(e.Selection))
				{
					Filters.Slots.Add(e.Selection);
				}
			}
			else
			{
				if (Filters.Slots.Contains(e.Selection))
				{
					Filters.Slots.Remove(e.Selection);
				}
			}
			RefreshDealsFiltersTooltip();
		}

		private void DealsClasses_SelectionChanged(object sender, MultiClassSelectorEventArgs e)
		{
			if (e.ActionType == SelectorActionType.Add)
			{
				if (!Filters.Classes.Contains(e.Selection))
				{
					Filters.Classes.Add(e.Selection);
				}
			}
			else
			{
				if (Filters.Classes.Contains(e.Selection))
				{
					Filters.Classes.Remove(e.Selection);
				}
			}
			RefreshDealsFiltersTooltip();
		}

		private void Deals_OfferBtn_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			string offerURL = btn?.Tag as string;

			if (offerURL != null)
			{
				Util.OpenLink(offerURL);
			}
		}

		private void DealsShowClassifiedsItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			ItemSale sale = item?.Tag as ItemSale;

			if (sale == null)
			{
				return;
			}

			ShowClassifieds(sale.CheapestSeller);
		}

		private void DealsShowBackpack_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			ItemSale sale = item?.Tag as ItemSale;

			if (sale != null)
			{

				OwnerWindow.MainTabControl.SelectedIndex = 1;
				OwnerWindow.BackpackView.OpenBackpack(sale.CheapestSeller.ListerSteamID64, true);
			}
		}
	}
}
