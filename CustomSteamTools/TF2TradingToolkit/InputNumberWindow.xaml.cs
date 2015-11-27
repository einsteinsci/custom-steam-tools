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
using System.Windows.Shapes;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for InputNumberWindow.xaml
	/// </summary>
	public partial class InputNumberWindow : Elysium.Controls.Window
	{
		public double? InputResult
		{
			get
			{
				if (InputBox == null)
				{
					return null;
				}

				double d;
				if (double.TryParse(InputBox.Text, out d))
				{
					return d;
				}
				return null;
			}
		}

		public InputNumberWindow(string prompt)
		{
			InitializeComponent();
			Title = prompt;
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = InputResult.HasValue;
			Close();
		}

		private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (OKBtn == null)
			{
				return;
			}

			OKBtn.IsEnabled = InputResult.HasValue;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (InputResult == null)
			{
				e.Cancel = true;
			}
		}
	}
}
