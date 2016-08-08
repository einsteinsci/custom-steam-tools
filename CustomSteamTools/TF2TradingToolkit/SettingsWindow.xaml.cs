using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using CustomSteamTools.Utils;
using UltimateUtil;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : MahApps.Metro.Controls.MetroWindow
	{
		public Settings Instance
		{ get; private set; }

		private bool _loaded;

		public SettingsWindow()
		{
			InitializeComponent();

			if (Settings.Instance != null)
			{
				Instance = Settings.Instance.Clone() as Settings; // shallow copy
			}
			else
			{
				Settings.Load();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TimeoutBox.Text = Instance.DownloadTimeoutSeconds.ToString("F2");
			SteamIDBox.Text = Instance.HomeSteamID64;
			SteamNameBox.Text = Instance.SteamPersonaName;
			BpTfAPIKeyBox.Text = Instance.BackpackTFAPIKey;
			SteamAPIKeyBox.Text = Instance.SteamAPIKey;
			MaxListingsBox.Text = Instance.DealsPriceDropThresholdListingCount.ToString();
			PriceThresholdBox.Text = Instance.DealsPriceDropThresholdPriceBelow.ToString("F2");

			_loaded = true;

			ValidateSettings();
		}

		public void ValidateSettings()
		{
			bool failed = false;

			SolidColorBrush validBrush = new SolidColorBrush(Colors.DarkGray);
			SolidColorBrush invalidBrush = new SolidColorBrush(Colors.Red);
			
			if (TimeoutBox.Text.IsValidDouble())
			{
				TimeoutBox.ToolTip = null;
				TimeoutBox.BorderBrush = validBrush;
			}
			else
			{
				TimeoutBox.ToolTip = "Not a valid number.";
				TimeoutBox.BorderBrush = invalidBrush;
				failed = true;
			}

			if (SteamIDBox.Text.IsNullOrWhitespace())
			{
				SteamIDBox.ToolTip = "You must provide a Steam ID.";
				SteamIDBox.BorderBrush = invalidBrush;
				failed = true;
			}
			else
			{
				SteamIDBox.ToolTip = null;
				SteamIDBox.BorderBrush = validBrush;
			}

			if (BpTfAPIKeyBox.Text.IsNullOrWhitespace())
			{
				BpTfAPIKeyBox.ToolTip = "You must provide a backpack.tf API key.";
				BpTfAPIKeyBox.BorderBrush = invalidBrush;
				failed = true;
			}
			else
			{
				BpTfAPIKeyBox.ToolTip = null;
				BpTfAPIKeyBox.BorderBrush = validBrush;
			}

			if (SteamAPIKeyBox.Text.IsNullOrWhitespace())
			{
				SteamAPIKeyBox.ToolTip = "You must provide a Steam API key.";
				SteamAPIKeyBox.BorderBrush = invalidBrush;
				failed = true;
			}
			else
			{
				SteamAPIKeyBox.ToolTip = null;
				SteamAPIKeyBox.BorderBrush = validBrush;
			}

			if (MaxListingsBox.Text.IsValidInt())
			{
				MaxListingsBox.ToolTip = null;
				MaxListingsBox.BorderBrush = validBrush;
			}
			else
			{
				MaxListingsBox.ToolTip = "Not a valid integer.";
				MaxListingsBox.BorderBrush = invalidBrush;
				failed = true;
			}

			if (PriceThresholdBox.Text.IsValidDouble())
			{
				double d = double.Parse(PriceThresholdBox.Text);
				if (d.IsBetween(0, 1))
				{
					PriceThresholdBox.ToolTip = null;
					PriceThresholdBox.BorderBrush = validBrush;
				}
				else
				{
					PriceThresholdBox.ToolTip = "Must be between 0 and 1.";
					PriceThresholdBox.BorderBrush = invalidBrush;
					failed = true;
				}
			}
			else
			{
				PriceThresholdBox.ToolTip = "Not a valid number.";
				PriceThresholdBox.BorderBrush = invalidBrush;
				failed = true;
			}

			SaveBtn.IsEnabled = !failed;
		}

		private void TimeoutBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (TimeoutBox.Text.IsValidDouble())
			{
				Instance.DownloadTimeoutSeconds = double.Parse(TimeoutBox.Text);
			}

			ValidateSettings();
		}

		private void SteamIDBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			Instance.HomeSteamID64 = SteamIDBox.Text;
			ValidateSettings();
		}

		private void SteamNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			Instance.SteamPersonaName = SteamNameBox.Text;
			ValidateSettings();
		}

		private void BpTfAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			Instance.BackpackTFAPIKey = BpTfAPIKeyBox.Text;
			ValidateSettings();
		}

		private void SteamAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			Instance.SteamAPIKey = SteamAPIKeyBox.Text;
			ValidateSettings();
		}

		private void MaxListingsBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (MaxListingsBox.Text.IsValidInt())
			{
				Instance.DealsPriceDropThresholdListingCount = int.Parse(MaxListingsBox.Text);
			}

			ValidateSettings();
		}

		private void PriceThresholdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			if (PriceThresholdBox.Text.IsValidDouble())
			{
				Instance.DealsPriceDropThresholdPriceBelow = double.Parse(PriceThresholdBox.Text);
			}

			ValidateSettings();
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
