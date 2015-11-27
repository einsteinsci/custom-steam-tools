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
		public ObservableCollection<UnusualEffect> Unusuals
		{ get; private set; }

		public ObservableCollection<ItemViewModel> AvailableItems
		{ get; private set; }

		public ItemViewModel ActiveItem => ItemSearchResultList.SelectedItem as ItemViewModel;

		public ItemPriceInfo Info
		{ get; private set; }

		public UnusualEffect ActiveUnusual => UnusualEffectsDropdown.SelectedItem as UnusualEffect;

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

				return EvaluatedPrice.Value.ToStringUSD();
			}
		}

		public bool IsMarket
		{ get; private set; }

		public ItemsView()
		{
			InitializeComponent();

			Unusuals = new ObservableCollection<UnusualEffect>();
			UnusualEffectsDropdown.ItemsSource = Unusuals;

			AvailableItems = new ObservableCollection<ItemViewModel>();
			ItemSearchResultList.ItemsSource = AvailableItems;
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

			AvailableItems = new ObservableCollection<ItemViewModel>(results);
			ItemSearchResultList.ItemsSource = AvailableItems;
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

			if (ActiveItem != null)
			{
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
			ItemSlotPlain plain = ActiveItem?.Item.PlainSlot ?? ItemSlotPlain.Unused;
			Killstreaks.IsEnabled = plain == ItemSlotPlain.Weapon;
			UnusualEffectsDropdown.IsEnabled = plain == ItemSlotPlain.Cosmetic;

			Unusuals.Clear();
			if (ActiveItem != null)
			{
				PriceCheckResults pcr = CmdPriceCheck.GetPriceCheckResults(ActiveItem.Item);
				List<UnusualEffect> ues = pcr.Unusuals.ConvertAll((cp) => cp.Unusual);
				Unusuals.AddRange(ues);
				UnusualEffectsDropdown.SelectedIndex = Unusuals.IsNullOrEmpty() ? -1 : 0;
				Info.Unusual = ActiveUnusual;
			}

			RefreshPriceLabels();
		}

		public void PostLoad()
		{
			_loaded = true;

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
			PriceActualText.ToolTip = EvaluatedPriceUSD;
		}

		private void Qualities_QualityChanged(object sender, Quality q)
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

			Info.Quality = Qualities.SelectedQuality;
			RefreshPriceLabels();
		}

		private void Killstreaks_KillstreakChanged(object sender, CustomSteamTools.Market.KillstreakType e)
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

			Info.Killstreak = Killstreaks.SelectedKillstreak;
			RefreshPriceLabels();
		}

		private void UnusualEffectsDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded || Info == null)
			{
				return;
			}

			Info.Unusual = ActiveUnusual;
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
			if (!_loaded)
			{
				return;
			}

			_updateSearch(false);
			if (Info == null)
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
	}
}
