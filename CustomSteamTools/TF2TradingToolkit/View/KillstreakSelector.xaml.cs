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
using CustomSteamTools.Market;
using CustomSteamTools.Schema;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for QualitySelector.xaml
	/// </summary>
	public partial class KillstreakSelector : UserControl
	{
		public KillstreakType SelectedKillstreak
		{
			get
			{
				return _selectedQuality;
			}
			set
			{
				_selectedQuality = value;
				UpdateKillstreak();

				if (KillstreakChanged != null)
				{
					KillstreakChanged(this, value);
				}
			}
		}
		private KillstreakType _selectedQuality = KillstreakType.None;

		public event EventHandler<KillstreakType> KillstreakChanged;

		public void UpdateKillstreak()
		{
			NoneBtn.IsChecked = false;
			BasicBtn.IsChecked = false;
			SpecializedBtn.IsChecked = false;
			ProfessionalBtn.IsChecked = false;

			switch (SelectedKillstreak)
			{
			case KillstreakType.None:
				NoneBtn.IsChecked = true;
				break;
			case KillstreakType.Basic:
				BasicBtn.IsChecked = true;
				break;
			case KillstreakType.Specialized:
				SpecializedBtn.IsChecked = true;
				break;
			case KillstreakType.Professional:
				ProfessionalBtn.IsChecked = true;
				break;
			}
		}

		public KillstreakSelector()
		{
			InitializeComponent();
		}

		private void NoneBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedKillstreak = KillstreakType.None;
		}

		private void BasicBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedKillstreak = KillstreakType.Basic;
		}

		private void SpecializedBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedKillstreak = KillstreakType.Specialized;
		}

		private void ProfessionalBtn_Click(object sender, RoutedEventArgs e)
		{
			SelectedKillstreak = KillstreakType.Professional;
		}
	}
}
