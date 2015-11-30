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
using CustomSteamTools.Commands;
using CustomSteamTools.Lookup;
using CustomSteamTools.Schema;
using TF2TradingToolkit.ViewModel;
using UltimateUtil;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for ItemsView.xaml
	/// </summary>
	public partial class ItemsView : UserControl
	{
		public MainWindow OwnerWindow
		{ get; private set; }

		public sealed class DeleteCalcCommand : ICommand
		{
			public ItemsView View
			{ get; private set; }

			public event EventHandler CanExecuteChanged;

			public DeleteCalcCommand(ItemsView view)
			{
				View = view;
			}

			public bool CanExecute(object parameter)
			{
				return View.CalcList.SelectedIndex != -1;
			}

			public void Execute(object parameter)
			{
				if (CanExecute(null))
				{
					View.CalculatorContents.RemoveAt(View.CalcList.SelectedIndex);
				}
				View.RefreshCalcLabels();
			}
		}

		public ObservableCollection<UnusualViewModel> Unusuals
		{ get; private set; }

		public ObservableCollection<ItemViewModel> AvailableItems
		{ get; private set; }

		public ItemViewModel ActiveItem => ItemSearchResultList.SelectedItem as ItemViewModel;

		public ItemPriceInfo Info
		{ get; private set; }

		public UnusualViewModel ActiveUnusual => UnusualEffectsDropdown.SelectedItem as UnusualViewModel;

		public ObservableCollection<PricedViewModel> CalculatorContents
		{ get; private set; }

		public DeleteCalcCommand DeleteCalcCmd
		{ get; private set; }

		public PriceRange CalcTotalPrice
		{
			get
			{
				PriceRange res = new PriceRange(Price.Zero);
				foreach (PricedViewModel pvm in CalculatorContents)
				{
					res += pvm.Price;
				}

				return res;
			}
		}
		public string CalcTotalPriceString => CalcTotalPrice.ToString();
		public string CalcTotalPriceUSD => "USD: " + CalcTotalPrice.ToStringUSD();

		private bool _loaded = false;

		public PriceRange? EvaluatedPrice
		{ get; private set; }

		public string EvaluatedPriceString
		{
			get
			{
				if (EvaluatedPrice == null)
				{
					return null;
				}

				return EvaluatedPrice.Value.ToString();
			}
		}
		public string EvaluatedPriceUSD
		{
			get
			{
				if (EvaluatedPrice == null)
				{
					return null;
				}

				return "(" + EvaluatedPrice.Value.ToStringUSD() + ")";
			}
		}

		public bool IsMarket
		{ get; private set; }

		public bool IsCalcEditing
		{ get; private set; }

		private bool _changingQuality = false;

		public ItemsView()
		{
			InitializeComponent();

			DeleteCalcCmd = new DeleteCalcCommand(this);

			Unusuals = new ObservableCollection<UnusualViewModel>();
			UnusualEffectsDropdown.ItemsSource = Unusuals;

			AvailableItems = new ObservableCollection<ItemViewModel>();
			ItemSearchResultList.ItemsSource = AvailableItems;

			CalculatorContents = new ObservableCollection<PricedViewModel>();
			CalcList.ItemsSource = CalculatorContents;
		}

		private void ItemSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			_updateSearch(true);
		}

		private void _updateSearch(bool resetPos)
		{
			int index = ItemSearchResultList.SelectedIndex;

			List<ItemViewModel> results = CmdInfo.GetMatchingItems(ItemSearchBox.Text, 50)
							.ConvertAll((i) => new ItemViewModel(i, Qualities));
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

		private void ItemSearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded || ActiveItem == null)
			{
				return;
			}

			ItemSlotPlain plain = ActiveItem?.Item.PlainSlot ?? ItemSlotPlain.Unused;
			Killstreaks.IsEnabled = plain == ItemSlotPlain.Weapon;
			if (!Killstreaks.IsEnabled)
			{
				Killstreaks.SelectedKillstreak = CustomSteamTools.Market.KillstreakType.None;
			}

			PriceCheckResults pcr = null;
			if (ActiveItem != null)
			{
				pcr = CmdPriceCheck.GetPriceCheckResults(ActiveItem.Item);

				_changingQuality = true;
				Quality oldQuality = Qualities.SelectedQuality;
				Qualities.ClearAllQualities();
				foreach (CheckedPrice cp in pcr.All)
				{
					Qualities.EnableQuality(cp.Quality);
				}
				if (Qualities.AvailableQualities.Contains(oldQuality))
				{
					Qualities.SelectedQuality = oldQuality;
				}
				else
				{
					Qualities.SelectFirstAvailable();
				}
				_changingQuality = false;

				if (pcr.All.Exists((cp) => cp.Quality == Qualities.SelectedQuality))
				{
					bool hasCraftable = pcr.All.Exists((cp) => cp.Craftable);
					bool hasUncraftable = pcr.All.Exists((cp) => !cp.Craftable);

					CraftableCheckbox.IsEnabled = hasCraftable && hasUncraftable;
					if (!CraftableCheckbox.IsEnabled)
					{
						CraftableCheckbox.IsChecked = hasCraftable;
					}

					bool hasTradable = pcr.All.Exists((cp) => cp.Tradable);
					bool hasNontradable = pcr.All.Exists((cp) => !cp.Tradable);

					TradableCheckbox.IsEnabled = hasTradable && hasNontradable;
					if (!TradableCheckbox.IsEnabled)
					{
						TradableCheckbox.IsChecked = hasTradable;
					}
				}
				else
				{
					CraftableCheckbox.IsEnabled = false;
					CraftableCheckbox.IsChecked = true;

					TradableCheckbox.IsEnabled = false;
					TradableCheckbox.IsChecked = true;
				}

				Info = new ItemPriceInfo(ActiveItem.Item, Qualities.SelectedQuality);
				Info.Killstreak = Killstreaks.SelectedKillstreak;
				Info.Craftable = CraftableCheckbox.IsChecked ?? false;
				Info.Tradable = CraftableCheckbox.IsChecked ?? false;

				AustraliumCheckbox.IsEnabled = ActiveItem.Item.CanBeAustralium();
				if (!AustraliumCheckbox.IsEnabled)
				{
					AustraliumCheckbox.IsChecked = false;
				}
			}

			UnusualEffectsDropdown.IsEnabled = Qualities.SelectedQuality == Quality.Unusual;
			if (!UnusualEffectsDropdown.IsEnabled)
			{
				UnusualEffectsDropdown.SelectedIndex = -1;
			}

			Unusuals.Clear();
			if (ActiveItem != null)
			{
				List<UnusualViewModel> ues = pcr.Unusuals.ConvertAll((cp) => new UnusualViewModel(cp.Unusual));
				Unusuals.AddRange(ues);
				UnusualEffectsDropdown.SelectedIndex = Unusuals.IsNullOrEmpty() ? -1 : 0;
				Info.Unusual = ActiveUnusual?.Effect;
			}

			RefreshPriceLabels();
		}

		public void PostLoad(MainWindow window)
		{
			_loaded = true;

			OwnerWindow = window;

			ItemSearchBox.Focus();
			ItemSearchBox_TextChanged(null, null);
			RefreshPriceLabels();
		}

		public void RefreshPriceLabels()
		{
			if (Info == null)
			{
				EvaluatedPrice = null;
				return;
			}

			var flagged = PriceChecker.GetPriceFlagged(Info);
			IsMarket = flagged.Contains("market");
			EvaluatedPrice = flagged.Result;

			// Bindings aren't working for some reason...
			PriceLabelText.Text = IsMarket ? "Price (Market):" : "Price:";
			PriceActualText.Text = EvaluatedPriceString;
			PriceUSDText.Text = EvaluatedPriceUSD;

			if (CalcList.SelectedIndex != -1 && IsCalcEditing)
			{
				int index = CalcList.SelectedIndex;
				CalculatorContents[index] = new PricedViewModel(Info, EvaluatedPrice ?? PriceRange.Zero);
				CalcList.SelectedIndex = index;
				RefreshCalcLabels();
			}
		}

		public void RefreshCalcLabels()
		{
			if (!_loaded)
			{
				return;
			}

			CalcRefText.Text = CalcTotalPriceString;
			CalcRefText.ToolTip = CalcTotalPriceUSD;
		}

		private void Qualities_QualityChanged(object sender, Quality q)
		{
			if (!_loaded || _changingQuality)
			{
				return;
			}
			
			if (Info == null)
			{
				return;
			}

			Info.Quality = Qualities.SelectedQuality;
			UnusualEffectsDropdown.IsEnabled = Qualities.SelectedQuality == Quality.Unusual;

			RefreshPriceLabels();
		}

		private void Killstreaks_KillstreakChanged(object sender, CustomSteamTools.Market.KillstreakType e)
		{
			if (!_loaded)
			{
				return;
			}
			
			if (Info == null)
			{
				return;
			}

			Info.Killstreak = Killstreaks.SelectedKillstreak;
			RefreshPriceLabels();
		}

		private void UnusualEffectsDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Unusual = ActiveUnusual?.Effect;
			RefreshPriceLabels();
		}

		private void TradableCheckbox_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Tradable = TradableCheckbox.IsChecked ?? true;
			RefreshPriceLabels();
		}

		private void CraftableCheckbox_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Craftable = CraftableCheckbox.IsChecked ?? true;
			RefreshPriceLabels();
		}

		private void AustraliumCheckbox_Checked(object sender, RoutedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			_updateSearch(false);
			if (Info == null)
			{
				return;
			}

			Info.Australium = AustraliumCheckbox.IsChecked ?? false;
			RefreshPriceLabels();
		}

		private void CalcAddItem_Click(object sender, RoutedEventArgs e)
		{
			if (!_loaded || Info == null || EvaluatedPrice == null)
			{
				return;
			}

			CalculatorContents.Add(new PricedViewModel(Info, EvaluatedPrice.Value));
			RefreshCalcLabels();
		}

		private void CalcClearBtn_Click(object sender, RoutedEventArgs e)
		{
			CalculatorContents.Clear();
			RefreshCalcLabels();
		}

		private void CalcList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CalcClearBtn.IsEnabled = CalculatorContents.HasItems();
			CalcEditBtn.IsEnabled = CalcList.SelectedIndex != -1;
		}

		private void CalcEditBtn_Click(object sender, RoutedEventArgs e)
		{
			PricedViewModel selected = CalcList.SelectedItem as PricedViewModel;
			if (selected == null)
			{
				return;
			}

			ItemSearchBox.Text = selected.Info.Item.ToString();

			Info = selected.Info;
			Qualities.SelectedQuality = Info.Quality;
			Killstreaks.SelectedKillstreak = Info.Killstreak;
			CraftableCheckbox.IsChecked = Info.Craftable;
			TradableCheckbox.IsChecked = Info.Tradable;
			if (Info.Unusual != null)
			{
				UnusualEffectsDropdown.SelectedItem = Info.Unusual;
			}
			else
			{
				UnusualEffectsDropdown.SelectedIndex = -1;
			}
		}

		private void CalcRemoveBtn_Click(object sender, RoutedEventArgs e)
		{
			DeleteCalcCmd.Execute(null);
		}

		private void SearchClassifiedsItem_Click(object sender, RoutedEventArgs e)
		{
			OwnerWindow.MainTabControl.SelectedIndex = 2;
			OwnerWindow.ClassifiedsView.ShowClassifieds(Info);
		}
	}
}
