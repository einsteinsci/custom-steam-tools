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
using CustomSteamTools.Schema;
using UltimateUtil;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for QualitySelector.xaml
	/// </summary>
	public partial class QualitySelector : UserControl
	{
		public HashSet<Quality> AvailableQualities
		{ get; private set; }

		public Quality SelectedQuality
		{
			get
			{
				return _selectedQuality;
			}
			set
			{
				_selectedQuality = value;
				UpdateQualities();

				if (QualityChanged != null)
				{
					QualityChanged(this, value);
				}
			}
		}
		private Quality _selectedQuality = Quality.Unique;

		public event EventHandler<Quality> QualityChanged;

		public void UpdateQualities()
		{
			UniqueBtn.IsChecked = false;
			StrangeBtn.IsChecked = false;
			GenuineBtn.IsChecked = false;
			CollectorsBtn.IsChecked = false;
			VintageBtn.IsChecked = false;
			HauntedBtn.IsChecked = false;
			UnusualBtn.IsChecked = false;

			switch (SelectedQuality)
			{
			case Quality.Genuine:
				GenuineBtn.IsChecked = true;
				break;
			case Quality.Vintage:
				VintageBtn.IsChecked = true;
				break;
			case Quality.Unusual:
				UnusualBtn.IsChecked = true;
				break;
			case Quality.Unique:
				UniqueBtn.IsChecked = true;
				break;
			case Quality.Strange:
				StrangeBtn.IsChecked = true;
				break;
			case Quality.Haunted:
				HauntedBtn.IsChecked = true;
				break;
			case Quality.Collectors:
				CollectorsBtn.IsChecked = true;
				break;
			}
		}

		public QualitySelector()
		{
			InitializeComponent();

			AvailableQualities = new HashSet<Quality>();
		}

		public void DisableQuality(Quality q)
		{
			switch (q)
			{
			case Quality.Genuine:
				GenuineBtn.IsEnabled = false;
				break;
			case Quality.Vintage:
				VintageBtn.IsEnabled = false;
				break;
			case Quality.Unusual:
				UnusualBtn.IsEnabled = false;
				break;
			case Quality.Unique:
				UniqueBtn.IsEnabled = false;
				break;
			case Quality.Strange:
				StrangeBtn.IsEnabled = false;
				break;
			case Quality.Haunted:
				HauntedBtn.IsEnabled = false;
				break;
			case Quality.Collectors:
				CollectorsBtn.IsEnabled = false;
				break;
			default:
				return;
			}

			AvailableQualities.Remove(q);
		}

		public void EnableQuality(Quality q)
		{
			switch (q)
			{
			case Quality.Genuine:
				GenuineBtn.IsEnabled = true;
				break;
			case Quality.Vintage:
				VintageBtn.IsEnabled = true;
				break;
			case Quality.Unusual:
				UnusualBtn.IsEnabled = true;
				break;
			case Quality.Unique:
				UniqueBtn.IsEnabled = true;
				break;
			case Quality.Strange:
				StrangeBtn.IsEnabled = true;
				break;
			case Quality.Haunted:
				HauntedBtn.IsEnabled = true;
				break;
			case Quality.Collectors:
				CollectorsBtn.IsEnabled = true;
				break;
			default:
				return;
			}

			AvailableQualities.Add(q);
		}

		public void ClearAllQualities()
		{
			for (Quality q = Quality.Stock; q <= Quality.Decorated; q++)
			{
				DisableQuality(q);
			}
		}

		private void UniqueBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Unique;
		}

		private void StrangeBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Strange;
		}

		private void GenuineBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Genuine;
		}

		private void CollectorsBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Collectors;
		}

		private void VintageBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Vintage;
		}

		private void HauntedBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Haunted;
		}

		private void UnusualBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuality = Quality.Unusual;
		}

		public void SelectFirstAvailable()
		{
			if (AvailableQualities.IsNullOrEmpty())
			{
				SelectedQuality = Quality.Unique;
			}

			SelectedQuality = AvailableQualities.FirstOrDefault();
		}
	}
}
