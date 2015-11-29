using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

		public ObservableCollection<Quality> SelectedQualities
		{ get; private set; }

		public Quality SelectedQuality
		{
			get
			{
				return SelectedQualities.First();
			}
			set
			{
				SelectedQualities.Clear();
				SelectedQualities.Add(value);
				UpdateQualities();

				if (QualityChanged != null)
				{
					QualityChanged(this, value);
				}
			}
		}

		public bool AllowMultiple
		{ get; set; }

		public event EventHandler<Quality> QualityChanged;

		public void UpdateQualities()
		{
			for (Quality q = Quality.Stock; q <= Quality.Decorated; q++)
			{
				ToggleButton btn = _getButtonFromQuality(q);
				if (btn != null)
				{
					btn.IsChecked = SelectedQualities.Contains(q);
				}
			}
		}

		public QualitySelector()
		{
			AvailableQualities = new HashSet<Quality>();
			SelectedQualities = new ObservableCollection<Quality>();

			InitializeComponent();
		}

		private ToggleButton _getButtonFromQuality(Quality q)
		{
			switch (q)
			{
			case Quality.Genuine:
				return GenuineBtn;
			case Quality.Vintage:
				return VintageBtn;
			case Quality.Unusual:
				return UnusualBtn;
			case Quality.Unique:
				return UniqueBtn;
			case Quality.Strange:
				return StrangeBtn;
			case Quality.Haunted:
				return HauntedBtn;
			case Quality.Collectors:
				return CollectorsBtn;
			default:
				return null;
			}
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

		private void HitQuality(Quality q)
		{
			ToggleButton btn = _getButtonFromQuality(q);
			if (btn == null)
			{
				return;
			}

			if (AllowMultiple)
			{
				if (SelectedQualities.Contains(q))
				{
					SelectedQualities.Remove(q);
					UpdateQualities();
				}
				else
				{
					SelectedQualities.Add(q);
					UpdateQualities();
				}
			}
			else
			{
				SelectedQuality = q;
			}
		}

		private void UniqueBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Unique);
		}

		private void StrangeBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Strange);
		}

		private void GenuineBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Genuine);
		}

		private void CollectorsBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Collectors);
		}

		private void VintageBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Vintage);
		}

		private void HauntedBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Haunted);
		}

		private void UnusualBtn_Click(object sender, RoutedEventArgs e)
		{
			HitQuality(Quality.Unusual);
		}

		public void SelectFirstAvailable()
		{
			if (AvailableQualities.IsNullOrEmpty())
			{
				SelectedQuality = Quality.Unique;
			}

			SelectedQuality = AvailableQualities.FirstOrDefault();
		}

		private void UserControl_Initialized(object sender, EventArgs e)
		{
			if (SelectedQualities.IsEmpty())
			{
				SelectFirstAvailable();
			}
		}
	}
}
